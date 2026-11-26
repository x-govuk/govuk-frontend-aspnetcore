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
