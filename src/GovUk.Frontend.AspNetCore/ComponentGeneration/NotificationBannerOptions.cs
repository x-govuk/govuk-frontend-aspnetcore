using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record NotificationBannerOptions
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? TitleText { get; set; }
    public IHtmlContent? TitleHtml { get; set; }
    public int? TitleHeadingLevel { get; set; }
    public TemplateString? Type { get; set; }
    public TemplateString? Role { get; set; }
    public TemplateString? TitleId { get; set; }
    public bool? DisableAutoFocus { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
