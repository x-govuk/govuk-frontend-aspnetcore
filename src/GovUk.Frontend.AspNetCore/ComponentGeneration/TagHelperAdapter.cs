using System.Text.Encodings.Web;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal static class TagHelperAdapter
{
    public static void ApplyComponentHtml(this TagHelperOutput output, IHtmlContent content, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(encoder);

        var unwrapped = UnwrapComponent(content, encoder);
        ApplyComponentHtml(output, unwrapped);
    }

    public static void ApplyComponentHtml(TagHelperOutput output, string html)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(html);

        var unwrapped = UnwrapComponent(html);
        ApplyComponentHtml(output, unwrapped);
    }

    public static void ApplyComponentHtml(TagHelperOutput output, ComponentTagHelperOutput unwrapped)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(unwrapped);

        output.TagName = unwrapped.TagName;
        output.TagMode = unwrapped.TagMode;

        output.Attributes.Clear();

        foreach (var attribute in unwrapped.Attributes)
        {
            output.Attributes.Add(attribute);
        }

        output.Content.AppendHtml(unwrapped.InnerHtml);
    }

    internal static ComponentTagHelperOutput UnwrapComponent(IHtmlContent content, HtmlEncoder encoder) =>
        UnwrapComponent(content.ToHtmlString(encoder));

    internal static ComponentTagHelperOutput UnwrapComponent(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        if (string.IsNullOrWhiteSpace(html))
        {
            return ComponentTagHelperOutput.Empty;
        }

        // We only need to parse the root tag and its attributes;
        // if any inner content is malformed we want to preserve it and output as-is.

        var startTagStart = html.IndexOf('<', 0);
        var startTagEnd = html.IndexOf('>', startTagStart + 1);
        var startTag = html[startTagStart..(startTagEnd + 1)];

        var tagNameStart = startTagStart + 1;
        var tagNameEnd = ReadUntil([' ', '>', '=', '/'], tagNameStart + 1);
        var tagName = html[tagNameStart..tagNameEnd];

        var rootTag = startTag;
        var isSelfClosing = startTag.EndsWith("/>", StringComparison.Ordinal);
        var tagMode = isSelfClosing ? TagMode.SelfClosing : TagMode.StartTagAndEndTag;

        IHtmlContent innerHtml;
        if (isSelfClosing)
        {
            innerHtml = HtmlString.Empty;
        }
        else
        {
            var endTagStart = html.LastIndexOf($"</{tagName}>", StringComparison.Ordinal);
            if (endTagStart == -1)
            {
                innerHtml = HtmlString.Empty;
                tagMode = TagMode.StartTagOnly;
            }
            else
            {
                rootTag += html[endTagStart..];
                innerHtml = new HtmlString(html[(startTagEnd + 1)..endTagStart]);
            }
        }

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(rootTag);
        var rootNode = htmlDocument.DocumentNode.FirstChild;

        var attributes = new TagHelperAttributeList();
        foreach (var attribute in rootNode.Attributes)
        {
            var tagHelperAttribute = attribute.QuoteType is AttributeValueQuote.WithoutValue
                ? new TagHelperAttribute(attribute.Name)
                : new TagHelperAttribute(attribute.Name, new HtmlString(attribute.Value));

            attributes.Add(tagHelperAttribute);
        }

        return new ComponentTagHelperOutput(tagName, tagMode, attributes, innerHtml);

        int ReadUntil(char[] stopChars, int startIndex)
        {
            var index = startIndex;

            while (index < html.Length && !stopChars.Contains(html[index]))
            {
                index++;
            }

            return index;
        }
    }

    internal record ComponentTagHelperOutput(
        string? TagName,
        TagMode TagMode,
        ReadOnlyTagHelperAttributeList Attributes,
        IHtmlContent InnerHtml)
    {
        public static ReadOnlyTagHelperAttributeList EmptyAttributes { get; } =
            new TagHelperAttributeList();

        public static IHtmlContent EmptyContent { get; } = new HtmlString("");

        public static ComponentTagHelperOutput Empty { get; } =
            new(null, TagMode.StartTagAndEndTag, EmptyAttributes, EmptyContent);
    }
}
