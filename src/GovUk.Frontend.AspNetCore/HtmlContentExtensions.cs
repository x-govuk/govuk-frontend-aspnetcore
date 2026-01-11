using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

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

    internal static bool IsEmpty(this IHtmlContent? content, HtmlEncoder? encoder = null)
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
        public bool IsEmpty { get; private set; } = true;

        public override void Write(char[] buffer, int index, int count)
        {
            if (!IsEmpty)
            {
                return;
            }

            IsEmpty ^= !buffer.All(Char.IsWhiteSpace);
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}
