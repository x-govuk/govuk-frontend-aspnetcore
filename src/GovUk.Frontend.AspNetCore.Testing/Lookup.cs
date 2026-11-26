using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

internal static class Lookup
{
    /// <summary>
    /// Describes what was searched and how to describe the candidates in a failure message.
    /// </summary>
    /// <param name="Singular">The thing being looked for, e.g. <c>summary list row</c>.</param>
    /// <param name="Plural">Its plural, e.g. <c>summary list rows</c>.</param>
    /// <param name="LabelPlural">
    /// What the candidates' labels are called, e.g. <c>keys</c>, or <see langword="null"/> if they're only counted.
    /// </param>
    public sealed record Subject(string Singular, string Plural, string? LabelPlural = null);

    /// <summary>
    /// Returns the single candidate that matches, throwing if more than one does or, when
    /// <paramref name="required"/> is <see langword="true"/>, if none do.
    /// </summary>
    public static T? Single<T>(
        INode scope,
        IReadOnlyList<T> candidates,
        Func<T, bool> predicate,
        Func<T, string> getLabel,
        Subject subject,
        string expected,
        bool required)
        where T : class
    {
        var matches = candidates.Where(predicate).ToArray();

        if (matches.Length == 1)
        {
            return matches[0];
        }

        if (matches.Length == 0)
        {
            if (!required)
            {
                return null;
            }

            throw new ComponentLookupException(new LookupFailure(
                $"{Capitalize(subject.Singular)} not found.",
                expected,
                LookupFailure.DescribeCandidates(subject.Plural, subject.LabelPlural, candidates.Select(getLabel).ToArray()),
                LookupFailure.FormatHtml(scope)));
        }

        throw new ComponentLookupException(new LookupFailure(
            $"More than one {subject.Singular} found.",
            expected,
            $"{matches.Length} {subject.Plural}",
            LookupFailure.FormatHtml(scope)));
    }

    /// <summary>
    /// Returns the only component in <paramref name="scope"/>, <see langword="null"/> if there are none, or throws
    /// if there's more than one.
    /// </summary>
    public static T? SingleOrNone<T>(INode scope, IReadOnlyList<T> candidates, Subject subject)
        where T : class =>
        Single(scope, candidates, _ => true, _ => "", subject, $"at most one {subject.Singular}", required: false);

    public static IReadOnlyList<T> All<T>(IParentNode scope, string selector, Func<IElement, T> create) =>
        scope.QuerySelectorAll(selector).Select(create).ToArray();

    public static IReadOnlyList<T> Children<T>(IElement? parent, string className, Func<IElement, T> create) =>
        parent is null ? [] : parent.Children.Where(c => c.ClassList.Contains(className)).Select(create).ToArray();

    public static IElement? Child(IElement? parent, string className) =>
        parent?.Children.FirstOrDefault(c => c.ClassList.Contains(className));

    internal static string Capitalize(string value) =>
        value.Length == 0 ? value : string.Concat(char.ToUpperInvariant(value[0]).ToString(), value.AsSpan(1));
}
