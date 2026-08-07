using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record DetailsOptions
{
    public TemplateString? Id { get; set; }
    public bool? Open { get; set; }
    public IHtmlContent? SummaryHtml { get; set; }
    public string? SummaryText { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? Text { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? SummaryAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? TextAttributes { get; set; }
}
