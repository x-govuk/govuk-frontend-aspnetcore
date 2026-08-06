using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record CookieBannerOptions
{
    public TemplateString? AriaLabel { get; set; }
    public bool? Hidden { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public IReadOnlyCollection<CookieBannerOptionsMessage>? Messages { get; set; }
}

public record CookieBannerOptionsMessage
{
    public string? HeadingText { get; set; }
    public IHtmlContent? HeadingHtml { get; set; }
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public IReadOnlyCollection<CookieBannerOptionsMessageAction>? Actions { get; set; }
    public bool? Hidden { get; set; }
    public TemplateString? Role { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? HeadingAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ContentAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ActionsAttributes { get; set; }
}

public record CookieBannerOptionsMessageAction
{
    public string? Text { get; set; }
    public TemplateString? Type { get; set; }
    public TemplateString? Href { get; set; }
    public TemplateString? Name { get; set; }
    public TemplateString? Value { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
