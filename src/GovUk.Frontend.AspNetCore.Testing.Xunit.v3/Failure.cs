using System.Diagnostics;
using System.Net;
using System.Text;
using AngleSharp.Dom;
using Microsoft.AspNetCore.WebUtilities;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

/// <summary>
/// Builds failure messages in xUnit's format: an <c>Assert.X() Failure: …</c> headline followed by aligned
/// <c>Label: value</c> lines and, where it helps, the HTML that was checked.
/// </summary>
[StackTraceHidden]
internal static class Failure
{
    /// <summary>
    /// Creates the exception for a failed assertion.
    /// </summary>
    /// <param name="assertion">The assertion's name, without <c>Assert.</c>, e.g. <c>ErrorSummaryContains</c>.</param>
    /// <param name="reason">What went wrong, e.g. <c>Error not found</c>.</param>
    /// <param name="lines">The labelled lines, e.g. <c>("Expected", "\"Enter your name\"")</c>.</param>
    /// <param name="html">The checked HTML, already formatted, or <see langword="null"/> to leave it out.</param>
    public static GovUkAssertException Create(
        string assertion,
        string reason,
        IReadOnlyList<(string Label, string Value)> lines,
        string? html = null)
    {
        var sb = new StringBuilder();
        sb.Append("Assert.").Append(assertion).Append("() Failure: ").Append(reason);

        var width = lines.Count == 0 ? 0 : lines.Max(l => l.Label.Length) + 2;

        foreach (var (label, value) in lines)
        {
            var prefix = (label + ":").PadRight(width);
            var continuation = new string(' ', width);

            sb.Append('\n').Append(prefix).Append(value.Replace("\n", "\n" + continuation, StringComparison.Ordinal));
        }

        if (html is not null)
        {
            sb.Append("\n\nHTML:\n").Append(html);
        }

        return new GovUkAssertException(sb.ToString());
    }

    /// <summary>
    /// Creates the exception for a failed assertion, with the HTML of <paramref name="scope"/>.
    /// </summary>
    public static GovUkAssertException Create(
        string assertion,
        string reason,
        IReadOnlyList<(string Label, string Value)> lines,
        INode? scope) =>
        Create(assertion, reason, lines, scope is null ? null : LookupFailure.FormatHtml(scope));

    /// <summary>
    /// Creates the exception for an assertion that failed because a lookup in the core package failed.
    /// </summary>
    /// <param name="assertion">The assertion's name, without <c>Assert.</c>.</param>
    /// <param name="failure">The lookup's failure.</param>
    /// <param name="hint">A suggestion for fixing the failure, or <see langword="null"/> to leave it out.</param>
    public static GovUkAssertException FromLookup(string assertion, LookupFailure failure, string? hint = null) =>
        Create(
            assertion,
            failure.Summary.TrimEnd('.'),
            hint is null
                ? [("Expected", failure.Expected), ("Actual", failure.Found)]
                : [("Expected", failure.Expected), ("Actual", failure.Found), ("Hint", hint)],
            failure.Html);

    /// <summary>
    /// Runs a lookup from the core package, turning a <see cref="ComponentLookupException"/> into an assertion
    /// failure.
    /// </summary>
    public static T Lookup<T>(string assertion, Func<T> lookup)
    {
        try
        {
            return lookup();
        }
        catch (ComponentLookupException ex)
        {
            throw FromLookup(assertion, ex.Failure);
        }
    }

    public static string Quote(string? value) => value is null ? "null" : LookupFailure.Quote(value);

    /// <summary>
    /// Formats values as xUnit formats a collection, e.g. <c>["Enter your name", "Enter an email address"]</c>.
    /// </summary>
    public static string List(IEnumerable<string> values) => $"[{LookupFailure.QuoteAll(values)}]";

    /// <summary>
    /// Formats a status code with its reason phrase, e.g. <c>302 Found</c>.
    /// </summary>
    public static string StatusCode(HttpStatusCode statusCode)
    {
        var code = (int)statusCode;
        var phrase = ReasonPhrases.GetReasonPhrase(code);

        return phrase.Length == 0 ? code.ToString(System.Globalization.CultureInfo.InvariantCulture) : $"{code} {phrase}";
    }
}
