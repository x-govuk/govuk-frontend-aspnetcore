namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class AttributeBindingTests(AttributeBindingTestsFixture fixture) : IClassFixture<AttributeBindingTestsFixture>
{
    [Fact]
    public async Task NotificationBannerTitle_CustomId_RendersWithCustomId()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/CustomIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("id=\"custom-title-id\"", html);
    }

    [Fact]
    public async Task NotificationBannerTitle_CustomId_AriaLabelledByReferencesCustomId()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/CustomIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("aria-labelledby=\"custom-title-id\"", html);
    }

    [Fact]
    public async Task NotificationBannerTitle_DefaultId_RendersWithDefaultId()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/DefaultIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("id=\"govuk-notification-banner-title\"", html);
    }

    [Fact]
    public async Task NotificationBannerTitle_DefaultId_AriaLabelledByReferencesDefaultId()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/DefaultIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("aria-labelledby=\"govuk-notification-banner-title\"", html);
    }

    [Fact]
    public async Task NotificationBannerTitle_CustomHeadingLevel_RendersWithH3Element()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/CustomIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("<h3 class=\"govuk-notification-banner__title\"", html);
    }

    [Fact]
    public async Task NotificationBannerTitle_DefaultHeadingLevel_RendersWithH2Element()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/AttributeBindingTests/NotificationBanner/DefaultIdAndHeadingLevel");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var html = (await response.Content.ReadAsStringAsync()).Trim();

        Assert.Contains("<h2 class=\"govuk-notification-banner__title\"", html);
    }
}

public class AttributeBindingTestsFixture : ServerFixture
{
    public AttributeBindingTestsFixture()
    {
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public HttpClient HttpClient { get; }

    public override async ValueTask InitializeAsync()
    {
        // Override to skip Playwright initialization - we only need HttpClient
        var host = CreateHost();
        await host.StartAsync();
        SetHost(host);
    }

    public override ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return base.DisposeAsync();
    }

    protected override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddRazorPages();
    }

    private IHost CreateHost() => Host.CreateDefaultBuilder(args: [])
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder
                .UseUrls(BaseUrl)
                .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices((context, services) => ConfigureServices(services))
                .Configure(Configure);
        })
        .Build();

    private void SetHost(IHost host)
    {
        var field = typeof(ServerFixture).GetField("_host", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(this, host);
    }
}
