using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ServiceNavigationContext
{
    public ServiceNavigationNavContext? Nav { get; set; }
    public (TemplateString Html, string TagName)? StartSlot { get; set; }
    public (TemplateString Html, string TagName)? EndSlot { get; set; }
}
