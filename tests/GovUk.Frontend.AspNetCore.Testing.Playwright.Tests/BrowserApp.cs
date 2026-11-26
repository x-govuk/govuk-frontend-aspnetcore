using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.FileProviders;

[assembly: AssemblyFixture(typeof(GovUk.Frontend.AspNetCore.Testing.Playwright.Tests.BrowserApp))]

namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

/// <summary>
/// Hosts the Razor Pages in <c>Pages/</c> on Kestrel, with govuk-frontend's CSS and JavaScript, and launches a
/// browser to test them in.
/// </summary>
public sealed class BrowserApp : IAsyncLifetime
{
    private WebApplication? _app;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private string? _baseUrl;

    /// <summary>
    /// Opens a new page in its own browser context, with relative URLs resolved against the app.
    /// </summary>
    public async Task<IPage> NewPageAsync()
    {
        var page = await _browser!.NewPageAsync(new BrowserNewPageOptions { BaseURL = _baseUrl });
        // Keep failing tests quick: everything here renders immediately.
        page.SetDefaultTimeout(5_000);
        return page;
    }

    public async ValueTask InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.Services.AddGovUkFrontend();
        builder.Services.AddRazorPages().AddApplicationPart(typeof(BrowserApp).Assembly);

        _app = builder.Build();

        var govUkFrontendLibrary = typeof(BrowserApp).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == "GovUkFrontendLibrary")
            .Value!;
        _app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(govUkFrontendLibrary) });

        _app.UseGovUkFrontend();
        _app.UseRouting();
        _app.MapRazorPages();

        await _app.StartAsync();

        _baseUrl = _app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!
            .Addresses.Single();

        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = !Debugger.IsAttached });
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
        }

        _playwright?.Dispose();

        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}
