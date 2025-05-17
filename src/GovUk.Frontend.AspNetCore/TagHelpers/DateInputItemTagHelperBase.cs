using GovUk.Frontend.AspNetCore.Components;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a GDS date input component.
/// </summary>
public abstract class DateInputItemTagHelperBase : TagHelper
{
    private const string AutoCompleteAttributeName = "autocomplete";
    private const string IdAttributeName = "id";
    private const string InputModeAttributeName = "inputmode";
    private const string NameAttributeName = "name";
    private const string PatternAttributeName = "pattern";
    private const string ValueAttributeName = "value";

    private readonly DateInputItemType _itemType;
    private readonly string _labelTagName;
    private string? _value;
    private bool _valueSpecified = false;

    /// <summary>
    /// Creates a <see cref="DateInputItemTagHelperBase"/>.
    /// </summary>
    private protected DateInputItemTagHelperBase(DateInputItemType itemType, string labelTagName)
    {
        ArgumentNullException.ThrowIfNull(labelTagName);
        _itemType = itemType;
        _labelTagName = labelTagName;
    }

    /// <summary>
    /// The <c>autocomplete</c> attribute for the generated <c>input</c> element.
    /// </summary>
    [HtmlAttributeName(AutoCompleteAttributeName)]
    public string? AutoComplete { get; set; }

    /// <summary>
    /// The <c>id</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// By default the value will be generated from the parent's <see cref="DateInputTagHelper.Id"/>.
    /// </remarks>
    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    /// <summary>
    /// The <c>inputmode</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// The default is <c>numeric</c>.
    /// </remarks>
    [HtmlAttributeName(InputModeAttributeName)]
    public string? InputMode { get; set; } = ComponentGenerator.DateInputDefaultInputMode;

    /// <summary>
    /// The <c>name</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// By default the value will be generated from the parent's <see cref="FormGroupTagHelperBase.AspFor"/> and/or <see cref="DateInputTagHelper.NamePrefix"/>.
    /// </remarks>
    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    /// <summary>
    /// The <c>pattern</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// The default is <c>[0-9]*</c>.
    /// </remarks>
    [HtmlAttributeName(PatternAttributeName)]
    public string? Pattern { get; set; }

    /// <summary>
    /// The <c>value</c> attribute for the generated <c>input</c> element.
    /// </summary>
    /// <remarks>
    /// This cannot be specified if the <see cref="DateInputTagHelper.Value"/> property on the parent is also specified.
    /// </remarks>
    [HtmlAttributeName(ValueAttributeName)]
    public string? Value
    {
        get => _value;
        set
        {
            _value = value;
            _valueSpecified = true;
        }
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var dateInputContext = context.GetContextItem<DateInputContext>();
        var dateInputItemContext = new DateInputItemContext(output.TagName, _labelTagName);

        using (context.SetScopedContextItem(dateInputItemContext))
        {
            await output.GetChildContentAsync();
        }

        var attributes = new AttributeCollection(output.Attributes);

        var itemContext = new DateInputContextItem()
        {
            TagName = output.TagName,
            Attributes = attributes,
            AutoComplete = AutoComplete,
            Id = Id,
            InputMode = InputMode,
            LabelHtml = dateInputItemContext.Label?.Html,
            LabelAttributes = dateInputItemContext.Label?.Attributes,
            Name = Name,
            Pattern = Pattern,
            Value = _value,
            ValueSpecified = _valueSpecified
        };

        dateInputContext.SetItem(_itemType, itemContext);

        output.SuppressOutput();
    }
}
