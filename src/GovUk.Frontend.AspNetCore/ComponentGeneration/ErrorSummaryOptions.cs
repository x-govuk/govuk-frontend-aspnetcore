using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record ErrorSummaryOptions
{
    public string? TitleText { get; set; }
    public IHtmlContent? TitleHtml { get; set; }
    public string? DescriptionText { get; set; }
    public IHtmlContent? DescriptionHtml { get; set; }
    public IReadOnlyCollection<ErrorSummaryOptionsErrorItem?>? ErrorList { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public bool? DisableAutoFocus { get; set; }

    [NonStandardParameter]
    public AttributeCollection? TitleAttributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? DescriptionAttributes { get; set; }
}

public record ErrorSummaryOptionsErrorItem
{
    public TemplateString? Href { get; set; }
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? ItemAttributes { get; set; }
}
