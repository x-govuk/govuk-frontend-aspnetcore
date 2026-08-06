using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Utility extensions for <see cref="IHtmlContent"/>.
/// </summary>
public static class HtmlContentExtensions
{
    /// <summary>
    /// Returns a <see cref="string"/> of HTML with the contents of the <paramref name="content"/>.
    /// </summary>
    public static string ToHtmlString(this IHtmlContent content, HtmlEncoder? encoder = null)
    {
        ArgumentNullException.ThrowIfNull(content);

        encoder ??= HtmlEncoder.Default;

        if (content is HtmlString htmlString)
        {
            return htmlString.Value ?? string.Empty;
        }

        using var writer = new StringWriter();
        content.WriteTo(writer, encoder);
        return writer.ToString();
    }

    /// <summary>
    /// Takes a copy of <paramref name="content"/> if it needs one to stay valid after the tag helper
    /// that produced it has finished.
    /// </summary>
    /// <remarks>
    /// Razor reuses <see cref="TagHelperContent"/> instances once a tag helper has rendered, so content
    /// held beyond that point has to be snapshotted or it ends up with somebody else's markup in it.
    /// </remarks>
    [return: NotNullIfNotNull(nameof(content))]
    public static IHtmlContent? Snapshot(this IHtmlContent? content) =>
        content is TagHelperContent tagHelperContent ? new HtmlString(tagHelperContent.GetContent()) : content;

    /// <summary>
    /// Whether <paramref name="value"/> has no content, matching how the <see cref="IHtmlContent"/>
    /// overload treats content that renders as nothing but whitespace.
    /// </summary>
    internal static bool IsEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    internal static bool IsEmpty([NotNullWhen(false)] this IHtmlContent? content, HtmlEncoder? encoder = null)
    {
        if (content is null)
        {
            return true;
        }

        encoder ??= HtmlEncoder.Default;

        if (content is HtmlString htmlString)
        {
            return string.IsNullOrEmpty(htmlString.Value);
        }

        using var writer = new IsEmptyStringWriter();
        content.WriteTo(writer, encoder);
        return writer.IsEmpty;
    }

    private class IsEmptyStringWriter : TextWriter
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public bool IsEmpty { get; private set; } = true;

        public override void Write(char[] buffer, int index, int count)
        {
            if (!IsEmpty)
            {
                return;
            }

            IsEmpty ^= !buffer.All(char.IsWhiteSpace);
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
