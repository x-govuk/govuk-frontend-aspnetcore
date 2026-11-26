using System.Net;
using AngleSharp.Dom;
using GovUk.Frontend.AspNetCore.Localization;
using GovUk.Frontend.AspNetCore.Tests.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.TestHost;

namespace GovUk.Frontend.AspNetCore.Tests;

public class ErrorPagesTests
{
    private const string HtmlAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

    [Fact]
    public async Task PageNotFound_HtmlRequest_RendersPageNotFoundPage()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/does-not-exist", HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Page not found", doc.Title);
        Assert.Equal("Page not found", GetHeading(doc));
        Assert.Contains("govuk-template", doc.DocumentElement.ClassList);
        Assert.Contains("govuk-main-wrapper--l", doc.QuerySelector("main")!.ClassList);
        Assert.Equal(
            ["If you typed the web address, check it is correct.", "If you pasted the web address, check you copied the entire address."],
            doc.QuerySelectorAll("main p.govuk-body").Select(p => p.TextContent.Trim()));
    }

    [Fact]
    public async Task PageNotFound_NonHtmlRequest_ReturnsEmptyResponse()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/does-not-exist", "*/*");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task PageNotFound_ApiController_ReturnsApiResponse()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/error-pages-api/not-found", HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain("govuk-heading-l", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task PageNotFound_HandlePageNotFoundFalse_ReturnsEmptyResponse()
    {
        // Arrange
        using var host = await CreateHostAsync(options => options.HandlePageNotFound = false);

        // Act
        using var response = await GetAsync(host, "/does-not-exist", HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Exception_HtmlRequest_RendersProblemWithTheServicePage()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/throw", HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Sorry, there is a problem with the service", GetHeading(doc));
        Assert.Equal("Try again later.", doc.QuerySelector("main p.govuk-body")!.TextContent.Trim());
        Assert.DoesNotContain("Boom", doc.DocumentElement.OuterHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Exception_ContentPartialExists_RendersPartialWithModel()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/throw", HtmlAccept);

        // Assert
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Contact the helpline (status 500).", doc.GetElementById("service-content")?.TextContent);
    }

    [Fact]
    public async Task Exception_ApiController_ReturnsEmptyResponse()
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, "/error-pages-api/throw", HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Exception_HandleExceptionsFalse_IsNotHandled()
    {
        // Arrange
        using var host = await CreateHostAsync(options => options.HandleExceptions = false);

        // Act
        var ex = await Record.ExceptionAsync(() => GetAsync(host, "/throw", HtmlAccept));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Boom", ex.Message);
    }

    [Theory]
    [InlineData("/does-not-exist", HttpStatusCode.NotFound, "Page not found")]
    [InlineData("/throw", HttpStatusCode.InternalServerError, "Sorry, there is a problem with the service")]
    public async Task HtmlRequest_ErrorPagesBeforeUseRouting_RendersPage(string path, HttpStatusCode expectedStatusCode, string expectedHeading)
    {
        // Arrange
        using var host = await CreateHostAsync(errorPagesBeforeRouting: true);

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal(expectedHeading, GetHeading(doc));
    }

    [Theory]
    [InlineData("/error-pages-api/not-found", HttpStatusCode.NotFound)]
    [InlineData("/error-pages-api/throw", HttpStatusCode.InternalServerError)]
    public async Task ApiController_ErrorPagesBeforeUseRouting_DoesNotRenderPage(string path, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        using var host = await CreateHostAsync(errorPagesBeforeRouting: true);

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.DoesNotContain("govuk-heading-l", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Layout_Set_UsesLayout()
    {
        // Arrange
        using var host = await CreateHostAsync(options => options.Layout = "_TestErrorPageLayout");

        // Act
        using var response = await GetAsync(host, "/does-not-exist", HtmlAccept);

        // Assert
        var doc = await response.GetHtmlDocument();
        Assert.NotNull(doc.GetElementById("test-layout"));
        Assert.Equal("Page not found – Test service", doc.Title);
        Assert.Equal("Page not found", GetHeading(doc));
    }

    [Fact]
    public async Task Localizer_ProvidesContent_UsesLocalizedContent()
    {
        // Arrange
        var localizer = DelegateLocalizer.ForName(GovUkFrontendResourceNames.PageNotFoundPageHeading, "Heb ddod o hyd i’r dudalen");

        using var host = await CreateHostAsync(configureServices: services => services.AddGovUkFrontendLocalization(_ => localizer));

        // Act
        using var response = await GetAsync(host, "/does-not-exist", HtmlAccept);

        // Assert
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Heb ddod o hyd i’r dudalen", doc.Title);
        Assert.Equal("Heb ddod o hyd i’r dudalen", GetHeading(doc));
        Assert.Equal("If you typed the web address, check it is correct.", doc.QuerySelector("main p.govuk-body")!.TextContent.Trim());
    }

    /// <summary>
    /// The error page resource names, with the path that renders each one. Listed as covered by
    /// <c>ComponentLocalizationTests.EveryResourceName_IsCoveredByATest</c>.
    /// </summary>
    public static TheoryData<string, string> ErrorPageResourceNames => new()
    {
        { GovUkFrontendResourceNames.PageNotFoundPageHeading, "/does-not-exist" },
        { GovUkFrontendResourceNames.PageNotFoundPageTypedAddressText, "/does-not-exist" },
        { GovUkFrontendResourceNames.PageNotFoundPagePastedAddressText, "/does-not-exist" },
        { GovUkFrontendResourceNames.ProblemWithTheServicePageHeading, "/throw" },
        { GovUkFrontendResourceNames.ProblemWithTheServicePageTryAgainText, "/throw" },
    };

    [Theory]
    [MemberData(nameof(ErrorPageResourceNames))]
    public async Task Localizer_ProvidesContentForName_ContentIsRendered(string name, string path)
    {
        // Arrange
        var localizer = DelegateLocalizer.ForName(name, "Localized content");

        using var host = await CreateHostAsync(configureServices: services => services.AddGovUkFrontendLocalization(_ => localizer));

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Localized content", html, StringComparison.Ordinal);
    }

    [Fact]
    public void UseGovUkFrontendErrorPages_WithoutViewServices_Throws()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddGovUkFrontend();
        using var serviceProvider = services.BuildServiceProvider();
        var app = new ApplicationBuilder(serviceProvider);

        // Act
        var ex = Record.Exception(() => app.UseGovUkFrontendErrorPages());

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
    }

    [Theory]
    [InlineData("/error-pages-mvc/not-found")]
    [InlineData("/error-pages-page?result=not-found")]
    public async Task GovUkPageNotFound_RendersPageNotFoundPage(string path)
    {
        // Arrange
        // Without the middleware, so an unmatched route would give an empty 404 rather than the same page
        using var host = await CreateHostAsync(useErrorPages: false);

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Page not found", GetHeading(doc));
        Assert.Contains("govuk-template", doc.DocumentElement.ClassList);
    }

    [Theory]
    [InlineData("/error-pages-mvc/forbidden")]
    [InlineData("/error-pages-page?result=forbidden")]
    public async Task GovUkProblemWithTheService_WithStatusCode_RendersPageWithStatusCode(string path)
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Sorry, there is a problem with the service", GetHeading(doc));
        Assert.Equal("Contact the helpline (status 403).", doc.GetElementById("service-content")?.TextContent);
    }

    [Theory]
    [InlineData("/error-pages-mvc/problem")]
    [InlineData("/error-pages-page?result=problem")]
    public async Task GovUkProblemWithTheService_DefaultStatusCode_Returns500(string path)
    {
        // Arrange
        using var host = await CreateHostAsync();

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Sorry, there is a problem with the service", GetHeading(doc));
    }

    [Theory]
    [InlineData("/error-pages-mvc/forbidden")]
    [InlineData("/error-pages-page?result=forbidden")]
    public async Task GovUkErrorPageResult_WithLayoutOption_UsesLayout(string path)
    {
        // Arrange
        using var host = await CreateHostAsync(options => options.Layout = "_TestErrorPageLayout");

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        var doc = await response.GetHtmlDocument();
        Assert.NotNull(doc.GetElementById("test-layout"));
    }

    [Theory]
    [InlineData("/error-pages-mvc/forbidden")]
    [InlineData("/error-pages-page?result=forbidden")]
    public async Task GovUkErrorPageResult_WithoutErrorPagesMiddleware_RendersPage(string path)
    {
        // Arrange
        using var host = await CreateHostAsync(useErrorPages: false);

        // Act
        using var response = await GetAsync(host, path, HtmlAccept);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var doc = await response.GetHtmlDocument();
        Assert.Equal("Sorry, there is a problem with the service", GetHeading(doc));
    }

    private static string? GetHeading(IDocument doc) => doc.QuerySelector("h1.govuk-heading-l")?.TextContent.Trim();

    private static Task<HttpResponseMessage> GetAsync(IHost host, string path, string accept)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.TryAddWithoutValidation("Accept", accept);
        return host.GetTestClient().SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static async Task<IHost> CreateHostAsync(
        Action<GovUkFrontendErrorPageOptions>? configureOptions = null,
        Action<IServiceCollection>? configureServices = null,
        bool errorPagesBeforeRouting = false,
        bool useErrorPages = true)
    {
        var host = new HostBuilder()
            .ConfigureWebHost(webBuilder => webBuilder
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddControllersWithViews()
                        .AddApplicationPart(typeof(ErrorPagesTests).Assembly);
                    services.AddRazorPages();
                    services.AddGovUkFrontend();

                    if (configureOptions is not null)
                    {
                        services.Configure(configureOptions);
                    }

                    configureServices?.Invoke(services);
                })
                .Configure(app =>
                {
                    if (!useErrorPages)
                    {
                        app.UseRouting();
                    }
                    else if (errorPagesBeforeRouting)
                    {
                        app.UseGovUkFrontendErrorPages();
                        app.UseRouting();
                    }
                    else
                    {
                        app.UseRouting();
                        app.UseGovUkFrontendErrorPages();
                    }

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapRazorPages();
                        endpoints.MapGet("/throw", IResult () => throw new InvalidOperationException("Boom"));
                    });
                }))
            .Build();

        await host.StartAsync(TestContext.Current.CancellationToken);
        return host;
    }
}

[ApiController]
[Route("error-pages-api")]
public class ErrorPagesApiController : ControllerBase
{
    [HttpGet("not-found")]
    public IActionResult GetNotFound() => NotFound();

    [HttpGet("throw")]
    public IActionResult Throw() => throw new InvalidOperationException("Boom");
}

[Route("error-pages-mvc")]
public class ErrorPagesMvcController : Controller
{
    [HttpGet("not-found")]
    public IActionResult PageNotFound() => this.GovUkPageNotFound();

    [HttpGet("forbidden")]
    public IActionResult Forbidden() => this.GovUkProblemWithTheService(StatusCodes.Status403Forbidden);

    [HttpGet("problem")]
    public IActionResult Problem() => this.GovUkProblemWithTheService();
}

public class ErrorPagesTestPageModel : PageModel
{
    public IActionResult OnGet(string result) => result switch
    {
        "not-found" => this.GovUkPageNotFound(),
        "forbidden" => this.GovUkProblemWithTheService(StatusCodes.Status403Forbidden),
        _ => this.GovUkProblemWithTheService()
    };
}
