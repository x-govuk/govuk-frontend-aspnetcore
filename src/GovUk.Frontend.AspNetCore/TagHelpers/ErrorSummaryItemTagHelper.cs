using System.ComponentModel;
using System.Diagnostics;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an error item in a GDS error summary component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ErrorSummaryTagHelper.TagName)]
public class ErrorSummaryItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-summary-item";

    private const string AspForAttributeName = "asp-for";
    private const string ForAttributeName = "for";
    private const string LinkAttributesPrefix = "link-";

    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates a new <see cref="ErrorSummaryItemTagHelper"/>.
    /// </summary>
    public ErrorSummaryItemTagHelper(
        IOptions<GovUkFrontendOptions> optionsAccessor)
        : this(optionsAccessor, modelHelper: new DefaultModelHelper())
    {
    }

    internal ErrorSummaryItemTagHelper(
        IOptions<GovUkFrontendOptions> optionsAccessor,
        IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        ArgumentNullException.ThrowIfNull(modelHelper);

        _optionsAccessor = optionsAccessor;
        _modelHelper = modelHelper;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(AspForAttributeName)]
    [Obsolete("Use the 'for' attribute instead.", DiagnosticId = DiagnosticIds.UseForAttributeInstead)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ModelExpression? AspFor
    {
        get => For;
        set => For = value;
    }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>a</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LinkAttributesPrefix)]
    public IDictionary<string, string?>? LinkAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagMode == TagMode.SelfClosing && For is null)
        {
            throw new InvalidOperationException(
                $"Content is required when the '{ForAttributeName}' attribute is not specified.");
        }

        var errorSummaryContext = context.GetContextItem<ErrorSummaryContext>();
        errorSummaryContext.HaveExplicitItems = true;

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        TemplateString itemHtml;

        if (output.TagMode == TagMode.StartTagAndEndTag)
        {
            itemHtml = content.ToTemplateString();
        }
        else
        {
            Debug.Assert(For is not null);

            var validationMessage = _modelHelper.GetValidationMessage(
                ViewContext!,
                For!.ModelExplorer,
                For.Name);

            if (validationMessage is null)
            {
                return;
            }

            itemHtml = validationMessage;
        }

        TemplateString? resolvedHref = null;

        if (output.Attributes.ContainsName("href"))
        {
            resolvedHref = output.GetUrlAttribute("href");
        }
        else if (For is not null)
        {
            static string CreateIdFromName(string name) => TagBuilder.CreateSanitizedId(name, Constants.IdAttributeDotReplacement);

            var errorFieldId = CreateIdFromName(_modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name));

            // Date inputs are special; they don't have an element with ID which exactly corresponds to the name derived above;
            // the IDs are suffixed with .Day .Month and .Year for each of the components.
            // We don't have a perfect way to know whether this error is for a date input.
            // The best we can do is look at the type for the ModelExpression and see if it looks like a date type.
            // If it does look like a date type we also consult DateInputParseErrorsProvider to know which input to link to
            // (e.g. if .Day is valid but .Month and .Year are not, we link to .Month as the first input with errors.)
            // (Note we cannot rely on DateInputParseErrorsProvider for identifying date inputs since we could have errors
            // that didn't come from model binding so TryGetErrorsForModel will return false.)

            if (IsModelExpressionForDate())
            {
                var dateInputErrorItems = DateInputItemTypes.All;

                var fullName = _modelHelper.GetFullHtmlFieldName(ViewContext!, For.Name);
                var invalidDateException = ViewContext!.ModelState[fullName]?.Errors.FirstOrDefault(e => e.Exception is DateInputParseException)
                    ?.Exception as DateInputParseException;

                if (invalidDateException?.ParseErrors is DateInputParseErrors parseErrors)
                {
                    dateInputErrorItems = parseErrors.GetItemsWithError();
                }

                Debug.Assert(dateInputErrorItems != DateInputItemTypes.None);

                errorFieldId = dateInputErrorItems.HasFlag(DateInputItemTypes.Day)
                    ? CreateIdFromName(
                        ModelNames.CreatePropertyModelName(errorFieldId, DateInputModelBinder.DayInputName))
                    : dateInputErrorItems.HasFlag(DateInputItemTypes.Month)
                        ? CreateIdFromName(
                                            ModelNames.CreatePropertyModelName(errorFieldId, DateInputModelBinder.MonthInputName))
                        : CreateIdFromName(
                                            ModelNames.CreatePropertyModelName(errorFieldId, DateInputModelBinder.YearInputName));
            }

            resolvedHref = $"#{errorFieldId}";
        }

        errorSummaryContext.AddItem(
            new ErrorSummaryContextItem(
                resolvedHref,
                itemHtml,
                new AttributeCollection(output.Attributes),
                new AttributeCollection(LinkAttributes)));

        output.SuppressOutput();

        bool IsModelExpressionForDate()
        {
            Debug.Assert(For is not null);

            var modelType = Nullable.GetUnderlyingType(For!.Metadata.ModelType) ?? For!.Metadata.ModelType;
            return _optionsAccessor.Value.FindDateInputModelConverterForType(modelType) is not null;
        }
    }
}
