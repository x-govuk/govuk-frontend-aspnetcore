using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
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
    FormGroupFieldsetLegendTagHelper.CheckboxesTagName,
    CheckboxesItemTagHelper.TagName,
    CheckboxesItemDividerTagHelper.TagName,
    CheckboxesHintTagHelper.TagName,
    CheckboxesErrorMessageTagHelper.TagName,
    CheckboxesBeforeInputsTagHelper.TagName,
    CheckboxesAfterInputsTagHelper.TagName
)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class CheckboxesTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes";

    private const string AttributesPrefix = "checkboxes-";
    private const string DescribedByAttributeName = "described-by";
    private const string FieldsetAttributeName = FormGroupFieldsetHelper.FieldsetAttributeName;
    private const string FieldsetAttributesPrefix = FormGroupFieldsetHelper.FieldsetAttributesPrefix;
    private const string ForAttributeName = "for";
    private const string IdPrefixAttributeName = "id-prefix";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string LegendAttributesPrefix = FormGroupFieldsetHelper.LegendAttributesPrefix;
    private const string LegendIsPageHeadingAttributeName = FormGroupFieldsetHelper.LegendIsPageHeadingAttributeName;
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
    /// Whether a <c>fieldset</c> should be generated to wrap the component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <c>fieldset</c> is generated automatically when a <c>govuk-checkboxes-fieldset</c> element or a
    /// <c>govuk-checkboxes-fieldset-legend</c> element is used, or when any <c>fieldset-*</c>, <c>legend-*</c>
    /// or <c>legend-is-page-heading</c> attribute is specified; this attribute is only required when a <c>fieldset</c>
    /// is wanted but none of those are used.
    /// </para>
    /// <para>The legend's content is deduced from the <see cref="For"/> expression's metadata.</para>
    /// </remarks>
    [HtmlAttributeName(FieldsetAttributeName)]
    public bool Fieldset { get; set; }

    /// <summary>
    /// Additional attributes for the generated <c>fieldset</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = FieldsetAttributesPrefix)]
    public IDictionary<string, string?>? FieldsetAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Additional attributes for the generated <c>fieldset</c>'s <c>legend</c> element.
    /// </summary>
    /// <remarks>
    /// These are combined with any attributes specified on a <c>govuk-checkboxes-fieldset-legend</c> element;
    /// where both specify the same attribute the one on the element wins, except for <c>class</c>, where the two
    /// values are combined.
    /// </remarks>
    [HtmlAttributeName(DictionaryAttributePrefix = LegendAttributesPrefix)]
    public IDictionary<string, string?>? LegendAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// Whether the generated <c>fieldset</c>'s <c>legend</c> also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// An <c>is-page-heading</c> attribute on a <c>govuk-checkboxes-fieldset-legend</c> element takes precedence over this.
    /// </remarks>
    [HtmlAttributeName(LegendIsPageHeadingAttributeName)]
    public bool? LegendIsPageHeading { get; set; }

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
        var checkboxesContext = new CheckboxesContext(Name, For);
        context.SetContextItem(checkboxesContext);
        context.SetContextItem<FormGroupContext3>(checkboxesContext);
        context.SetContextItem(checkboxesContext.ImplicitFieldset);
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        _ = await output.GetChildContentAsync();

        var idPrefix = ResolveIdPrefix();
        var name = ResolveName();

        var hintOptions = checkboxesContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = checkboxesContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        var fieldsetOptions = checkboxesContext.GetFieldsetOptions(
            _modelHelper,
            Fieldset,
            new AttributeCollection(FieldsetAttributes),
            new AttributeCollection(LegendAttributes),
            LegendIsPageHeading);

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new CheckboxesOptionsFormGroup
        {
            BeforeInputs = checkboxesContext.BeforeInputs is { } beforeInputs ?
                new CheckboxesOptionsBeforeInputs
                {
                    Text = null,
                    Html = beforeInputs
                } :
                null,
            AfterInputs = checkboxesContext.AfterInputs is { } afterInputs ?
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
            DescribedBy = DescribedBy,
            IdPrefix = idPrefix,
            Name = name,
            Fieldset = fieldsetOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Items = checkboxesContext.Items,
            Values = null,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            // The message may be markup written in the view or text from ModelState.
            var errorContent = errorMessageOptions.Html ?? new TemplateString(errorMessageOptions.Text);
            var containerErrorContext = ViewContext!.HttpContext.GetPageErrorContext();
            containerErrorContext.AddError(errorContent, href: "#" + idPrefix);
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

        var resolvedName = ResolveName();
        Debug.Assert(resolvedName is not null);

        return TagBuilder.CreateSanitizedId(resolvedName, Constants.IdAttributeDotReplacement);
    }

    private string? ResolveName() =>
        Name ?? (For is not null ? _modelHelper.GetFullHtmlFieldName(ViewContext!, For.Name) : null);
}
