using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A radios component.
/// </summary>
public sealed class Radios : FormField
{
    internal const string Selector = ".govuk-radios";

    private static readonly Lookup.Subject _itemSubject = new("radio", "radios", "labels");

    internal Radios(IElement radios, IElement formGroup, IElement? fieldset, IElement container)
        : base(formGroup, GetLegend(fieldset), container)
    {
        RadiosElement = radios;
        Items = ChoiceItem.GetInputs(radios, "govuk-radios").Select(i => new RadiosItem(i.Input, i.Item)).ToArray();
    }

    /// <summary>
    /// Gets the <c>govuk-radios</c> element that contains the items.
    /// </summary>
    public IElement RadiosElement { get; }

    /// <summary>
    /// Gets the radios, in order, leaving out any dividers.
    /// </summary>
    public IReadOnlyList<RadiosItem> Items { get; }

    /// <summary>
    /// Gets the radio that's checked, or <see langword="null"/> if none is.
    /// </summary>
    public RadiosItem? CheckedItem => Items.LastOrDefault(i => i.IsChecked);

    /// <summary>
    /// Gets the value of the <see cref="CheckedItem"/>, or <see langword="null"/> if none is checked.
    /// </summary>
    public string? Value => CheckedItem?.Value;

    /// <summary>
    /// Gets the radio labelled <paramref name="label"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such radio, or there's more than one.</exception>
    public RadiosItem GetItemByLabel(string label) => ChoiceItem.Find(Element, Items, label, _itemSubject, "a radio", required: true)!;

    /// <summary>
    /// Gets the radio labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such radio.</exception>
    public RadiosItem? FindItemByLabel(string label) =>
        ChoiceItem.Find(Element, Items, label, _itemSubject, "a radio", required: false);

    /// <inheritdoc/>
    public override string? Name => GetSharedName(Items.Select(i => i.Name));

    /// <inheritdoc/>
    public override IReadOnlyList<string> Names => GetNames(Name, Items.Select(i => i.Name));

    internal override string Kind => "radios";
}

/// <summary>
/// A checkboxes component.
/// </summary>
public sealed class Checkboxes : FormField
{
    internal const string Selector = ".govuk-checkboxes";

    private static readonly Lookup.Subject _itemSubject = new("checkbox", "checkboxes", "labels");

    internal Checkboxes(IElement checkboxes, IElement formGroup, IElement? fieldset, IElement container)
        : base(formGroup, GetLegend(fieldset), container)
    {
        CheckboxesElement = checkboxes;
        Items = ChoiceItem.GetInputs(checkboxes, "govuk-checkboxes").Select(i => new CheckboxesItem(i.Input, i.Item)).ToArray();
    }

    /// <summary>
    /// Gets the <c>govuk-checkboxes</c> element that contains the items.
    /// </summary>
    public IElement CheckboxesElement { get; }

    /// <summary>
    /// Gets the checkboxes, in order, leaving out any dividers.
    /// </summary>
    public IReadOnlyList<CheckboxesItem> Items { get; }

    /// <summary>
    /// Gets the checkboxes that are checked, in order.
    /// </summary>
    public IReadOnlyList<CheckboxesItem> CheckedItems => Items.Where(i => i.IsChecked).ToArray();

    /// <summary>
    /// Gets the values of the <see cref="CheckedItems"/>, in order.
    /// </summary>
    public IReadOnlyList<string> Values => CheckedItems.Select(i => i.Value).ToArray();

    /// <summary>
    /// Gets the checkbox labelled <paramref name="label"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such checkbox, or there's more than one.</exception>
    public CheckboxesItem GetItemByLabel(string label) =>
        ChoiceItem.Find(Element, Items, label, _itemSubject, "a checkbox", required: true)!;

    /// <summary>
    /// Gets the checkbox labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such checkbox.</exception>
    public CheckboxesItem? FindItemByLabel(string label) =>
        ChoiceItem.Find(Element, Items, label, _itemSubject, "a checkbox", required: false);

