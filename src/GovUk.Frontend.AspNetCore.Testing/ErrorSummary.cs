using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// An error summary component.
/// </summary>
public sealed class ErrorSummary
{
    internal const string Selector = ".govuk-error-summary";

    internal ErrorSummary(IElement element)
    {
        Element = element;

        Title = TextExtraction.GetText(element.QuerySelector(".govuk-error-summary__title"));

        var body = element.QuerySelector(".govuk-error-summary__body");
        Description = TextExtraction.GetTextOrNull(body?.Children.FirstOrDefault(c => c.LocalName == "p"));

        Items = body?.QuerySelector(".govuk-error-summary__list")?.Children
            .Where(c => c.LocalName == "li")
            .Select(li => new ErrorSummaryItem(li))
            .ToArray() ?? [];
    }

    /// <summary>
    /// Gets the error summary's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the title, e.g. <c>There is a problem</c>.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the description shown above the list of errors, or <see langword="null"/> if there isn't one.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the errors, in the order they're listed.
    /// </summary>
    public IReadOnlyList<ErrorSummaryItem> Items { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Error summary: {LookupFailure.QuoteAll(Items.Select(i => i.Text))}";
}

/// <summary>
/// An error listed in an <see cref="ErrorSummary"/>.
/// </summary>
public sealed class ErrorSummaryItem
{
    internal ErrorSummaryItem(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element);
        Href = element.QuerySelector("a")?.GetAttribute("href");
    }

    /// <summary>
    /// Gets the item's <c>&lt;li&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the link's <c>href</c>, e.g. <c>#Email</c>, or <see langword="null"/> if the error isn't a link.
    /// </summary>
    public string? Href { get; }

    /// <inheritdoc/>
    public override string ToString() => Text;
}
