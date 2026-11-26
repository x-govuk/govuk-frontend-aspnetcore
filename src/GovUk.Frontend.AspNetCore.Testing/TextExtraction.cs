using System.Text;
using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// Gets the text a sighted user sees in an element.
/// </summary>
/// <remarks>
/// Whitespace is collapsed to single spaces and trimmed, line breaks (<c>&lt;br&gt;</c>) and block-level
/// elements separate lines, and text inside <c>govuk-visually-hidden</c> elements is left out.
/// </remarks>
internal static class TextExtraction
{
    private const string VisuallyHiddenClass = "govuk-visually-hidden";

    private static readonly HashSet<string> _blockElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "address", "article", "aside", "blockquote", "caption", "dd", "details", "div", "dl", "dt",
        "fieldset", "figcaption", "figure", "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6", "header",
        "hr", "legend", "li", "main", "nav", "ol", "p", "pre", "section", "summary", "table", "tbody", "td",
        "tfoot", "th", "thead", "tr", "ul"
    };

    private static readonly HashSet<string> _ignoredElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "script", "style", "template", "noscript"
    };

    public static string GetText(INode? node) => node is null ? "" : string.Join(" ", GetLines(node));

    public static string? GetTextOrNull(INode? node) => node is null ? null : GetText(node);

    public static IReadOnlyList<string> GetLines(INode node)
    {
        var lines = new List<string>();
        var current = new StringBuilder();

        Walk(node, isRoot: true);
        EndLine();

        return lines.AsReadOnly();

        void Walk(INode n, bool isRoot)
        {
            if (n is IText text)
            {
                current.Append(text.Data);
                return;
            }

            if (n is not IElement element)
            {
                foreach (var child in n.ChildNodes)
                {
                    Walk(child, isRoot: false);
                }

                return;
            }

            if (!isRoot && (IsVisuallyHidden(element) || _ignoredElements.Contains(element.LocalName)))
            {
                return;
            }

            if (element.LocalName.Equals("br", StringComparison.OrdinalIgnoreCase))
            {
                EndLine();
                return;
            }

            var isBlock = !isRoot && _blockElements.Contains(element.LocalName);

            if (isBlock)
            {
                EndLine();
            }

            foreach (var child in element.ChildNodes)
            {
                Walk(child, isRoot: false);
            }

            if (isBlock)
            {
                EndLine();
            }
        }

        void EndLine()
        {
            var line = CollapseWhitespace(current.ToString());
            current.Clear();

            if (line.Length > 0)
            {
                lines.Add(line);
            }
        }
    }

    /// <summary>
    /// Gets the text of the <c>govuk-visually-hidden</c> elements inside <paramref name="element"/>.
    /// </summary>
    public static string? GetVisuallyHiddenText(IElement element)
    {
        var hidden = element.QuerySelectorAll($".{VisuallyHiddenClass}")
            .Select(e => CollapseWhitespace(e.TextContent))
            .Where(t => t.Length > 0)
            .ToArray();

        return hidden.Length > 0 ? string.Join(" ", hidden) : null;
    }

    public static bool IsVisuallyHidden(IElement element) => element.ClassList.Contains(VisuallyHiddenClass);

    public static string CollapseWhitespace(string value)
    {
        var sb = new StringBuilder(value.Length);
        var pendingSpace = false;

        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c))
            {
                pendingSpace = sb.Length > 0;
                continue;
            }

            if (pendingSpace)
            {
                sb.Append(' ');
                pendingSpace = false;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}
