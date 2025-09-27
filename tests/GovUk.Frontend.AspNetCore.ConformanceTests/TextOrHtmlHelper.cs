using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ConformanceTests;

public static class TextOrHtmlHelper
{
    public static IHtmlContent GetHtmlContent(string text, string html)
    {
        return html is not null
            ? new HtmlString(html)
            : text is not null ? new HtmlString(HtmlEncoder.Default.Encode(text)) : (IHtmlContent)null;
    }
}
