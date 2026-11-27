using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// An accordion component.
/// </summary>
public sealed class Accordion
{
    internal const string Selector = ".govuk-accordion";

    internal static readonly Lookup.Subject Subject = new("accordion", "accordions");

    private static readonly Lookup.Subject _sectionSubject = new("accordion section", "sections", "headings");

    internal Accordion(IElement element)
    {
        Element = element;
        Sections = Lookup.Children(element, "govuk-accordion__section", e => new AccordionSection(e));
    }

    /// <summary>
    /// Gets the accordion's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the sections, in order.
    /// </summary>
    public IReadOnlyList<AccordionSection> Sections { get; }

    /// <summary>
    /// Gets the section with the heading <paramref name="heading"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such section, or there's more than one.</exception>
    public AccordionSection GetSectionByHeading(string heading) => FindSectionByHeading(heading, required: true)!;

    /// <summary>
    /// Gets the section with the heading <paramref name="heading"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such section.</exception>
    public AccordionSection? FindSectionByHeading(string heading) => FindSectionByHeading(heading, required: false);

    private AccordionSection? FindSectionByHeading(string heading, bool required)
    {
        ArgumentNullException.ThrowIfNull(heading);

        return Lookup.Single(
            Element,
            Sections,
            s => s.Heading == heading,
            s => s.Heading,
            _sectionSubject,
            $"a section with the heading {LookupFailure.Quote(heading)}",
            required);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Accordion: {LookupFailure.QuoteAll(Sections.Select(s => s.Heading))}";
}

/// <summary>
/// A section of an <see cref="Accordion"/>.
/// </summary>
public sealed class AccordionSection
{
    internal AccordionSection(IElement element)
    {
        Element = element;

        var header = Lookup.Child(element, "govuk-accordion__section-header");
        Heading = TextExtraction.GetText(header?.QuerySelector(".govuk-accordion__section-button"));
        Summary = TextExtraction.GetTextOrNull(Lookup.Child(header, "govuk-accordion__section-summary"));

        ContentElement = Lookup.Child(element, "govuk-accordion__section-content");
        TextLines = ContentElement is null ? [] : TextExtraction.GetLines(ContentElement);
        Text = string.Join(" ", TextLines);
        IsExpanded = element.ClassList.Contains("govuk-accordion__section--expanded");
    }

    /// <summary>
    /// Gets the section's element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the section's heading, e.g. <c>Writing well for the web</c>.
    /// </summary>
    public string Heading { get; }

    /// <summary>
    /// Gets the summary under the heading, or <see langword="null"/> if there isn't one.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// Gets the element with the section's content, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public IElement? ContentElement { get; }

    /// <summary>
    /// Gets the section's content as a single line of text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the lines of the section's content, as split by line breaks and block-level elements.
    /// </summary>
    public IReadOnlyList<string> TextLines { get; }

    /// <summary>
    /// Gets whether the section is rendered expanded.
    /// </summary>
    public bool IsExpanded { get; }

    /// <inheritdoc/>
    public override string ToString() => Heading;
}
