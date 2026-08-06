using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ServiceNavigationNavContext
{
    public string? TagName { get; set; }
    public TemplateString? AriaLabel { get; set; }
    public string? MenuButtonText { get; set; }
    public TemplateString? MenuButtonLabel { get; set; }
    public TemplateString? Label { get; set; }
    public TemplateString? Id { get; set; }
    public bool? CollapseNavigationOnMobile { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public List<ServiceNavigationOptionsNavigationItem> Items { get; } = [];
    public string? FirstItemTagName { get; set; }
    public (IHtmlContent Html, string TagName)? NavigationStartSlot { get; set; }
    public (IHtmlContent Html, string TagName)? NavigationEndSlot { get; set; }
}
