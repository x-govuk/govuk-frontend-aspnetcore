using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A tabs component.
/// </summary>
public sealed class Tabs
{
    internal const string Selector = ".govuk-tabs";

    internal static readonly Lookup.Subject Subject = new("tabs component", "tabs components");

    private static readonly Lookup.Subject _tabSubject = new("tab", "tabs", "labels");

    internal Tabs(IElement element)
    {
        Element = element;
        Title = TextExtraction.GetTextOrNull(Lookup.Child(element, "govuk-tabs__title"));

        var panels = Lookup.Children(element, "govuk-tabs__panel", e => e);
        Items = Lookup.Children(Lookup.Child(element, "govuk-tabs__list"), "govuk-tabs__list-item", e => new Tab(e, panels));
    }

    /// <summary>
    /// Gets the component's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the title shown above the tabs when they're shown as a list of links, or <see langword="null"/> if there
    /// isn't one.
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// Gets the tabs, in order.
    /// </summary>
    public IReadOnlyList<Tab> Items { get; }

    /// <summary>
    /// Gets the selected tab, or <see langword="null"/> if none is marked as selected.
    /// </summary>
    public Tab? Selected => Items.FirstOrDefault(i => i.IsSelected);

    /// <summary>
    /// Gets the tab labelled <paramref name="label"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such tab, or there's more than one.</exception>
    public Tab GetTabByLabel(string label) => FindTabByLabel(label, required: true)!;

    /// <summary>
    /// Gets the tab labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such tab.</exception>
    public Tab? FindTabByLabel(string label) => FindTabByLabel(label, required: false);

    private Tab? FindTabByLabel(string label, bool required)
    {
        ArgumentNullException.ThrowIfNull(label);

        return Lookup.Single(
            Element,
            Items,
            t => t.Label == label,
            t => t.Label,
            _tabSubject,
            $"a tab labelled {LookupFailure.Quote(label)}",
            required);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Tabs: {LookupFailure.QuoteAll(Items.Select(i => i.Label))}";
}

/// <summary>
/// A tab in a <see cref="Tabs"/> component, with its panel.
/// </summary>
public sealed class Tab
{
    internal Tab(IElement element, IReadOnlyList<IElement> panels)
    {
        Element = element;

        var link = Lookup.Child(element, "govuk-tabs__tab");
        Label = TextExtraction.GetText(link);
        var href = link?.GetAttribute("href");
        Id = href is ['#', .. var id] ? id : null;

        PanelElement = Id is null ? null : panels.FirstOrDefault(p => p.Id == Id);
        TextLines = PanelElement is null ? [] : TextExtraction.GetLines(PanelElement);
        Text = string.Join(" ", TextLines);
        IsSelected = element.ClassList.Contains("govuk-tabs__list-item--selected");
    }

    /// <summary>
    /// Gets the tab's <c>&lt;li&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the tab's label, e.g. <c>Past day</c>.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets the <c>id</c> of the tab's panel, from the tab's link, or <see langword="null"/> if it doesn't link to one.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// Gets the tab's panel element, or <see langword="null"/> if there isn't one.
    /// </summary>
    public IElement? PanelElement { get; }

    /// <summary>
    /// Gets the panel's content as a single line of text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the lines of the panel's content, as split by line breaks and block-level elements.
    /// </summary>
    public IReadOnlyList<string> TextLines { get; }

    /// <summary>
    /// Gets whether the tab is marked as selected.
    /// </summary>
    public bool IsSelected { get; }

    /// <inheritdoc/>
    public override string ToString() => Label;
}
