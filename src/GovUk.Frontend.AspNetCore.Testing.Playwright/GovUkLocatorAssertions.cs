using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// Assertions for GOV.UK Design System components, as extension members on Playwright's
/// <see cref="ILocatorAssertions"/>.
/// </summary>
/// <remarks>
/// <para>
/// Each one calls one of Playwright's own matchers, so it retries until the default or given timeout, and works with
/// <c>.Not</c>. A failure's message is Playwright's, with a line above it that says which component was checked.
/// </para>
/// <para>
/// As with Playwright's own matchers, they fail when nothing matches the locator, even with <c>.Not</c>. To check
/// that a component isn't there, use <c>Expect(locator).ToHaveCountAsync(0)</c>.
/// </para>
/// <para>
/// The assertions can't see the locator they're called on, so they rely on being given the right one, e.g.
/// <see cref="FieldLocator.ErrorMessage"/> for <see cref="ToHaveErrorMessageAsync"/>.
/// </para>
/// </remarks>
public static class GovUkLocatorAssertions
{
    // How Playwright's messages say that nothing matched the locator: a single-value matcher reports "element(s) not
    // found", and a list matcher reports an empty list.
    private const string NotFound = "element(s) not found";
    private const string NoneFound = "But was: '[]'";

    extension(ILocatorAssertions assertions)
    {
        /// <summary>
        /// Ensures a field's error message is <paramref name="expected"/>, ignoring the visually hidden text before it
        /// (<c>Error:</c> by default).
        /// </summary>
        /// <param name="expected">The error message, without the visually hidden prefix.</param>
        /// <param name="options">Options for the underlying <c>ToHaveTextAsync</c>.</param>
        /// <example>
        /// <code>
        /// await Expect(page.GetFieldByLabelOrLegend("Email address").ErrorMessage)
        ///     .ToHaveErrorMessageAsync("Enter an email address");
        /// </code>
        /// </example>
        public async Task ToHaveErrorMessageAsync(string expected, LocatorAssertionsToHaveTextOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(assertions);

            await WithHeadlineAsync(
                () => assertions.ToHaveTextAsync(ErrorMessagePattern(expected), options),
                $"GOV.UK error message assertion for {Quote(expected)}, ignoring any visually hidden prefix such as \"Error:\".",
                "No error message was found: the field may not have an error, or the locator may not be a field's ErrorMessage.",
                NotFound)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures an error summary's errors are exactly <paramref name="expected"/>, in order.
        /// </summary>
        /// <param name="expected">Every error in the error summary, in order.</param>
        /// <param name="options">Options for the underlying <c>ToHaveTextAsync</c>.</param>
        /// <remarks>
        /// Call this on <see cref="ErrorSummaryLocator.Items"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// await Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter your name"]);
        /// </code>
        /// </example>
        public async Task ToHaveErrorSummaryErrorsAsync(
            IEnumerable<string> expected,
            LocatorAssertionsToHaveTextOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(assertions);
            ArgumentNullException.ThrowIfNull(expected);

            var errors = expected.ToArray();

            await WithHeadlineAsync(
                () => assertions.ToHaveTextAsync(errors, options),
                $"GOV.UK error summary assertion for the errors [{string.Join(", ", errors.Select(Quote))}].",
                "No errors were found: the page may not have an error summary, or the locator may not be an error summary's Items.",
                NoneFound)
                .ConfigureAwait(false);
        }
    }

