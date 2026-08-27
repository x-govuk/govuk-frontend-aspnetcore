using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record FeedbackOptions
{
    public string? TitleText { get; set; }
    public IHtmlContent? TitleHtml { get; set; }
    public int? HeadingLevel { get; set; }
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? TitleAttributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? BodyAttributes { get; set; }
}
