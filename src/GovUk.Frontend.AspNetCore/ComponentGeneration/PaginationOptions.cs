using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record PaginationOptions
{
    public IReadOnlyCollection<PaginationOptionsItem?>? Items { get; set; }
    public PaginationOptionsPrevious? Previous { get; set; }
    public PaginationOptionsNext? Next { get; set; }
    public TemplateString? LandmarkLabel { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record PaginationOptionsItem
{
    public string? Number { get; set; }
    [NonStandardParameter]
    public IHtmlContent? NumberHtml { get; set; }
    public string? VisuallyHiddenText { get; set; }
    public TemplateString? Href { get; set; }
    public bool? Current { get; set; }
    public bool? Ellipsis { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record PaginationOptionsPrevious
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? LabelText { get; set; }
    public TemplateString? Href { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? ContainerAttributes { get; set; }
}

public record PaginationOptionsNext
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? LabelText { get; set; }
    public TemplateString? Href { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? ContainerAttributes { get; set; }
}
