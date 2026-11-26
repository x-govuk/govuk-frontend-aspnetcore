using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A pagination component.
/// </summary>
public sealed class Pagination
{
    internal const string Selector = "nav.govuk-pagination";

    internal Pagination(IElement element)
    {
        Element = element;

        var previous = Lookup.Child(element, "govuk-pagination__prev")?.QuerySelector("a");
        Previous = previous is null ? null : new PaginationLink(previous);

        var next = Lookup.Child(element, "govuk-pagination__next")?.QuerySelector("a");
        Next = next is null ? null : new PaginationLink(next);

        Items = Lookup.Children(
            Lookup.Child(element, "govuk-pagination__list"),
            "govuk-pagination__item",
            e => new PaginationItem(e));
    }

    /// <summary>
    /// Gets the <c>&lt;nav&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the link to the previous page, or <see langword="null"/> if there isn't one.
    /// </summary>
    public PaginationLink? Previous { get; }

    /// <summary>
    /// Gets the link to the next page, or <see langword="null"/> if there isn't one.
    /// </summary>
    public PaginationLink? Next { get; }

    /// <summary>
    /// Gets the numbered pages and ellipses, in order.
    /// </summary>
    public IReadOnlyList<PaginationItem> Items { get; }

    /// <summary>
    /// Gets the item for the current page, or <see langword="null"/> if none is marked as current.
    /// </summary>
    public PaginationItem? Current => Items.FirstOrDefault(i => i.IsCurrent);

    /// <inheritdoc/>
    public override string ToString() => $"Pagination: {string.Join(" ", Items)}";
}

/// <summary>
/// The link to the previous or next page in a <see cref="Pagination"/>.
/// </summary>
public sealed class PaginationLink
{
    internal PaginationLink(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element.QuerySelector(".govuk-pagination__link-title"));
        Label = TextExtraction.GetTextOrNull(element.QuerySelector(".govuk-pagination__link-label"));
        Href = element.GetAttribute("href");
    }

    /// <summary>
    /// Gets the link's <c>&lt;a&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the link's visible text, e.g. <c>Next</c>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the label shown under the text in block-level pagination, e.g. <c>Applying for a provisional lorry or
    /// bus licence</c>, or <see langword="null"/> if there isn't one.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Gets the link's <c>href</c>.
    /// </summary>
    public string? Href { get; }

    /// <inheritdoc/>
    public override string ToString() => Label is null ? Text : $"{Text}: {Label}";
}

/// <summary>
/// A numbered page, or an ellipsis, in a <see cref="Pagination"/>.
/// </summary>
public sealed class PaginationItem
{
    internal PaginationItem(IElement element)
    {
        Element = element;
        IsEllipsis = element.ClassList.Contains("govuk-pagination__item--ellipsis");
        IsCurrent = element.ClassList.Contains("govuk-pagination__item--current");

        var link = element.QuerySelector("a");
        Number = IsEllipsis ? null : TextExtraction.GetText(link ?? element);
        Href = link?.GetAttribute("href");
    }

    /// <summary>
    /// Gets the item's <c>&lt;li&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the page number as shown, e.g. <c>2</c>, or <see langword="null"/> for an ellipsis.
    /// </summary>
    public string? Number { get; }

    /// <summary>
    /// Gets the link's <c>href</c>, or <see langword="null"/> for an ellipsis.
    /// </summary>
    public string? Href { get; }

    /// <summary>
    /// Gets whether this is the current page.
    /// </summary>
    public bool IsCurrent { get; }

    /// <summary>
    /// Gets whether this is an ellipsis standing for pages that aren't shown.
    /// </summary>
    public bool IsEllipsis { get; }

    /// <inheritdoc/>
    public override string ToString() => IsEllipsis ? "…" : IsCurrent ? $"[{Number}]" : Number ?? "";
}
