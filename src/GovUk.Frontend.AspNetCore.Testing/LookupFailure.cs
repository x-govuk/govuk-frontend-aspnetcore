using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// Describes why a lookup of a component, or of part of one, failed.
/// </summary>
/// <remarks>
/// <see cref="ToString"/> gives the message used by <see cref="ComponentLookupException"/>. Assertion libraries
/// can use the individual parts to build messages in their own format.
/// </remarks>
public sealed class LookupFailure
{
    // Enough for a summary list or a form; beyond this the fragment stops being readable anyway.
    internal const int MaxHtmlLength = 4000;

    /// <summary>
    /// Creates a new <see cref="LookupFailure"/>.
    /// </summary>
    /// <param name="summary">A sentence saying what went wrong, e.g. <c>Summary list row not found.</c></param>
    /// <param name="expected">What was looked for, e.g. <c>a row with the key "Email address"</c>.</param>
    /// <param name="found">What was there instead, e.g. <c>rows with the keys "Name", "Phone"</c>.</param>
    /// <param name="html">The HTML that was searched, if it's narrower than the whole page.</param>
    public LookupFailure(string summary, string expected, string found, string? html = null)
    {
        ArgumentNullException.ThrowIfNull(summary);
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(found);

        Summary = summary;
        Expected = expected;
        Found = found;
        Html = html;
    }

    /// <summary>
    /// Gets a sentence saying what went wrong, e.g. <c>Summary list row not found.</c>
    /// </summary>
    public string Summary { get; }

    /// <summary>
    /// Gets a description of what was looked for, e.g. <c>a row with the key "Email address"</c>.
    /// </summary>
    public string Expected { get; }

    /// <summary>
    /// Gets a description of what was found instead, e.g. <c>rows with the keys "Name", "Phone"</c>.
    /// </summary>
    public string Found { get; }

    /// <summary>
    /// Gets the HTML that was searched, formatted and truncated, or <see langword="null"/> if the whole page was
    /// searched.
    /// </summary>
    public string? Html { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Summary).Append('\n');
        sb.Append("Expected: ").Append(Expected).Append('\n');
        sb.Append("Found:    ").Append(Found);

        if (Html is not null)
        {
            sb.Append("\n\nHTML:\n").Append(Html);
        }

        return sb.ToString();
    }

    internal static string? FormatHtml(INode scope)
    {
        if (scope is not IElement element)
        {
            return null;
        }

        var html = element.ToHtml(new PrettyMarkupFormatter { Indentation = "  ", NewLine = "\n" }).Trim();

        return html.Length > MaxHtmlLength
            ? string.Concat(html.AsSpan(0, MaxHtmlLength), "\n… (truncated)")
            : html;
    }

    internal static string Quote(string value) => $"\"{value}\"";

    internal static string QuoteAll(IEnumerable<string> values) => string.Join(", ", values.Select(Quote));

    /// <summary>
    /// Describes a set of candidates, e.g. <c>rows with the keys "Name", "Phone"</c>, or <c>no rows</c> if there
    /// are none.
    /// </summary>
    internal static string DescribeCandidates(string plural, string? labelPlural, IReadOnlyCollection<string> labels) =>
        labels.Count == 0 ? $"no {plural}"
            : labelPlural is null ? $"{labels.Count} {plural}"
            : $"{plural} with the {labelPlural} {QuoteAll(labels)}";
}

/// <summary>
/// The exception thrown when a component, or part of one, is not on the page, or when more than one matches.
/// </summary>
public sealed class ComponentLookupException : InvalidOperationException
{
    /// <summary>
    /// Creates a new <see cref="ComponentLookupException"/>.
    /// </summary>
    public ComponentLookupException(LookupFailure failure)
        : base((failure ?? throw new ArgumentNullException(nameof(failure))).ToString())
    {
        Failure = failure;
    }

    /// <summary>
    /// Gets the details of what was looked for and what was found.
    /// </summary>
    public LookupFailure Failure { get; }
}
