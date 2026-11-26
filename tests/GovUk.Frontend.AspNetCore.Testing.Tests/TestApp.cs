using Microsoft.AspNetCore.TestHost;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

/// <summary>
/// Hosts the Razor Pages in <c>Pages/</c> on a <see cref="TestServer"/>, so tests can run over markup rendered by the
/// tag helpers, bound to a model as an app would.
/// </summary>
public sealed class TestApp : IAsyncLifetime
{
    private WebApplication? _app;

    public HttpClient CreateClient() => _app!.GetTestClient();

    /// <summary>
    /// Creates a client that keeps cookies, as <c>WebApplicationFactory</c>'s does, so antiforgery tokens validate.
    /// </summary>
    public HttpClient CreateClientWithCookies()
    {
        var server = _app!.GetTestServer();
        return new HttpClient(new CookieHandler(server.CreateHandler())) { BaseAddress = server.BaseAddress };
    }

    public async ValueTask InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.Services.AddGovUkFrontend();
        builder.Services.AddRazorPages().AddApplicationPart(typeof(TestApp).Assembly);

        _app = builder.Build();
        _app.UseRouting();
        _app.MapRazorPages();

        // Echoes the request, for checking what a submission or redirect sent.
        _app.Map("/Echo/{**path}", async (HttpRequest request) =>
        {
            using var reader = new StreamReader(request.Body);
            var body = await reader.ReadToEndAsync();
            return Results.Text($"{request.Method} {request.Path}{request.QueryString} {body}".TrimEnd());
        });

        // Redirects with the given status code to /Echo/{path}, or to /Redirect/{code}/{n-1}/{path} while n > 0.
        _app.Map("/Redirect/{code:int}/{remaining:int}/{**path}", (HttpResponse response, int code, int remaining, string? path) =>
        {
            response.StatusCode = code;
            response.Headers.Location = remaining > 0 ? $"/Redirect/{code}/{remaining - 1}/{path}" : $"/Echo/{path}";
        });

        _app.Map("/NoLocation", (HttpResponse response) => response.StatusCode = StatusCodes.Status302Found);

        await _app.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}

internal sealed class CookieHandler(HttpMessageHandler inner) : DelegatingHandler(inner)
{
    private readonly System.Net.CookieContainer _cookies = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        var header = _cookies.GetCookieHeader(uri);

        if (header.Length > 0)
        {
            request.Headers.Add("Cookie", header);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var setCookie in setCookies)
            {
                _cookies.SetCookies(uri, setCookie);
            }
        }

        return response;
    }
}
