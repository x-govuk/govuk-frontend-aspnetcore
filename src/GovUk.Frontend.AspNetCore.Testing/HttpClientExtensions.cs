namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// Extensions for submitting forms with an <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    // As HttpClientHandler.MaxAutomaticRedirections.
    internal const int MaxRedirects = 50;

    extension(HttpClient client)
    {
        /// <summary>
        /// Sends <paramref name="submission"/>.
        /// </summary>
        /// <param name="submission">The submission to send.</param>
        /// <param name="followRedirects">
        /// Whether to follow redirects, as a browser does, and return the final response. Intermediate responses are
        /// disposed.
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="followRedirects"/> is <see langword="true"/> and there were more than 50 redirects.
        /// </exception>
        public async Task<HttpResponseMessage> SubmitAsync(
            FormSubmission submission,
            bool followRedirects = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(submission);

            // Not disposed: its content is sent again if a 307 or 308 is followed, and the response keeps it as its
            // RequestMessage. The content is in memory.
#pragma warning disable CA2000
            var request = submission.ToRequest();
#pragma warning restore CA2000
            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!followRedirects)
            {
                return response;
            }

            for (var redirects = 0; HttpResponseMessageExtensions.IsRedirect(response); redirects++)
            {
                if (redirects == MaxRedirects)
                {
                    response.Dispose();
                    throw new InvalidOperationException($"Stopped after {MaxRedirects} redirects.");
                }

                using var redirect = response;
                response = await redirect.FollowRedirectAsync(client, cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        /// <summary>
        /// Requests the page at <paramref name="url"/> and gets the antiforgery token from its forms.
        /// </summary>
        /// <remarks>
        /// The token is paired with a cookie set by the same response, so the client must keep cookies, as clients from
        /// <c>WebApplicationFactory.CreateClient()</c> do.
        /// </remarks>
        /// <param name="url">The page to request.</param>
        /// <param name="fieldName">The name of the antiforgery field.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">
        /// The request failed, the response isn't HTML, or the page doesn't have exactly one distinct token.
        /// </exception>
        public Task<string> GetAntiforgeryTokenAsync(
            string url,
            string fieldName = FormSubmission.DefaultAntiforgeryFieldName,
            CancellationToken cancellationToken = default) =>
            client.GetAntiforgeryTokenAsync(
                new Uri(url ?? throw new ArgumentNullException(nameof(url)), UriKind.RelativeOrAbsolute),
                fieldName,
                cancellationToken);

        /// <summary>
        /// Requests the page at <paramref name="url"/> and gets the antiforgery token from its forms.
        /// </summary>
        /// <remarks>
        /// The token is paired with a cookie set by the same response, so the client must keep cookies, as clients from
        /// <c>WebApplicationFactory.CreateClient()</c> do.
        /// </remarks>
        /// <param name="url">The page to request.</param>
        /// <param name="fieldName">The name of the antiforgery field.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="InvalidOperationException">
        /// The request failed, the response isn't HTML, or the page doesn't have exactly one distinct token.
        /// </exception>
        public async Task<string> GetAntiforgeryTokenAsync(
            Uri url,
            string fieldName = FormSubmission.DefaultAntiforgeryFieldName,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(url);
            ArgumentNullException.ThrowIfNull(fieldName);

            using var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Requesting {response.RequestMessage?.RequestUri ?? url} for an antiforgery token returned status code " +
                    $"{(int)response.StatusCode} {response.StatusCode}.");
            }

            var doc = await response.GetHtmlDocumentAsync(cancellationToken).ConfigureAwait(false);

            // Every form on a page has the same token, so only distinct values count.
            var tokens = doc.HiddenInputs.Where(i => i.Name == fieldName).Select(i => i.Value).Distinct().ToArray();

            return tokens.Length switch
            {
                // Throws, saying which hidden inputs there are.
                0 => doc.GetHiddenInputValue(fieldName),
                1 => tokens[0],
                _ => throw new InvalidOperationException(
                    $"The page at {response.RequestMessage?.RequestUri ?? url} has {tokens.Length} different antiforgery tokens.")
            };
        }
    }
}
