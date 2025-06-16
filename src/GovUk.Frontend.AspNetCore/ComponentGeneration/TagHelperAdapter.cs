using System.Text.Encodings.Web;
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

        // This is a roughly good enough HTML parser that lets us extract the root tag and its attributes.
        // It's certainly not fully-compliant but it's enough for the components that we generate.

        var rootStartTagStartsAt = html.IndexOf('<', 0);
        var rootStartTagNameEndsAt = html.IndexOfAny([' ', '/', '>'], rootStartTagStartsAt) - 1;
        var rootStartTagEndsAt = html.IndexOf('>', rootStartTagStartsAt + 1);

        var rootTagName = html.Substring(rootStartTagStartsAt + 1, rootStartTagNameEndsAt - rootStartTagStartsAt);
        var isSelfClosing = html[rootStartTagEndsAt - 1] == '/';

        var attributeList = html
            .Substring(rootStartTagNameEndsAt + 1, html.LastIndexOf('>', rootStartTagEndsAt) - rootStartTagNameEndsAt - 1)
            .TrimStart(' ')
            .TrimEnd('/', ' ');

        var attributes = new TagHelperAttributeList();
        for (var i = 0; i < attributeList.Length;)
        {
            var endOfAttributeName = attributeList.IndexOfAny(['=', ' '], i) - 1;
            if (endOfAttributeName == -2)
            {
                endOfAttributeName = attributeList.Length + 1;
            }

            var attributeName = attributeList.Substring(i, endOfAttributeName - i + 1);
            i += attributeName.Length + 1;

            if (attributeList[endOfAttributeName + 1] == '=')
            {
                var startOfAttributeValue = endOfAttributeName + 3;
                var endOfAttributeValue = attributeList.IndexOf('"', startOfAttributeValue + 1);
                var attributeValue = endOfAttributeValue != -1 ?
                    attributeList.Substring(startOfAttributeValue, endOfAttributeValue - startOfAttributeValue) :
                    string.Empty;
                i += attributeValue.Length + 3;

                attributes.Add(new TagHelperAttribute(attributeName, new HtmlString(attributeValue), HtmlAttributeValueStyle.DoubleQuotes));
            }
            else
            {
                attributes.Add(new TagHelperAttribute(attributeName, null, HtmlAttributeValueStyle.Minimized));
            }
        }

        string innerHtml = string.Empty;
        if (!isSelfClosing)
        {
            var rootEndTagStartsAt = html.LastIndexOf("</", StringComparison.InvariantCulture);
            if (rootEndTagStartsAt == -1)
            {
                isSelfClosing = true;
            }
            else
            {
                innerHtml = html.Substring(rootStartTagEndsAt + 1, rootEndTagStartsAt - rootStartTagEndsAt - 1);
            }
        }

        var tagMode = isSelfClosing ? TagMode.SelfClosing : TagMode.StartTagAndEndTag;

        return new ComponentTagHelperOutput(rootTagName, tagMode, attributes, new HtmlString(innerHtml));
    }

    internal record ComponentTagHelperOutput(
        string? TagName,
        TagMode TagMode,
        ReadOnlyTagHelperAttributeList Attributes,
        IHtmlContent InnerHtml)
    {
        private static readonly ReadOnlyTagHelperAttributeList _emptyAttributes = new TagHelperAttributeList();

        private static readonly IHtmlContent _emptyContent = new HtmlString("");

        public static ComponentTagHelperOutput Empty { get; } =
            new(null, TagMode.StartTagAndEndTag, _emptyAttributes, _emptyContent);
    }
}
