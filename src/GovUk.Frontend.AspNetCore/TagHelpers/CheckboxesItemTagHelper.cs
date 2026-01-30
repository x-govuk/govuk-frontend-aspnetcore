using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
public class CheckboxesItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-item";

    private const string CheckedAttributeName = "checked";
    private const string DisabledAttributeName = "disabled";
    private const string IdAttributeName = "id";
    private const string InputAttributesPrefix = "input-";
    private const string LabelAttributesPrefix = "label-";
    private const string NameAttributeName = "name";
    private const string ValueAttributeName = "value";

    /// <summary>
    /// Creates a new <see cref="CheckboxesItemTagHelper"/>.
    /// </summary>
    public CheckboxesItemTagHelper()
    {
    }

    /// <summary>
    /// The behavior when the item is checked.
    /// If set to <see cref="CheckboxesItemBehavior.Exclusive"/> implements a 'None of these' type behaviour via JavaScript when checkboxes are clicked.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="CheckboxesItemBehavior.Default"/>.
    /// </remarks>
    public CheckboxesItemBehavior? Behavior { get; set; }

    /// <summary>
    /// Whether the item should be checked.
    /// </summary>
    /// <remarks>
    /// If <c>null</c> and <see cref="CheckboxesTagHelper.For"/> is not <c>null</c> on the parent <see cref="CheckboxesTagHelper"/> then the value
    /// will be computed by comparing the specified model expression with <see cref="Value"/>.
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(CheckedAttributeName)]
    public bool? Checked { get; set; }

    /// <summary>
    /// Whether the <c>disabled</c> attribute should be added to the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(DisabledAttributeName)]
    public bool? Disabled { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// If not specified then a value is generated from the <c>name</c> attribute.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = InputAttributesPrefix)]
    public IDictionary<string, string?> InputAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// Additional attributes to add to the generated <c>label</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LabelAttributesPrefix)]
    public IDictionary<string, string?> LabelAttributes { get; set; } = new Dictionary<string, string?>();

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// Required unless <see cref="CheckboxesTagHelper.For"/> or <see cref="CheckboxesTagHelper.Name"/> is specified on the parent <see cref="CheckboxesTagHelper"/>.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// The <c>value</c> attribute for the item.
    /// </summary>
    [HtmlAttributeName(ValueAttributeName)]
    public string? Value { get; set; }

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
        context.SetContextItem(new CheckboxesItemContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (Value is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(ValueAttributeName);
        }

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();
        var itemContext = context.GetContextItem<CheckboxesItemContext>();

        if (Name is null && checkboxesContext.Name is null && checkboxesContext.For is null)
        {
            throw new InvalidOperationException($"The '{NameAttributeName}' attribute must be specified on each item when not specified on the parent <{CheckboxesTagHelper.TagName}>.");
        }

        TagHelperContent content;
        context.SetContextItem(itemContext);
        content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var resolvedChecked = Checked ??
            (checkboxesContext.For is not null ? DoesModelMatchItemValue() : null);

        var itemAttributes = new AttributeCollection(output.Attributes);

        var labelAttributes = new AttributeCollection(LabelAttributes);
        labelAttributes.Remove("class", out var labelClasses);

        var inputAttributes = new AttributeCollection(InputAttributes);

        checkboxesContext.AddItem(new CheckboxesOptionsItem
        {
            Text = null,
            Html = content.ToTemplateString(),
            Id = Id,
            Name = Name,
            Value = Value,
            Label = new LabelOptions
            {
                Classes = labelClasses,
                Attributes = labelAttributes
            },
            Hint = itemContext.Hint?.Options,
            Checked = resolvedChecked,
            Conditional = itemContext.Conditional?.Options,
            Behaviour = Behavior is CheckboxesItemBehavior.Exclusive ? "exclusive" : null,
            Disabled = Disabled,
            Attributes = inputAttributes,
            ItemAttributes = itemAttributes
        });

        output.SuppressOutput();

        bool DoesModelMatchItemValue()
        {
            Debug.Assert(checkboxesContext.For is not null);
            Debug.Assert(ViewContext is not null);

            var modelExpression = checkboxesContext.For!;
            object model = modelExpression.Model;

            if (modelExpression.Metadata.IsEnumerableType)
            {
                var value = ViewContext!.ModelState.TryGetValue(modelExpression.Name, out var entry) ?
                    entry.RawValue :
                    model;

                var values = (value as IEnumerable)?.Cast<object>();
                return values?.Any(v => v?.ToString() == Value) is true;
            }

            return model?.ToString() == Value;
        }
    }
}
