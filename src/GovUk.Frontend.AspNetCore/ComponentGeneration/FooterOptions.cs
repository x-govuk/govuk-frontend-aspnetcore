using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record FooterOptions
{
    public FooterOptions()
    {
        ContentLicence = new();
        Copyright = new();
    }

    public FooterOptionsMeta? Meta { get; set; }
    public IReadOnlyCollection<FooterOptionsNavigation?>? Navigation { get; set; }
    public FooterOptionsContentLicence? ContentLicence { get; set; }
    public FooterOptionsCopyright? Copyright { get; set; }
    public TemplateString? ContainerClasses { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record FooterOptionsMeta
{
    public TemplateString? VisuallyHiddenTitle { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? Text { get; set; }
    public IReadOnlyCollection<FooterOptionsMetaItem?>? Items { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ContentAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ItemsAttributes { get; set; }
}

public record FooterOptionsMetaItem
{
    public string? Text { get; set; }
    [NonStandardParameter]
    public IHtmlContent? Html { get; set; }
    public TemplateString? Href { get; set; }
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ItemAttributes { get; set; }
}

public record FooterOptionsNavigation
{
    public IHtmlContent? Title { get; set; }
    public int? Columns { get; set; }
    public TemplateString? Width { get; set; }
    public IReadOnlyCollection<FooterOptionsNavigationItem?>? Items { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ItemsAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? TitleAttributes { get; set; }
}

public record FooterOptionsNavigationItem
{
    public string? Text { get; set; }
    [NonStandardParameter]
    public IHtmlContent? Html { get; set; }
    public TemplateString? Href { get; set; }
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ItemAttributes { get; set; }
}

public record FooterOptionsContentLicence
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record FooterOptionsCopyright
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}
