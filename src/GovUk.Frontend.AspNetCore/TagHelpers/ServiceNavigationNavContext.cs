using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ServiceNavigationNavContext
{
    public string? TagName { get; set; }
    public string? AriaLabel { get; set; }
    public string? MenuButtonText { get; set; }
    public string? MenuButtonLabel { get; set; }
    public string? Label { get; set; }
    public string? Id { get; set; }
    public bool? CollapseNavigationOnMobile { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public List<ServiceNavigationOptionsNavigationItem> Items { get; } = [];
    public string? FirstItemTagName { get; set; }
    public (TemplateString Html, string TagName)? NavigationStartSlot { get; set; }
    public (TemplateString Html, string TagName)? NavigationEndSlot { get; set; }
}