    /// <inheritdoc/>
    public override string? Name => GetSharedName(Items.Select(i => i.Name));

    /// <inheritdoc/>
    public override IReadOnlyList<string> Names => GetNames(Name, Items.Select(i => i.Name));

    internal override string Kind => "checkboxes";
}

/// <summary>
/// A radio in <see cref="Radios"/> or a checkbox in <see cref="Checkboxes"/>.
/// </summary>
public abstract class ChoiceItem
{
    private protected ChoiceItem(IHtmlInputElement input, IElement item, string componentClass)
    {
        Input = input;
        Element = item;

        var label = item.Children.FirstOrDefault(c => c.LocalName == "label");
        Label = TextExtraction.GetText(label);
        Hint = TextExtraction.GetTextOrNull(item.Children.FirstOrDefault(c => c.ClassList.Contains($"{componentClass}__hint")));

        // The conditional reveal follows the item, and the input points to it with data-aria-controls.
        var conditionalId = input.GetAttribute("data-aria-controls");
        ConditionalElement = conditionalId is null
            ? null
            : item.ParentElement?.Children.FirstOrDefault(c => c.Id == conditionalId);
    }

    /// <summary>
    /// Gets the item's element, which contains the input, its label and its hint.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c> element.
    /// </summary>
    public IHtmlInputElement Input { get; }

    /// <summary>
    /// Gets the item's label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the text of the item's hint, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Hint { get; }

    /// <summary>
    /// Gets the input's <c>name</c>.
    /// </summary>
    public string? Name => Input.GetAttribute("name");

    /// <summary>
    /// Gets the input's <c>value</c>.
    /// </summary>
    public string Value => Input.Value;

    /// <summary>
    /// Gets whether the input is checked.
    /// </summary>
    public bool IsChecked => Input.IsChecked;

    /// <summary>
    /// Gets whether the input is disabled.
    /// </summary>
    public bool IsDisabled => Input.IsDisabled;

    /// <summary>
    /// Gets the element that's revealed when the input is checked, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <remarks>
    /// Fields inside it can be found by calling the lookup members on this element, e.g.
    /// <c>item.ConditionalElement.GetTextInputByLabel("Email address")</c>.
    /// </remarks>
    public IElement? ConditionalElement { get; }

    /// <inheritdoc/>
    public override string ToString() => $"{(IsChecked ? "[x]" : "[ ]")} {Label}";

    internal static IEnumerable<(IHtmlInputElement Input, IElement Item)> GetInputs(IElement root, string componentClass) =>
        root.Children
            .Where(c => c.ClassList.Contains($"{componentClass}__item"))
            .Select(item => (Input: item.Children.OfType<IHtmlInputElement>().FirstOrDefault(), Item: item))
            .Where(i => i.Input is not null)
            .Select(i => (i.Input!, i.Item));

    internal static T? Find<T>(IElement scope, IReadOnlyList<T> items, string label, Lookup.Subject subject, string expected, bool required)
        where T : ChoiceItem
    {
        ArgumentNullException.ThrowIfNull(label);

        return Lookup.Single(
            scope,
            items,
            i => i.Label == label,
            i => i.Label,
            subject,
            $"{expected} labelled {LookupFailure.Quote(label)}",
            required);
    }
}

/// <summary>
/// A radio in <see cref="Radios"/>.
/// </summary>
public sealed class RadiosItem : ChoiceItem
{
    internal RadiosItem(IHtmlInputElement input, IElement item)
        : base(input, item, "govuk-radios")
    {
    }
}

/// <summary>
/// A checkbox in <see cref="Checkboxes"/>.
/// </summary>
public sealed class CheckboxesItem : ChoiceItem
{
    internal CheckboxesItem(IHtmlInputElement input, IElement item)
        : base(input, item, "govuk-checkboxes")
    {
    }

    /// <summary>
    /// Gets whether checking this checkbox unchecks the others, as for a <c>None</c> option.
    /// </summary>
    public bool IsExclusive => Input.GetAttribute("data-behaviour") == "exclusive";
}
