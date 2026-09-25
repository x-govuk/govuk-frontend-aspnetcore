using System.Diagnostics;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class ServerFixture : IAsyncLifetime
{
    // TFM specific ports so tests can run concurrently
#if NET8_0
    public const string BaseUrl = "http://localhost:55348";
#elif NET9_0
    public const string BaseUrl = "http://localhost:55349";
#elif NET10_0
    public const string BaseUrl = "http://localhost:55350";
#endif

    private IHost? _host;
    private IPlaywright? _playright;
    private bool _disposed = false;

    public IBrowser? Browser { get; private set; }

    public IServiceProvider Services => _host!.Services;

    public virtual async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playright?.Dispose();

        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }

    public virtual async ValueTask InitializeAsync()
    {
        await StartHostAsync();

        _playright = await Playwright.CreateAsync();
        Browser = await _playright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions() { Headless = !Debugger.IsAttached });
    }

    /// <summary>
    /// Starts the host without initializing Playwright, for fixtures that don't need a browser.
    /// </summary>
    protected async Task StartHostAsync()
    {
        _host = CreateHost();
        await _host.StartAsync();
    }

    protected virtual void Configure(IApplicationBuilder app)
    {
        // Registered before the developer exception page so the flag is already set by the time
        // that page runs, rather than relying on how execution context flows back out of the
        // pipeline it wraps
        app.UseMiddleware<ExpectedExceptionMiddleware>();

        app.UseDeveloperExceptionPage();

        app.UseGovUkFrontend();

        app.UseRouting();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddGovUkFrontend();
    }

    private IHost CreateHost() => Host.CreateDefaultBuilder(args: [])
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                .UseUrls(BaseUrl)
                .ConfigureLogging(logging => logging
                    .SetMinimumLevel(LogLevel.Warning)
                    .AddFilter(ExpectedExceptionMiddleware.ShouldLog))
                .ConfigureServices((context, services) => ConfigureServices(services))
                .Configure(Configure);
        })
        .Build();
}
