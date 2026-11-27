namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public static class HttpClientExtensions
{
    /// <summary>
    /// Issues a GET that's expected to throw, keeping the exception's stack trace out of the test
    /// output. See <see cref="ExpectedExceptionMiddleware"/>.
    /// </summary>
    public static Task<HttpResponseMessage> GetExpectingExceptionAsync(this HttpClient httpClient, string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add(ExpectedExceptionMiddleware.HeaderName, "true");
        return httpClient.SendAsync(request);
    }
}
