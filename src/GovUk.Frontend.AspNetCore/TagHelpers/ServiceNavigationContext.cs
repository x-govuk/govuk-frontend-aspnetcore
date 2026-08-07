using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class ServiceNavigationContext
{
    public ServiceNavigationNavContext? Nav { get; set; }
    public (IHtmlContent Html, string TagName)? StartSlot { get; set; }
    public (IHtmlContent Html, string TagName)? EndSlot { get; set; }
}
