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

    public static AttributeCollection ToAttributeCollection(this AttributeDictionary? attributeDictionary)
    {
        if (attributeDictionary is null)
        {
            return new AttributeCollection();
        }

        var collection = new AttributeCollection();
        foreach (var kvp in attributeDictionary)
        {
            collection.Add(kvp.Key, kvp.Value);
        }
        return collection;
    }

    public static TemplateString ToTemplateString(this IHtmlContent content)
    {
        return new TemplateString(content.ToHtmlString());
    }

    public static string ToHtmlString(this IHtmlContent content)
    {
        using var writer = new System.IO.StringWriter();
        content.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }

    public static AttributeCollection MergeWith(this AttributeCollection source, AttributeCollection other)
    {
        var result = source.Clone();
        foreach (var attr in other.GetAttributes())
        {
            result.Add(attr);
        }
        return result;
    }
}
