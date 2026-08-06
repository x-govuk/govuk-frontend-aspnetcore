using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record ExitThisPageOptions
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public TemplateString? RedirectUrl { get; set; }
    public TemplateString? Id { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public string? ActivatedText { get; set; }
    public string? TimedOutText { get; set; }
    public string? PressTwoMoreTimesText { get; set; }
    public string? PressOneMoreTimeText { get; set; }
}
