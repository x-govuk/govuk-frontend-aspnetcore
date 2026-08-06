using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record SkipLinkOptions
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public TemplateString? Href { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
