using Microsoft.AspNetCore.Html;
namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record ServiceNavigationOptions
{
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public TemplateString? AriaLabel { get; set; }
    public string? MenuButtonText { get; set; }
    public TemplateString? MenuButtonLabel { get; set; }
    public TemplateString? NavigationLabel { get; set; }
    public TemplateString? NavigationId { get; set; }
    public TemplateString? NavigationClasses { get; set; }
    public bool? CollapseNavigationOnMobile { get; set; }
    public TemplateString? ServiceName { get; set; }
    public TemplateString? ServiceUrl { get; set; }
    public IReadOnlyCollection<ServiceNavigationOptionsNavigationItem>? Navigation { get; set; }
    public ServiceNavigationOptionsSlots? Slots { get; set; }

    [NonStandardParameter]
    public AttributeCollection? NavigationAttributes { get; set; }
}

public record ServiceNavigationOptionsNavigationItem
{
    public bool? Current { get; set; }
    public bool? Active { get; set; }
    public IHtmlContent? Html { get; set; }
    public string? Text { get; set; }
    public TemplateString? Href { get; set; }
    public AttributeCollection? Attributes { get; set; }
    [NonStandardParameter]
    public AttributeCollection? ItemAttributes { get; set; }
}

public record ServiceNavigationOptionsSlots
{
    public IHtmlContent? Start { get; set; }
    public ServiceNavigationOptionsEndSlot? End { get; set; }
    public IHtmlContent? NavigationStart { get; set; }
    public IHtmlContent? NavigationEnd { get; set; }
}

public record ServiceNavigationOptionsEndSlot
{
    public IHtmlContent? Html { get; set; }
    public string? Align { get; set; }
}
