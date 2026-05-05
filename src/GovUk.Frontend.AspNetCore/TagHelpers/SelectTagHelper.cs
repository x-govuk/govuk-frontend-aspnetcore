using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AttributeCollection = GovUk.Frontend.AspNetCore.ComponentGeneration.AttributeCollection;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS select component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    SelectItemTagHelper.TagName,
    SelectLabelTagHelper.TagName,
    SelectHintTagHelper.TagName,
    SelectErrorMessageTagHelper.TagName,
    SelectBeforeInputTagHelper.TagName,
    SelectAfterInputTagHelper.TagName
#if SHORT_TAG_NAMES
    ,
    FormGroupLabelTagHelperBase.ShortTagName,
    FormGroupHintTagHelperBase.ShortTagName,
    FormGroupErrorMessageTagHelperBase.ShortTagName,
    ShortTagNames.BeforeInput,
    ShortTagNames.AfterInput
#endif
)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class SelectTagHelper : TagHelper
{
    internal const string TagName = "govuk-select";

    private const string AttributesPrefix = "select-";
    private const string DescribedByAttributeName = "described-by";
    private const string DisabledAttributeName = "disabled";
    private const string ForAttributeName = "for";
    private const string IdAttributeName = "id";
    private const string IgnoreModelStateErrorsAttributeName = "ignore-modelstate-errors";
    private const string LabelClassAttributeName = "label-class";
    private const string NameAttributeName = "name";

    private readonly IComponentGenerator _componentGenerator;
    private readonly IModelHelper _modelHelper;

    /// <summary>
    /// Creates a new <see cref="SelectTagHelper"/>.
    /// </summary>
    public SelectTagHelper(IComponentGenerator componentGenerator)
        : this(componentGenerator, new DefaultModelHelper())
    {
    }

    internal SelectTagHelper(IComponentGenerator componentGenerator, IModelHelper modelHelper)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);
        ArgumentNullException.ThrowIfNull(modelHelper);

        _componentGenerator = componentGenerator;
        _modelHelper = modelHelper;
    }

    /// <summary>
    /// One or more element IDs to add to the <c>aria-describedby</c> attribute of the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DescribedByAttributeName)]
    public string? DescribedBy { get; set; }

    /// <summary>
    /// Whether the <c>disabled</c> attribute should be added to the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DisabledAttributeName)]
    public bool? Disabled { get; set; }

    /// <summary>
    /// An expression to be evaluated against the current model.
    /// </summary>
    [HtmlAttributeName(ForAttributeName)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the generated <c>select</c> element.
    /// </summary>
    /// <remarks>
    /// If not specified then a value is generated from the <c>name</c> attribute.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Whether the <see cref="ModelStateEntry.Errors"/> for the <see cref="For"/> expression should be used
    /// to deduce an error message.
    /// </summary>
    /// <remarks>
    /// <para>When there are multiple errors in the <see cref="ModelErrorCollection"/> the first is used.</para>
    /// </remarks>
    [HtmlAttributeName(IgnoreModelStateErrorsAttributeName)]
    public bool? IgnoreModelStateErrors { get; set; }

    /// <summary>
    /// Additional classes for the generated <c>label</c> element.
    /// </summary>
    [HtmlAttributeName(LabelClassAttributeName)]
    public string? LabelClass { get; set; }

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>select</c> element.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="For"/> is specified.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>select</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = AttributesPrefix)]
    public IDictionary<string, string?> SelectAttributes { get; set; } = new Dictionary<string, string?>();

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
        context.SetContextItem(new SelectContext(For));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var selectContext = context.GetContextItem<SelectContext>();

        context.SetContextItem(typeof(FormGroupContext3), selectContext);
        _ = await output.GetChildContentAsync();

        var name = ResolveName();
        var id = ResolveId(name);
        var labelOptions = selectContext.GetLabelOptions(For, ViewContext!, _modelHelper, id, ForAttributeName);
        var hintOptions = selectContext.GetHintOptions(For, _modelHelper);
        var errorMessageOptions = selectContext.GetErrorMessageOptions(For, ViewContext!, _modelHelper, IgnoreModelStateErrors);

        if (LabelClass is not null)
        {
            labelOptions.Classes = labelOptions.Classes.AppendCssClasses(LabelClass);
        }

        var formGroupAttributes = new AttributeCollection(output.Attributes);
        formGroupAttributes.Remove("class", out var formGroupClasses);
        var formGroupOptions = new SelectFormGroupOptions
        {
            Attributes = formGroupAttributes,
            Classes = formGroupClasses,
            BeforeInput = selectContext.BeforeInput is TemplateString beforeInput ?
                new SelectOptionsBeforeInput
                {
                    Html = beforeInput
                } :
                null,
            AfterInput = selectContext.AfterInput is TemplateString afterInput ?
                new SelectOptionsAfterInput
                {
                    Html = afterInput
                } :
                null
        };

        var attributes = new AttributeCollection(SelectAttributes);
        attributes.Remove("class", out var classes);

        if (Disabled == true)
        {
            attributes.AddBoolean("disabled");
        }

        var component = await _componentGenerator.GenerateSelectInputAsync(new SelectOptions
        {
            Id = id,
            Name = name,
            DescribedBy = DescribedBy,
            Label = labelOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Classes = classes,
            Items = selectContext.Items,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);
            var containerErrorContext = ViewContext!.HttpContext.GetPageErrorContext();
            containerErrorContext.AddError(errorMessageOptions.Html, href: "#" + id);
        }
    }

    private string ResolveId(string name) =>
        Id ?? TagBuilder.CreateSanitizedId(name, Constants.IdAttributeDotReplacement);

    private string ResolveName()
    {
        return Name is null && For is null
            ? throw ExceptionHelper.AtLeastOneOfAttributesMustBeProvided(
                NameAttributeName,
                ForAttributeName)
            : Name ?? _modelHelper.GetFullHtmlFieldName(ViewContext!, For!.Name);
    }
}
