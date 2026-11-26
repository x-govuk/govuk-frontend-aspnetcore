using AngleSharp;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// Extensions for reading HTML from an <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
    extension(HttpResponseMessage response)
    {
        /// <summary>
        /// Parses the response's content as an HTML document.
        /// </summary>
        /// <remarks>
        /// The document's address is the request's URI, so relative links in it resolve as they would in a browser.
        /// The response's status code isn't checked.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The response's content type is not <c>text/html</c>.</exception>
        public async Task<IHtmlDocument> GetHtmlDocumentAsync(CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(response);

            var mediaType = response.Content.Headers.ContentType?.MediaType;

            if (mediaType is not null && !mediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Expected an HTML response but the content type was '{mediaType}' " +
                    $"(status code {(int)response.StatusCode} {response.StatusCode}).");
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var requestUri = response.RequestMessage?.RequestUri;

            // Not disposed: disposing the context disposes the document it opened.
#pragma warning disable CA2000
            var context = BrowsingContext.New(Configuration.Default);
#pragma warning restore CA2000

            var document = await context.OpenAsync(
                req =>
                {
                    req.Content(html);

                    if (requestUri is { IsAbsoluteUri: true })
                    {
                        req.Address(requestUri);
                    }
                },
                cancellationToken).ConfigureAwait(false);

            return (IHtmlDocument)document;
        }

        /// <summary>
        /// Gets the response's <c>Location</c> header, resolved against the request's URI, or <see langword="null"/> if
        /// it doesn't have one.
        /// </summary>
        public Uri? GetRedirectLocation()
        {
            ArgumentNullException.ThrowIfNull(response);

            var location = response.Headers.Location;

            if (location is null)
            {
                return null;
            }

            return !location.IsAbsoluteUri && response.RequestMessage?.RequestUri is { IsAbsoluteUri: true } requestUri
                ? new Uri(requestUri, location)
                : location;
        }

        /// <summary>
        /// Follows the redirect in this response, as a browser does.
        /// </summary>
        /// <remarks>
        /// A <c>301</c>, <c>302</c> or <c>303</c> is followed with a <c>GET</c>. A <c>307</c> or <c>308</c> repeats the
        /// request's method and content. One redirect is followed; call this again on the result to follow another.
        /// </remarks>
        /// <param name="client">The client to send the request with.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">
        /// The response isn't a redirect, or doesn't have a <c>Location</c> header.
        /// </exception>
        public Task<HttpResponseMessage> FollowRedirectAsync(HttpClient client, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(response);
            ArgumentNullException.ThrowIfNull(client);

            var statusCode = (int)response.StatusCode;

            if (!IsRedirect(response))
            {
                throw new InvalidOperationException(
                    $"Expected a redirect but the status code was {statusCode} {response.StatusCode}.");
            }

            var location = response.GetRedirectLocation() ?? throw new InvalidOperationException(
                $"The redirect ({statusCode} {response.StatusCode}) doesn't have a Location header.");

            var keepMethod = statusCode is 307 or 308;
            var original = response.RequestMessage;

            // Not disposed: disposing a request disposes its content, which a 307 or 308 shares with the original
            // request and which a further 307 or 308 would send again. The response keeps it as its RequestMessage.
#pragma warning disable CA2000
            var request = keepMethod && original is not null
                ? new HttpRequestMessage(original.Method, location) { Content = original.Content }
                : new HttpRequestMessage(HttpMethod.Get, location);
#pragma warning restore CA2000

            return client.SendAsync(request, cancellationToken);
        }
    }

    internal static bool IsRedirect(HttpResponseMessage response) => (int)response.StatusCode is 301 or 302 or 303 or 307 or 308;
}
