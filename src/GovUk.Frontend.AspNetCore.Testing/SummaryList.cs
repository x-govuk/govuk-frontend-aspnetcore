using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A summary list component, on its own or inside a <see cref="SummaryCard"/>.
/// </summary>
public sealed class SummaryList
{
    internal const string Selector = "dl.govuk-summary-list";

    internal static readonly Lookup.Subject RowSubject = new("summary list row", "rows", "keys");

    internal SummaryList(IElement element)
    {
        Element = element;
        Rows = Lookup.Children(element, "govuk-summary-list__row", e => new SummaryListRow(e));
    }

    /// <summary>
    /// Gets the summary list's <c>&lt;dl&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the rows, in order.
    /// </summary>
    public IReadOnlyList<SummaryListRow> Rows { get; }

    /// <summary>
    /// Gets the summary card the list is in, or <see langword="null"/> if it isn't in one.
    /// </summary>
    public SummaryCard? Card =>
        Element.ParentElement is { } content && content.ClassList.Contains("govuk-summary-card__content") &&
        content.ParentElement is { } card && card.ClassList.Contains("govuk-summary-card")
            ? new SummaryCard(card)
            : null;

    /// <summary>
    /// Gets the row with the key <paramref name="key"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">
    /// There is no row with the key <paramref name="key"/>, or there's more than one; use <see cref="GetRowsByKey"/> for
    /// lists with duplicate keys.
    /// </exception>
    public SummaryListRow GetRowByKey(string key) => FindRowByKey(key, required: true)!;

    /// <summary>
    /// Gets the row with the key <paramref name="key"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one row with the key <paramref name="key"/>.</exception>
    public SummaryListRow? FindRowByKey(string key) => FindRowByKey(key, required: false);

    /// <summary>
    /// Gets every row with the key <paramref name="key"/>.
    /// </summary>
    public IReadOnlyList<SummaryListRow> GetRowsByKey(string key) => Rows.Where(r => r.Key == key).ToArray();

    private SummaryListRow? FindRowByKey(string key, bool required)
    {
        ArgumentNullException.ThrowIfNull(key);

        return Lookup.Single(
            Element,
            Rows,
            r => r.Key == key,
            r => r.Key,
            RowSubject,
            $"a row with the key {LookupFailure.Quote(key)}",
            required);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Summary list: {LookupFailure.QuoteAll(Rows.Select(r => r.Key))}";
}

/// <summary>
/// A row in a <see cref="SummaryList"/>.
/// </summary>
public sealed class SummaryListRow
{
    internal SummaryListRow(IElement element)
    {
        Element = element;

        Key = TextExtraction.GetText(Lookup.Child(element, "govuk-summary-list__key"));

        ValueElement = Lookup.Child(element, "govuk-summary-list__value");
        ValueLines = ValueElement is null ? [] : TextExtraction.GetLines(ValueElement);
        Value = string.Join(" ", ValueLines);

        Actions = Lookup.Child(element, "govuk-summary-list__actions")?.QuerySelectorAll("a")
            .Select(a => new SummaryListAction(a))
            .ToArray() ?? [];
    }

    /// <summary>
    /// Gets the row's element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the key, e.g. <c>Name</c>.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the value's element, the row's <c>&lt;dd class="govuk-summary-list__value"&gt;</c>, or
    /// <see langword="null"/> if it doesn't have one.
    /// </summary>
    /// <remarks>
    /// Use this to check what's in the value beyond its text, e.g. a link: <c>row.ValueElement?.QuerySelector("a")</c>.
    /// </remarks>
    public IElement? ValueElement { get; }

    /// <summary>
    /// Gets the value as a single line of text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the value's lines, as split by line breaks and block-level elements such as paragraphs and list items.
    /// Empty lines are left out.
    /// </summary>
    public IReadOnlyList<string> ValueLines { get; }

    /// <summary>
    /// Gets the row's action links.
    /// </summary>
    public IReadOnlyList<SummaryListAction> Actions { get; }

    /// <summary>
    /// Gets the action whose visible text is <c>Change</c>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <remarks>
    /// Use <see cref="GetActionByText"/> for an action with other text, such as a localized one.
    /// </remarks>
    public SummaryListAction? ChangeLink => FindActionByText("Change");

    /// <summary>
    /// Gets the action whose visible text is <paramref name="text"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such action, or there's more than one.</exception>
    public SummaryListAction GetActionByText(string text) => SummaryListAction.Find(Element, Actions, text, required: true)!;

