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
    }
}
