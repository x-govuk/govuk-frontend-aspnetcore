using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
public class RadiosItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-radios-item";

    private const string CheckedAttributeName = "checked";
    private const string DisabledAttributeName = "disabled";
    private const string IdAttributeName = "id";
    private const string InputAttributesPrefix = "input-";
    private const string LabelAttributesPrefix = "label-";
    private const string ValueAttributeName = "value";

    /// <summary>
    /// Creates a new <see cref="RadiosItemTagHelper"/>.
    /// </summary>
    public RadiosItemTagHelper()
    {
    }

    /// <summary>
    /// Whether the item should be checked.
    /// </summary>
    /// <remarks>
    /// If <c>null</c> and <see cref="RadiosTagHelper.For"/> is not <c>null</c> on the parent <see cref="RadiosTagHelper"/> then the value
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
        context.SetContextItem(new RadiosItemContext());
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

        var radiosContext = context.GetContextItem<RadiosContext>();
        var itemContext = context.GetContextItem<RadiosItemContext>();

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
            (radiosContext.For is not null ? DoesModelMatchItemValue() : null);

        var itemAttributes = new AttributeCollection(output.Attributes);

        var labelAttributes = new AttributeCollection(LabelAttributes);
        labelAttributes.Remove("class", out var labelClasses);

        var inputAttributes = new AttributeCollection(InputAttributes);

        radiosContext.AddItem(new RadiosOptionsItem
        {
            Text = null,
            Html = content.ToTemplateString(),
            Id = Id,
            Value = Value,
            Label = new LabelOptions
            {
                Classes = labelClasses,
                Attributes = labelAttributes
            },
            Hint = itemContext.Hint?.Options,
            Checked = resolvedChecked,
            Conditional = itemContext.Conditional?.Options,
            Disabled = Disabled,
            Attributes = inputAttributes,
            ItemAttributes = itemAttributes
        });

        output.SuppressOutput();

        bool DoesModelMatchItemValue()
        {
            Debug.Assert(radiosContext.For is not null);
            Debug.Assert(ViewContext is not null);

            var modelValue = radiosContext.For?.Model?.ToString();
            return modelValue == Value;
        }
    }
}
