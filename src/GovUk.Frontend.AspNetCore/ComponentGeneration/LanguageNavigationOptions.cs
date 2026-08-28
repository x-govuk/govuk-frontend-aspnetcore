using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record LanguageNavigationOptions
{
    public IReadOnlyCollection<LanguageNavigationOptionsItem?>? Items { get; set; }
    public string? AriaLabel { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record LanguageNavigationOptionsItem
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public TemplateString? Lang { get; set; }
    public TemplateString? HrefLang { get; set; }
    public TemplateString? Dir { get; set; }
    public TemplateString? Href { get; set; }
    public bool? Current { get; set; }
    public string? LanguageDescriptionText { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
