using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal static class Extensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static IHtmlContent? EncodeHtml(this string? value) =>
        value is not null ? new HtmlString(HtmlEncoder.Default.Encode(value)) : null;
}