    extension(FieldLocator field)
    {
        /// <summary>
        /// Ensures the field's error message is <paramref name="expectedError"/> and, unless
        /// <paramref name="inErrorSummary"/> is <see langword="false"/>, that the page's error summary lists it with a
        /// link to the field.
        /// </summary>
        /// <param name="expectedError">The error message, without the visually hidden prefix.</param>
        /// <param name="inErrorSummary">
        /// Whether to check that the page's error summary lists <paramref name="expectedError"/> with a link to an
        /// element in the field, as the error summary generated from model state does. Pass <see langword="false"/>
        /// for an error that isn't in the error summary, or a page without one.
        /// </param>
        /// <param name="options">
        /// Options for the underlying matchers. The timeout applies to the error message and to the error summary.
        /// </param>
        /// <remarks>
        /// <para>
        /// The error message is checked as <see cref="ToHaveErrorMessageAsync"/> checks it, and the error summary's
        /// item must have exactly the same text. Both checks retry until the timeout. The link is checked once the
        /// item is found: its <c>href</c> must be <c>#</c> followed by the id of an element in the field.
        /// </para>
        /// <para>
        /// This isn't one of Playwright's matchers, since it checks two parts of the page, so it doesn't support
        /// <c>.Not</c>. To check that a field has no error, use
        /// <c>Expect(field.ErrorMessage).ToHaveCountAsync(0)</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// await page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address");
        /// </code>
        /// </example>
        public async Task ExpectErrorAsync(
            string expectedError,
            bool inErrorSummary = true,
            LocatorAssertionsToHaveTextOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(field);
            ArgumentNullException.ThrowIfNull(expectedError);

            await Assertions.Expect(field.ErrorMessage).ToHaveErrorMessageAsync(expectedError, options).ConfigureAwait(false);

            if (!inErrorSummary)
            {
                return;
            }

            var items = field.Locator.Page.ErrorSummary.Items.Filter(new() { HasTextRegex = Selectors.ExactText(expectedError) });

            try
            {
                await Assertions.Expect(items).Not.ToHaveCountAsync(0, new() { Timeout = options?.Timeout }).ConfigureAwait(false);
            }
            catch (PlaywrightException ex)
            {
                throw new PlaywrightException(
                    $"GOV.UK field error assertion: the page has no error summary listing {Quote(expectedError)}.\n" +
                    $"If the error shouldn't be in the error summary, pass inErrorSummary: false to {nameof(ExpectErrorAsync)}.\n" +
                    ex.Message,
                    ex);
            }

            var hrefs = await items
                .EvaluateAllAsync<string?[]>("items => items.map(i => i.querySelector('a')?.getAttribute('href') ?? null)")
                .ConfigureAwait(false);

            var linksToField = await field.Locator.EvaluateAsync<bool>(LinksToFieldScript, hrefs).ConfigureAwait(false);

            if (!linksToField)
            {
                throw new PlaywrightException(
                    $"GOV.UK field error assertion: the error summary lists {Quote(expectedError)}, but doesn't link it to the field.\n" +
                    $"Expected: a link to an element in the field\n" +
                    $"Actual:   {string.Join(", ", hrefs.Select(h => h is null ? "no link" : $"a link to {Quote(h)}"))}");
            }
        }
    }

    /// <summary>
    /// Whether any of the hrefs passed is <c>#</c> followed by the id of an element in the form group it runs on.
    /// </summary>
    private const string LinksToFieldScript = """
        (formGroup, hrefs) => hrefs.some(href => {
            if (!href || href.length < 2 || href[0] !== '#') return false;
            let id;
            try { id = decodeURIComponent(href.slice(1)); } catch { return false; }
            const target = document.getElementById(id);
            return target !== null && formGroup.contains(target);
        })
        """;

    /// <summary>
    /// Matches an error message's text: <paramref name="expected"/>, after an optional visually hidden prefix that ends
    /// in a colon. The prefix can't itself contain a colon, so an expected message of <c>B</c> doesn't match
    /// <c>Error: A: B</c>.
    /// </summary>
    internal static Regex ErrorMessagePattern(string expected) =>
        new($"^\\s*(?:[^:]+:\\s*)?{Selectors.Words(expected)}\\s*$");

    /// <summary>
    /// Runs one of Playwright's matchers and, if it fails, puts <paramref name="headline"/> above its message.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The headline has to read correctly whether or not <c>.Not</c> was applied, since there's no way to tell.
    /// </para>
    /// <para>
    /// <paramref name="notFoundHint"/> is added when Playwright's message contains <paramref name="notFoundMarker"/>,
    /// meaning nothing matched the locator. A matcher fails then even with <c>.Not</c>, so the hint is worded to
    /// read correctly either way. If a later Playwright words its message differently, the hint is just left out.
    /// </para>
    /// </remarks>
    private static async Task WithHeadlineAsync(
        Func<Task> assertion,
        string headline,
        string notFoundHint,
        string notFoundMarker)
    {
        try
        {
            await assertion().ConfigureAwait(false);
        }
        catch (PlaywrightException ex)
        {
            var message = ex.Message.Contains(notFoundMarker, StringComparison.Ordinal)
                ? $"{headline}\n{notFoundHint}\n{ex.Message}"
                : $"{headline}\n{ex.Message}";

            throw new PlaywrightException(message, ex);
        }
    }

    private static string Quote(string value) => $"\"{value}\"";
}
