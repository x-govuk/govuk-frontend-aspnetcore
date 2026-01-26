using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
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
[RestrictChildren(SelectItemTagHelper.TagName, LabelTagName, HintTagName, ErrorMessageTagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class SelectTagHelper : TagHelper
{
    internal const string ErrorMessageTagName = "govuk-select-error-message";
    internal const string HintTagName = "govuk-select-hint";
    internal const string LabelTagName = "govuk-select-label";
    internal const string TagName = "govuk-select";

    private const string AspForAttributeName = "asp-for";
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

        using (context.SetScopedContextItem(selectContext))
        using (context.SetScopedContextItem(typeof(FormGroupContext3), selectContext))
        {
            _ = await output.GetChildContentAsync();
        }

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
            Classes = formGroupClasses
        };

        var attributes = new AttributeCollection(SelectAttributes);
        attributes.Remove("class", out var classes);

        if (Disabled == true)
        {
            attributes.AddBoolean("disabled");
        }

        var items = selectContext.Items.Select(item => new SelectOptionsItem
        {
            Value = item.Value != null ? new HtmlString(item.Value) : null,
            Text = item.Content?.ToHtmlString(),
            Selected = item.Selected,
            Disabled = item.Disabled,
            Attributes = item.Attributes != null ? new AttributeCollection(item.Attributes) : null
        }).ToList();

        var component = await _componentGenerator.GenerateSelectAsync(new SelectOptions
        {
            Id = new HtmlString(id),
            Name = new HtmlString(name),
            DescribedBy = DescribedBy != null ? new HtmlString(DescribedBy) : null,
            Label = labelOptions,
            Hint = hintOptions,
            ErrorMessage = errorMessageOptions,
            FormGroup = formGroupOptions,
            Classes = classes,
            Items = items,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);

        if (errorMessageOptions is not null)
        {
            Debug.Assert(errorMessageOptions.Html is not null);
            var containerErrorContext = ViewContext!.HttpContext.GetContainerErrorContext();
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
