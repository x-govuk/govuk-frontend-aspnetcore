using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record GenericHeaderOptions
{
    public TemplateString? Url { get; set; }
    public string? LogoText { get; set; }
    public IHtmlContent? LogoHtml { get; set; }
    public TemplateString? ContainerClasses { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }

    internal string? Namespace { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ContainerAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? LogoAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? LinkAttributes { get; set; }
    [NonStandardParameter]
    public IHtmlContent? Html { get; set; }
}
