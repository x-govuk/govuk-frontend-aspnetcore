using System.Net;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class MiddlewareTests
{
    [Theory]
    [InlineData("govuk-frontend.min.css")]
    [InlineData("govuk-frontend.min.js")]
    public async Task HostsCompiledAssets(string fileName)
    {
        // Arrange
        await using var fixture = new MiddlewareTestFixture(services =>
        {
            services.Configure<GovUkFrontendOptions>(
                options => options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostCompiledFiles);
        });
        await fixture.InitializeAsync();

        var resolvedPath = $"{fixture.PathBase}/{fileName}?v={GovUkFrontendInfo.Version}";

        var request = new HttpRequestMessage(HttpMethod.Get, resolvedPath);

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(response.Headers.GetValues("Cache-Control"));
    }

    [Theory]
    [InlineData("images/favicon.ico")]
    [InlineData("images/govuk-crest.svg")]
    [InlineData("fonts/bold-affa96571d-v2.woff")]
    [InlineData("manifest.json")]
    [InlineData("rebrand/images/favicon.ico")]
    [InlineData("rebrand/manifest.json")]
    public async Task HostsStaticAssets(string fileName)
    {
        // Arrange
        await using var fixture = new MiddlewareTestFixture(services =>
        {
            services.Configure<GovUkFrontendOptions>(
                options => options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostAssets);
        });
        await fixture.InitializeAsync();

        var resolvedPath = $"{fixture.PathBase}{PageTemplateHelper.DefaultAssetsPath}/{fileName}";

        var request = new HttpRequestMessage(HttpMethod.Get, resolvedPath);

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RewritesAssetPathInCssFile()
    {
        // Arrange
        await using var fixture = new MiddlewareTestFixture(services =>
        {
            services.Configure<GovUkFrontendOptions>(options =>
                options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostAssets | FrontendPackageHostingOptions.HostCompiledFiles);
        });
        await fixture.InitializeAsync();

        var resolvedPath = $"{fixture.PathBase}/govuk-frontend.min.css";

        var request = new HttpRequestMessage(HttpMethod.Get, resolvedPath);

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        var css = await response.Content.ReadAsStringAsync();
        Assert.Contains($"{fixture.PathBase}{PageTemplateHelper.DefaultAssetsPath}", css);
    }
}

public class MiddlewareTestFixture : ServerFixture
{
    private readonly Action<IServiceCollection> _configureServices;

    public MiddlewareTestFixture(Action<IServiceCollection> configureServices)
    {
        _configureServices = configureServices;

        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public HttpClient HttpClient { get; }

    public PathString PathBase { get; } = new("/pathbase");

    public override Task DisposeAsync()
    {
        HttpClient.Dispose();
        return base.DisposeAsync();
    }

    protected override void Configure(IApplicationBuilder app)
    {
        app.UsePathBase(PathBase);

        app.UseDeveloperExceptionPage();

        app.UseGovUkFrontend();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddRazorPages();

        _configureServices.Invoke(services);
    }
}
