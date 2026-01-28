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
    /// If <c>null</c> and <see cref="FormGroupTagHelperBase.AspFor"/> is not <c>null</c> on the parent <see cref="CheckboxesTagHelper"/> then the value
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
    /// Required unless <see cref="FormGroupTagHelperBase.AspFor"/> or <see cref="CheckboxesTagHelper.Name"/> is specified on the parent <see cref="CheckboxesTagHelper"/>.
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
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (Value is null)
        {
            throw ExceptionHelper.TheAttributeMustBeSpecified(ValueAttributeName);
        }

        var checkboxesContext = context.GetContextItem<CheckboxesContext>();

        if (Name is null && checkboxesContext.Name is null && checkboxesContext.AspFor is null)
        {
            throw new InvalidOperationException($"The '{NameAttributeName}' attribute must be specified on each item when not specified on the parent <{CheckboxesTagHelper.TagName}>.");
        }

        var itemContext = new CheckboxesItemContext();

        TagHelperContent content;
        using (context.SetScopedContextItem(itemContext))
        {
            content = await output.GetChildContentAsync();
        }

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var resolvedChecked = Checked ??
            (checkboxesContext.AspFor is not null ? (bool?)DoesModelMatchItemValue() : null) ??
            false;

        var itemAttributes = output.Attributes.ToAttributeDictionary().ToAttributeCollection();

        var labelAttributes = LabelAttributes.ToAttributeDictionary().ToAttributeCollection();
        labelAttributes.Remove("class", out var labelClasses);

        var inputAttributes = InputAttributes.ToAttributeDictionary().ToAttributeCollection();

        HintOptions? hintOptions = null;
        if (itemContext.Hint is not null)
        {
            var hintAttrs = itemContext.Hint.Value.Attributes.ToAttributeCollection();
            hintAttrs.Remove("class", out var hintClasses);

            hintOptions = new HintOptions
            {
                Text = null,
                Html = itemContext.Hint.Value.Content.ToTemplateString(),
                Classes = hintClasses,
                Attributes = hintAttrs
            };
        }

        CheckboxesOptionsItemConditional? conditionalOptions = null;
        if (itemContext.Conditional is not null)
        {
            conditionalOptions = new CheckboxesOptionsItemConditional
            {
                Html = itemContext.Conditional.Value.Content.ToTemplateString()
            };
        }

        checkboxesContext.AddItem(new CheckboxesOptionsItem
        {
            Text = null,
            Html = content.ToTemplateString(),
            Id = Id is not null ? new TemplateString(Id) : null,
            Name = Name is not null ? new TemplateString(Name) : null,
            Value = Value is not null ? new TemplateString(Value) : null,
            Label = new LabelOptions
            {
                Classes = labelClasses,
                Attributes = labelAttributes
            },
            Hint = hintOptions,
            Checked = resolvedChecked,
            Conditional = conditionalOptions,
            Behaviour = Behavior == CheckboxesItemBehavior.Exclusive ? "exclusive" : null,
            Disabled = Disabled ?? false,
            Attributes = itemAttributes.MergeWith(inputAttributes)
        });

        output.SuppressOutput();

        bool DoesModelMatchItemValue()
        {
            Debug.Assert(checkboxesContext.AspFor is not null);
            Debug.Assert(ViewContext is not null);

            var modelExpression = checkboxesContext.AspFor!;
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
