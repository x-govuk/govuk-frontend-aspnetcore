using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

[assembly: AssemblyFixture(typeof(Samples.ApplyForALibraryCard.BrowserTests.JourneyApp))]

namespace Samples.ApplyForALibraryCard.BrowserTests;

/// <summary>
/// Runs the app on Kestrel, so a browser can reach it. Shared by every test.
/// </summary>
public sealed class JourneyApp : IAsyncDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();

    public JourneyApp()
    {
        _factory.UseKestrel(0);
        _factory.StartServer();

        BaseUrl = _factory.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!
            .Addresses.First();
    }

    /// <summary>
    /// The address the app is listening on.
    /// </summary>
    public string BaseUrl { get; }

    public ValueTask DisposeAsync() => _factory.DisposeAsync();
}
