using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A breadcrumbs component.
/// </summary>
public sealed class Breadcrumbs
{
    internal const string Selector = ".govuk-breadcrumbs";

    internal Breadcrumbs(IElement element)
    {
        Element = element;
        Items = Lookup.Children(
            Lookup.Child(element, "govuk-breadcrumbs__list"),
            "govuk-breadcrumbs__list-item",
            e => new BreadcrumbsItem(e));
    }

    /// <summary>
    /// Gets the component's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the items, in order.
    /// </summary>
    public IReadOnlyList<BreadcrumbsItem> Items { get; }

    /// <summary>
    /// Gets the item for the current page, marked with <c>aria-current="page"</c>, or <see langword="null"/> if there
    /// isn't one.
    /// </summary>
    public BreadcrumbsItem? Current => Items.FirstOrDefault(i => i.IsCurrent);

    /// <inheritdoc/>
    public override string ToString() => $"Breadcrumbs: {string.Join(" > ", Items.Select(i => i.Text))}";
}

/// <summary>
/// An item in a <see cref="Breadcrumbs"/> component.
/// </summary>
public sealed class BreadcrumbsItem
{
    internal BreadcrumbsItem(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element);
        Href = Lookup.Child(element, "govuk-breadcrumbs__link")?.GetAttribute("href");
        IsCurrent = element.GetAttribute("aria-current") == "page";
    }

    /// <summary>
    /// Gets the item's <c>&lt;li&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the item's text, e.g. <c>Passports, travel and living abroad</c>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the <c>href</c> of the item's link, or <see langword="null"/> if it isn't a link.
    /// </summary>
    public string? Href { get; }

    /// <summary>
    /// Gets whether the item is the current page, marked with <c>aria-current="page"</c>.
    /// </summary>
    public bool IsCurrent { get; }

    /// <inheritdoc/>
    public override string ToString() => Text;
}
