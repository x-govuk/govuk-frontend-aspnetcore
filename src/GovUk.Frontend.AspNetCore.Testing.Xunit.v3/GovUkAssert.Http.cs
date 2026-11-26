using System.Diagnostics;
using System.Net;
using AngleSharp.Html.Dom;
using Xunit;

namespace GovUk.Frontend.AspNetCore.Testing.Xunit;

/// <summary>
/// GOV.UK Frontend assertions, added to xUnit's <see cref="Assert"/> as extension members.
/// </summary>
/// <remarks>
/// With C# 14 or later, call these as <c>Assert.ErrorSummaryContains(doc, "…")</c>. On older language versions, call
/// them on this class instead, e.g. <c>GovUkAssert.ErrorSummaryContains(doc, "…")</c>.
/// </remarks>
[StackTraceHidden]
public static partial class GovUkAssert
{
    extension(Assert)
    {
        /// <summary>
        /// Verifies that a response has the expected status code and is HTML, and parses it.
        /// </summary>
        /// <remarks>
        /// The document's address is the request's URI, so relative links in it resolve as they would in a browser.
        /// </remarks>
        /// <param name="response">The response.</param>
        /// <param name="expectedStatusCode">The expected status code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The parsed document.</returns>
        /// <exception cref="GovUkAssertException">
        /// The status code isn't <paramref name="expectedStatusCode"/>, or the response isn't <c>text/html</c>.
        /// </exception>
        [StackTraceHidden]
        public static Task<IHtmlDocument> HtmlResponseAsync(
            HttpResponseMessage response,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(response);

            // Both checks only need the headers, so they fail before anything is read, with the caller's stack trace.
            if (response.StatusCode != expectedStatusCode)
            {
                var lines = new List<(string, string)>
                {
                    ("Expected", Failure.StatusCode(expectedStatusCode)),
                    ("Actual", Failure.StatusCode(response.StatusCode)),
                };

                if (response.GetRedirectLocation() is { } location)
                {
                    lines.Add(("Location", location.ToString()));
                }

                throw Failure.Create(nameof(HtmlResponseAsync), "Status code differs", lines);
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType;

            if (mediaType is not null && !mediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
            {
                throw Failure.Create(
                    nameof(HtmlResponseAsync),
                    "Not an HTML response",
                    [("Expected", "text/html"), ("Actual", mediaType)]);
            }

            return response.GetHtmlDocumentAsync(cancellationToken);
        }

        /// <summary>
        /// Verifies that a response redirects to the expected location.
        /// </summary>
        /// <remarks>
        /// Any of <c>301</c>, <c>302</c>, <c>303</c>, <c>307</c> and <c>308</c> is a redirect.
        /// A relative <paramref name="expectedLocation"/> is resolved against the request's URI, as the
        /// <c>Location</c> header is, so <c>/next</c> and <c>http://localhost/next</c> both match a redirect to
        /// <c>/next</c> from <c>http://localhost/</c>.
        /// </remarks>
        /// <param name="response">The response.</param>
        /// <param name="expectedLocation">The URL the response should redirect to, relative or absolute.</param>
        /// <returns>The redirect's location, resolved against the request's URI.</returns>
        /// <exception cref="GovUkAssertException">
        /// The response isn't a redirect, has no <c>Location</c> header, or redirects somewhere else.
        /// </exception>
        [StackTraceHidden]
        public static Uri RedirectsTo(HttpResponseMessage response, string expectedLocation)
        {
            ArgumentNullException.ThrowIfNull(response);
            ArgumentNullException.ThrowIfNull(expectedLocation);

            var requestUri = response.RequestMessage?.RequestUri;
            var expected = new Uri(expectedLocation, UriKind.RelativeOrAbsolute);

            if (!expected.IsAbsoluteUri && requestUri is { IsAbsoluteUri: true })
            {
                expected = new Uri(requestUri, expected);
            }

            if (!HttpResponseMessageExtensions.IsRedirect(response))
            {
                throw Failure.Create(
                    nameof(RedirectsTo),
                    "Not a redirect",
                    [("Expected", $"a redirect to {expected}"), ("Actual", Failure.StatusCode(response.StatusCode))]);
            }

            var actual = response.GetRedirectLocation() ?? throw Failure.Create(
                nameof(RedirectsTo),
                "No Location header",
                [("Expected", $"a redirect to {expected}"), ("Actual", $"{Failure.StatusCode(response.StatusCode)} with no Location header")]);

            // Without an absolute request URI to resolve against, relative locations can only be compared as written.
            var matches = expected.IsAbsoluteUri && actual.IsAbsoluteUri
                ? Uri.Compare(expected, actual, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.Ordinal) == 0
                : expected.IsAbsoluteUri == actual.IsAbsoluteUri && expected.OriginalString == actual.OriginalString;

            if (!matches)
            {
                throw Failure.Create(
                    nameof(RedirectsTo),
                    "Location differs",
                    [("Expected", expected.ToString()), ("Actual", actual.ToString())]);
            }

            return actual;
        }
    }
}
