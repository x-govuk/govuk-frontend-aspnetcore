namespace GovUk.Frontend.AspNetCore.Tests;

public class GovUkFrontendPathsTests
{
    [Fact]
    public void Create_WithTheDefaultDirectories_MapsThemToTheDefaultUrls()
    {
        // Arrange
        var buildInfo = new GovUkFrontendBuildInfoAttribute("wwwroot/assets", "wwwroot", "wwwroot");

        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo);

        // Assert
        Assert.Equal("/assets", paths.Assets);
        Assert.Equal("", paths.JavaScript);
        Assert.Equal("", paths.Stylesheet);
        Assert.Equal("/govuk-frontend.min.js", paths.JavaScriptUrlPath);
        Assert.Equal("/govuk-frontend.min.css", paths.StylesheetUrlPath);
    }

    [Fact]
    public void Create_WithCustomDirectories_MapsThemRelativeToTheWebRoot()
    {
        // Arrange
        var buildInfo = new GovUkFrontendBuildInfoAttribute("wwwroot/govuk/assets", "wwwroot/govuk", "wwwroot/govuk");

        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo);

        // Assert
        Assert.Equal("/govuk/assets", paths.Assets);
        Assert.Equal("/govuk/govuk-frontend.min.js", paths.JavaScriptUrlPath);
        Assert.Equal("/govuk/govuk-frontend.min.css", paths.StylesheetUrlPath);
    }

    [Fact]
    public void Create_WithWindowsStyleSeparators_NormalizesThem()
    {
        // Arrange
        var buildInfo = new GovUkFrontendBuildInfoAttribute(@"wwwroot\govuk\assets", @"wwwroot\govuk", @"wwwroot\govuk");

        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo);

        // Assert
        Assert.Equal("/govuk/assets", paths.Assets);
        Assert.Equal("/govuk", paths.JavaScript);
        Assert.Equal("/govuk", paths.Stylesheet);
    }

    [Fact]
    public void Create_WithANonDefaultWebRoot_StripsThatInstead()
    {
        // Arrange
        var environment = CreateEnvironment(webRoot: "/app/public");
        var buildInfo = new GovUkFrontendBuildInfoAttribute("public/assets", "public", "public");

        // Act
        var paths = GovUkFrontendPaths.Create(environment, buildInfo);

        // Assert
        Assert.Equal("/assets", paths.Assets);
        Assert.Equal("", paths.Stylesheet);
    }

    [Fact]
    public void Create_WithADirectoryOutsideTheWebRoot_ReportsNoPath()
    {
        // Arrange - nothing outside the web root gets served, so there is no URL for it
        var buildInfo = new GovUkFrontendBuildInfoAttribute("lib/assets", "lib", "lib");

        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo);

        // Assert
        Assert.Null(paths.Assets);
        Assert.Null(paths.JavaScript);
        Assert.Null(paths.Stylesheet);
    }

    [Fact]
    public void Create_WithASiblingDirectoryWhoseNameStartsWithTheWebRoot_ReportsNoPath()
    {
        // Arrange
        var buildInfo = new GovUkFrontendBuildInfoAttribute("wwwrootstuff/assets", "wwwrootstuff", "wwwrootstuff");

        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo);

        // Assert
        Assert.Null(paths.Assets);
        Assert.Null(paths.JavaScript);
        Assert.Null(paths.Stylesheet);
    }

    [Fact]
    public void Create_WithAWebRootOutsideTheContentRoot_ReportsNoPaths()
    {
        // Arrange
        var environment = CreateEnvironment(contentRoot: "/app", webRoot: "/var/www");
        var buildInfo = new GovUkFrontendBuildInfoAttribute("wwwroot/assets", "wwwroot", "wwwroot");

        // Act
        var paths = GovUkFrontendPaths.Create(environment, buildInfo);

        // Assert
        Assert.Null(paths.Assets);
    }

    [Fact]
    public void Create_WithNoBuildInfo_ReportsNoPaths()
    {
        // Act
        var paths = GovUkFrontendPaths.Create(CreateEnvironment(), buildInfo: null);

        // Assert
        Assert.Null(paths.Assets);
        Assert.Null(paths.JavaScript);
        Assert.Null(paths.Stylesheet);
    }

    [Fact]
    public void Create_WithNoEnvironment_ReportsNoPaths()
    {
        // Arrange
        var buildInfo = new GovUkFrontendBuildInfoAttribute("wwwroot/assets", "wwwroot", "wwwroot");

        // Act
        var paths = GovUkFrontendPaths.Create(environment: null, buildInfo);

        // Assert
        Assert.Null(paths.Assets);
    }

    /// <summary>
    /// A file the build didn't restore still has to get a URL, since the project may be managing it
    /// itself; what it doesn't get is a claim that the build put it there.
    /// </summary>
    [Fact]
    public void UrlPaths_WhenNothingWasRestored_FallBackToTheDefaults()
    {
        // Act
        var paths = GovUkFrontendPaths.None;

        // Assert
        Assert.Equal("/assets", paths.AssetsUrlPath);
        Assert.Equal("/govuk-frontend.min.js", paths.JavaScriptUrlPath);
        Assert.Equal("/govuk-frontend.min.css", paths.StylesheetUrlPath);

        Assert.Null(paths.RestoredJavaScriptFile);
        Assert.Null(paths.RestoredStylesheetFile);
    }

    private static IWebHostEnvironment CreateEnvironment(string contentRoot = "/app", string? webRoot = null)
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(e => e.ContentRootPath).Returns(contentRoot);
        environment.SetupGet(e => e.WebRootPath).Returns(webRoot ?? Path.Combine(contentRoot, "wwwroot"));
        return environment.Object;
    }
}
