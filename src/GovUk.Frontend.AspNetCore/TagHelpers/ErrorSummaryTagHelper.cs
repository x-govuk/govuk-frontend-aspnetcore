using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS error summary component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    ErrorSummaryTitleTagHelper.TagName,
    ErrorSummaryTitleTagHelper.ShortTagName,
    ErrorSummaryDescriptionTagHelper.TagName,
    ErrorSummaryDescriptionTagHelper.ShortTagName,
    ErrorSummaryItemTagHelper.TagName,
    ErrorSummaryItemTagHelper.ShortTagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.ErrorSummary)]
public class ErrorSummaryTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-summary";

    private const string DisableAutoFocusAttributeName = "disable-auto-focus";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IGovUkFrontendLocalizer _localizer;

    /// <summary>
    /// Creates a new <see cref="ErrorSummaryTagHelper"/>.
    /// </summary>
    public ErrorSummaryTagHelper(IComponentGenerator componentGenerator, IGovUkFrontendLocalizer localizer)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(localizer);

        _componentGenerator = componentGenerator;
        _localizer = localizer;
    }

    /// <summary>
    /// Whether to disable the behavior that focuses the error summary when the page loads.
    /// </summary>
    [HtmlAttributeName(DisableAutoFocusAttributeName)]
    public bool? DisableAutoFocus { get; set; }

    /// <summary>
    /// Gets the <see cref="ViewContext"/> of the executing view.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    [DisallowNull]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(typeof(ErrorSummaryContext), new ErrorSummaryContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var errorSummaryContext = context.GetContextItem<ErrorSummaryContext>();

        _ = await output.GetChildContentAsync();

        var containerErrorContext = ViewContext!.HttpContext.GetPageErrorContext();

        IReadOnlyCollection<ErrorSummaryOptionsErrorItem> errorList = errorSummaryContext.HaveExplicitItems
            ? errorSummaryContext.Items
                .Select(i => new ErrorSummaryOptionsErrorItem
                {
                    Href = i.Href,
                    Text = null,
                    Html = i.Html,
                    Attributes = i.Attributes,
                    ItemAttributes = i.ItemAttributes
                })
                .ToArray()
            : containerErrorContext.GetErrorSummaryItems();
        if (errorSummaryContext.Title is null &&
            errorSummaryContext.Description is null &&
            errorList.Count == 0)
        {
            output.SuppressOutput();
            return;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var (titleText, titleHtml) = DefaultComponentGenerator.GetErrorSummaryTitle(
            _localizer,
            errorSummaryContext.Title?.Html);

        var component = await _componentGenerator.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = titleText,
            TitleHtml = titleHtml,
            DescriptionText = null,
            DescriptionHtml = errorSummaryContext.Description?.Html,
            ErrorList = errorList,
            Classes = classes,
            Attributes = attributes,
            DisableAutoFocus = DisableAutoFocus,
            TitleAttributes = errorSummaryContext?.Title?.Attributes,
            DescriptionAttributes = errorSummaryContext?.Description?.Attributes
        });

        component.ApplyToTagHelper(output);

        containerErrorContext.ErrorSummaryHasBeenRendered = true;
    }
}