    /// <summary>
    /// Gets the action whose visible text is <paramref name="text"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such action.</exception>
    public SummaryListAction? FindActionByText(string text) => SummaryListAction.Find(Element, Actions, text, required: false);

    /// <inheritdoc/>
    public override string ToString() => $"{Key}: {Value}";
}

/// <summary>
/// An action link in a <see cref="SummaryListRow"/> or a <see cref="SummaryCard"/>.
/// </summary>
public sealed class SummaryListAction
{
    private static readonly Lookup.Subject _subject = new("summary list action", "actions", "text");

    internal SummaryListAction(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element);
        VisuallyHiddenText = TextExtraction.GetVisuallyHiddenText(element);
        Href = element.GetAttribute("href");
    }

    /// <summary>
    /// Gets the link's <c>&lt;a&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the visible text, e.g. <c>Change</c>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the visually hidden text that describes the action to screen reader users, e.g. <c>name</c>, or
    /// <see langword="null"/> if there isn't any.
    /// </summary>
    /// <remarks>
    /// In a summary card, this includes the card's title in brackets, e.g. <c>name (Director 1)</c>.
    /// </remarks>
    public string? VisuallyHiddenText { get; }

    /// <summary>
    /// Gets the link's <c>href</c>.
    /// </summary>
    public string? Href { get; }

    internal static SummaryListAction? Find(IElement scope, IReadOnlyList<SummaryListAction> actions, string text, bool required)
    {
        ArgumentNullException.ThrowIfNull(text);

        return Lookup.Single(
            scope,
            actions,
            a => a.Text == text,
            a => a.Text,
            _subject,
            $"an action with the text {LookupFailure.Quote(text)}",
            required);
    }

    /// <inheritdoc/>
    public override string ToString() => VisuallyHiddenText is null ? Text : $"{Text} ({VisuallyHiddenText})";
}

/// <summary>
/// A summary card: a summary list with a title and actions of its own.
/// </summary>
public sealed class SummaryCard
{
    internal const string Selector = ".govuk-summary-card";

    internal static readonly Lookup.Subject Subject = new("summary card", "summary cards", "titles");

    internal SummaryCard(IElement element)
    {
        Element = element;

        var titleWrapper = Lookup.Child(element, "govuk-summary-card__title-wrapper");
        Title = TextExtraction.GetText(Lookup.Child(titleWrapper, "govuk-summary-card__title"));

        Actions = Lookup.Child(titleWrapper, "govuk-summary-card__actions")?.QuerySelectorAll("a")
            .Select(a => new SummaryListAction(a))
            .ToArray() ?? [];

        var list = Lookup.Child(element, "govuk-summary-card__content")?.Children
            .FirstOrDefault(c => c.ClassList.Contains("govuk-summary-list"));
        SummaryList = list is null ? null : new SummaryList(list);
    }

    /// <summary>
    /// Gets the card's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the card's title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the card's own action links, shown next to its title.
    /// </summary>
    public IReadOnlyList<SummaryListAction> Actions { get; }

    /// <summary>
    /// Gets the summary list inside the card, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public SummaryList? SummaryList { get; }

    /// <summary>
    /// Gets the card action whose visible text is <paramref name="text"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such action, or there's more than one.</exception>
    public SummaryListAction GetActionByText(string text) => SummaryListAction.Find(Element, Actions, text, required: true)!;

    /// <summary>
    /// Gets the card action whose visible text is <paramref name="text"/>, or <see langword="null"/> if there isn't
    /// one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such action.</exception>
    public SummaryListAction? FindActionByText(string text) => SummaryListAction.Find(Element, Actions, text, required: false);

    /// <summary>
    /// Gets the row with the key <paramref name="key"/> in the card's summary list.
    /// </summary>
    /// <exception cref="ComponentLookupException">
    /// There is no row with the key <paramref name="key"/>, or there's more than one.
    /// </exception>
    public SummaryListRow GetRowByKey(string key) =>
        SummaryList?.GetRowByKey(key) ?? throw new ComponentLookupException(new LookupFailure(
            "Summary list row not found.",
            $"a row with the key {LookupFailure.Quote(key)}",
            $"summary card {LookupFailure.Quote(Title)} has no summary list",
            LookupFailure.FormatHtml(Element)));

    /// <summary>
    /// Gets the row with the key <paramref name="key"/> in the card's summary list, or <see langword="null"/> if
    /// there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one row with the key <paramref name="key"/>.</exception>
    public SummaryListRow? FindRowByKey(string key) => SummaryList?.FindRowByKey(key);

    /// <inheritdoc/>
    public override string ToString() => $"Summary card: {Title}";
}
