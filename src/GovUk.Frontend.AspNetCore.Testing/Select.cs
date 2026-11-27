using System.Diagnostics.CodeAnalysis;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A select component.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Named after the GOV.UK component; the package needs C# 14, so it's only used from C#.")]
public sealed class Select : FormField
{
    private static readonly Lookup.Subject _itemSubject = new("select option", "options", "text");

    internal Select(IHtmlSelectElement select, IElement formGroup)
        : base(formGroup, GetLabel(formGroup, select), formGroup)
    {
        SelectElement = select;
        Items = select.QuerySelectorAll("option").OfType<IHtmlOptionElement>().Select(o => new SelectItem(o)).ToArray();
    }

    /// <summary>
    /// Gets the <c>&lt;select&gt;</c> element.
    /// </summary>
    public IHtmlSelectElement SelectElement { get; }

    /// <summary>
    /// Gets the select's <c>name</c>.
    /// </summary>
    public override string? Name => SelectElement.GetAttribute("name");

    /// <summary>
    /// Gets the options, in order.
    /// </summary>
    public IReadOnlyList<SelectItem> Items { get; }

    /// <summary>
    /// Gets the option a browser shows as selected, or <see langword="null"/> if there are no options that can be.
    /// </summary>
    /// <remarks>
    /// When no option is marked as <c>selected</c>, this is the first one that isn't disabled.
    /// </remarks>
    public SelectItem? SelectedItem => Items.LastOrDefault(i => i.IsSelected);

    /// <summary>
    /// Gets the value of the <see cref="SelectedItem"/>, or <see langword="null"/> if there are no options.
    /// </summary>
    public string? Value => SelectedItem?.Value;

    /// <summary>
    /// Gets the option whose text is <paramref name="text"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such option, or there's more than one.</exception>
    public SelectItem GetItemByText(string text) => FindItemByText(text, required: true)!;

    /// <summary>
    /// Gets the option whose text is <paramref name="text"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such option.</exception>
    public SelectItem? FindItemByText(string text) => FindItemByText(text, required: false);

    private SelectItem? FindItemByText(string text, bool required)
    {
        ArgumentNullException.ThrowIfNull(text);

        return Lookup.Single(
            Element,
            Items,
            i => i.Text == text,
            i => i.Text,
            _itemSubject,
            $"an option with the text {LookupFailure.Quote(text)}",
            required);
    }

    internal override string Kind => "select";
}

/// <summary>
/// An option in a <see cref="Select"/>.
/// </summary>
public sealed class SelectItem
{
    internal SelectItem(IHtmlOptionElement option)
    {
        Element = option;
        Text = TextExtraction.GetText(option);
    }

    /// <summary>
    /// Gets the <c>&lt;option&gt;</c> element.
    /// </summary>
    public IHtmlOptionElement Element { get; }

    /// <summary>
    /// Gets the option's text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the option's value: its <c>value</c> attribute or, if it doesn't have one, its text.
    /// </summary>
    public string Value => Element.Value;

    /// <summary>
    /// Gets whether the option is selected, as a browser would show it.
    /// </summary>
    /// <remarks>
    /// When no option is marked as <c>selected</c>, the first one that isn't disabled is.
    /// </remarks>
    public bool IsSelected => Element.IsSelected;

    /// <summary>
    /// Gets whether the option is disabled.
    /// </summary>
    public bool IsDisabled => Element.IsDisabled;

    /// <inheritdoc/>
    public override string ToString() => Text;
}
