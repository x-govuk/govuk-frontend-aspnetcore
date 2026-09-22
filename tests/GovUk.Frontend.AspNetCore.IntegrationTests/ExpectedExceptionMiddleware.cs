namespace GovUk.Frontend.AspNetCore.IntegrationTests;

/// <summary>
/// Handles exceptions for requests that a test expects to throw, so that the developer exception
/// page never sees them and never writes their stack traces to the test output. A passing negative
/// test otherwise leaves a <c>fail:</c> entry in the log that reads like a failure.
/// </summary>
/// <remarks>
/// This must be registered <em>after</em> <c>UseDeveloperExceptionPage()</c> so that it's the inner
/// of the two and catches first. Only requests carrying <see cref="HeaderName"/> are handled here;
/// an unexpected exception anywhere else still reaches the developer exception page and is still
/// logged.
/// </remarks>
public class ExpectedExceptionMiddleware(RequestDelegate next)
{
    /// <summary>
    /// The request header a test sets to declare that it expects the request to throw.
    /// </summary>
    public const string HeaderName = "X-Expected-Exception";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey(HeaderName))
        {
            await next(context);
            return;
        }

        try
        {
            await next(context);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            // Mirrors what the developer exception page would have returned, minus the logging and
            // the HTML; tests assert on the status code and on the exception message appearing in
            // the body
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(ex.ToString());
        }
    }
}
