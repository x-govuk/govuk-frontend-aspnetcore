using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record AccordionOptions
{
    public TemplateString? Id { get; set; }
    public int? HeadingLevel { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public bool? RememberExpanded { get; set; }
    public string? HideAllSectionsText { get; set; }
    public string? HideSectionText { get; set; }
    public string? HideSectionAriaLabelText { get; set; }
    public string? ShowAllSectionsText { get; set; }
    public string? ShowSectionText { get; set; }
    public string? ShowSectionAriaLabelText { get; set; }
    public IReadOnlyCollection<AccordionOptionsItem?>? Items { get; set; }
}

public record AccordionOptionsItem
{
    public AccordionOptionsItemHeading? Heading { get; set; }
    public AccordionOptionsItemSummary? Summary { get; set; }
    public AccordionOptionsItemContent? Content { get; set; }
    public bool? Expanded { get; set; }
}

public record AccordionOptionsItemHeading
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
}

public record AccordionOptionsItemSummary
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
}

public record AccordionOptionsItemContent
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
}
