using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    CheckboxesFieldsetTagHelper.TagName,
    CheckboxesItemTagHelper.TagName,
    CheckboxesItemDividerTagHelper.TagName,
    HintTagName,
    ErrorMessageTagName,
    CheckboxesBeforeInputsTagHelper.TagName,
    CheckboxesAfterInputsTagHelper.TagName
)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class CheckboxesTagHelper : TagHelper
{
    internal const string ErrorMessageTagName = "govuk-checkboxes-error-message";
    internal const string HintTagName = "govuk-checkboxes-hint";
    internal const string TagName = "govuk-checkboxes";

    private const string AspForAttributeName = "asp-for";
    private const string AttributesPrefix = "checkboxes-";
    private const string DescribedByAttributeName = "described-by";
    private const string ForAttributeName = "for";
    private const string IdPrefixAttributeName = "id-prefix";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string NameAttributeName = "name";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates a new <see cref="CheckboxesTagHelper"/>.
    /// </summary>
    public CheckboxesTagHelper(IComponentGenerator componentGenerator)
        : this(componentGenerator, new DefaultModelHelper())
    {
    }

    internal CheckboxesTagHelper(IComponentGenerator componentGenerator, IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(modelHelper);

        _componentGenerator = componentGenerator;
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
    /// Additional attributes for the container element that wraps the items.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = AttributesPrefix)]
    public IDictionary<string, string?>? CheckboxesAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute of the generated elements.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// The prefix to use when generating IDs for the hint, error message and items.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="For"/> or <see cref="Name"/> is specified.
    /// </remarks>
    [HtmlAttributeName(IdPrefixAttributeName)]
    public string? IdPrefix { get; set; }

    /// <summary>
    /// Whether the <see cref="Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateEntry.Errors"/> for the <see cref="For"/> expression should be used
    /// to deduce an error message.
    /// </summary>
    [HtmlAttributeName(IgnoreModelStateErrorsAttributeName)]
    public bool? IgnoreModelStateErrors { get; set; }

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>input</c> elements.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="For"/> or <see cref="IdPrefix"/> is specified.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

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
        context.SetContextItem(new CheckboxesContext(Name, For));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        using (context.SetScopedContextItem(checkboxesContext))
        {
            _ = await output.GetChildContentAsync();
        }

        var idPrefix = ResolveIdPrefix();
        TryResolveName(out var name);

        var hintOptions = checkboxesContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = checkboxesContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        var fieldsetOptions = GetFieldsetOptions(checkboxesContext);

        var items = checkboxesContext.Items.ToList();
        var values = GetCheckedValues();

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new CheckboxesOptionsFormGroup
        {
            BeforeInputs = checkboxesContext.BeforeInputs is TemplateString beforeInputs ?
                new CheckboxesOptionsBeforeInputs
                {
                    Text = null,
                    Html = beforeInputs
                } :
                null,
            AfterInputs = checkboxesContext.AfterInputs is TemplateString afterInputs ?
                new CheckboxesOptionsAfterInputs
                {
                    Text = null,
                    Html = afterInputs
                } :
                null,
            Attributes = formGroupAttributes,
            Classes = formGroupClasses
        };

        var attributes = new AttributeCollection(CheckboxesAttributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateCheckboxesAsync(new CheckboxesOptions
        {
            IdPrefix = new TemplateString(idPrefix),
            Name = name is not null ? new TemplateString(name) : null,
            DescribedBy = DescribedBy,
            Fieldset = fieldsetOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Items = items,
            Values = values,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);
            var containerErrorContext = ViewContext!.HttpContext.GetContainerErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html, href: "#" + idPrefix);
        }
    }

    private string ResolveIdPrefix()
    {
        if (IdPrefix is not null)
        {
            return IdPrefix;
        }

        if (Name is null && For is null)
        {
            throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(
                IdPrefixAttributeName,
                NameAttributeName,
                ForAttributeName);
        }

        TryResolveName(out var resolvedName);
        Debug.Assert(resolvedName is not null);

        return TagBuilder.CreateSanitizedId(resolvedName!, Constants.IdAttributeDotReplacement);
    }

    private bool TryResolveName([NotNullWhen(true)] out string? name)
    {
        if (Name is null && For is null)
        {
            name = default;
            return false;
        }

        name = Name ?? _modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name);
        return true;
    }

    private FieldsetOptions? GetFieldsetOptions(CheckboxesContext checkboxesContext)
    {
        if (checkboxesContext.Fieldset is null)
        {
            return null;
        }

        return checkboxesContext.Fieldset.GetFieldsetOptions(_modelHelper);
    }

    private IReadOnlyCollection<string>? GetCheckedValues()
    {
        if (For is null)
        {
            return null;
        }

        var modelExpression = For;
        object model = modelExpression.Model;

        if (modelExpression.Metadata.IsEnumerableType)
        {
            var value = ViewContext!.ModelState.TryGetValue(modelExpression.Name, out var entry) ?
                entry.RawValue :
                model;

            var values = (value as IEnumerable)?.Cast<object>();
            return values?.Select(v => v?.ToString()).Where(v => v is not null).Cast<string>().ToList();
        }

        var singleValue = model?.ToString();
        return singleValue is not null ? new[] { singleValue } : null;
    }
}
