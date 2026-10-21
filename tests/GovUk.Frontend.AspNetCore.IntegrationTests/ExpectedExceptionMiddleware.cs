namespace GovUk.Frontend.AspNetCore.IntegrationTests;

/// <summary>
/// Flags requests that a test expects to throw, so that the log records the developer exception
/// page writes for them can be dropped. A passing negative test otherwise leaves a <c>fail:</c>
/// entry in the test output that reads like a failure.
/// </summary>
/// <remarks>
/// Registered <em>before</em> <c>UseDeveloperExceptionPage()</c>, so the flag is set before that
/// page is entered and is unambiguously part of the execution context it logs from. Registering it
/// the other way round also happens to work, because the flag is assigned synchronously and so is
/// captured by the time the pipeline below suspends, but that depends on where the awaits fall
/// rather than on anything guaranteed.
/// </remarks>
public class ExpectedExceptionMiddleware(RequestDelegate next)
{
    private static readonly AsyncLocal<bool> Expected = new();

    /// <summary>
    /// The request header a test sets to declare that it expects the request to throw.
    /// </summary>
    public const string HeaderName = "X-Expected-Exception";

    /// <summary>
    /// The log category dropped for expected exceptions.
    /// </summary>
    public const string SuppressedCategory = "Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware";

    /// <summary>
    /// Whether the request being handled on this execution context expects to throw.
    /// </summary>
    public static bool IsExpected => Expected.Value;

    /// <summary>
    /// Log filter that drops the developer exception page's records for expected exceptions.
    /// Anything that hasn't declared itself expected is still logged, so a genuine failure keeps
    /// its stack trace.
    /// </summary>
    public static bool ShouldLog(string? category, LogLevel logLevel) =>
        !(IsExpected && category == SuppressedCategory);

    public Task InvokeAsync(HttpContext context)
    {
        // Assigned on every request rather than only the flagged ones. Connections are pooled and
        // an AsyncLocal set during one request can still be visible to the next one served on the
        // same connection, so leaving it alone here would let a flag leak onto a later request and
        // silently swallow a log record that matters
        Expected.Value = context.Request.Headers.ContainsKey(HeaderName);

        return next(context);
    }
}
