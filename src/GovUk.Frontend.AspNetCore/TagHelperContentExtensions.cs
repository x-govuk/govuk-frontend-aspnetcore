using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore;

internal static class TagHelperContentExtensions
{
    public static IHtmlContent Snapshot(this TagHelperContent content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return new HtmlString(content.GetContent());
    }
}
