using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A details component.
/// </summary>
public sealed class Details
{
    internal const string Selector = "details.govuk-details";

    internal static readonly Lookup.Subject Subject = new("details component", "details components", "summaries");

    internal Details(IElement element)
    {
        Element = element;
        Summary = TextExtraction.GetText(
            Lookup.Child(element, "govuk-details__summary")?.QuerySelector(".govuk-details__summary-text"));
        var text = Lookup.Child(element, "govuk-details__text");
        TextLines = text is null ? [] : TextExtraction.GetLines(text);
        Text = string.Join(" ", TextLines);
        IsOpen = element.HasAttribute("open");
    }

    /// <summary>
    /// Gets the component's <c>&lt;details&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the summary that's shown when the component is closed, e.g. <c>Help with nationality</c>.
    /// </summary>
    public string Summary { get; }

    /// <summary>
    /// Gets the text that's revealed when the component is opened, as a single line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the lines of the text that's revealed when the component is opened, as split by line breaks and
    /// block-level elements.
    /// </summary>
    public IReadOnlyList<string> TextLines { get; }

    /// <summary>
    /// Gets whether the component is rendered open.
    /// </summary>
    public bool IsOpen { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Details: {Summary}";
}
