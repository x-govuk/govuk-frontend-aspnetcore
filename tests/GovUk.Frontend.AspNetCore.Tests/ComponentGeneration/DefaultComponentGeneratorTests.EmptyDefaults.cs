using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

// govuk-frontend applies these defaults with `default(value, true)`, which treats an empty value as unset.
public partial class DefaultComponentGeneratorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Footer_MetaWithoutVisuallyHiddenTitle_UsesDefaultTitle(string? visuallyHiddenTitle)
    {
        // Arrange
        var options = new FooterOptions
        {
            Meta = new FooterOptionsMeta { VisuallyHiddenTitle = visuallyHiddenTitle }
        };

        // Act
        var html = (await _componentGenerator.GenerateFooterAsync(options)).GetHtml();

        // Assert
        Assert.Contains("<h2 class=\"govuk-visually-hidden\">Support links</h2>", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CookieBanner_EmptyAriaLabel_UsesDefaultAriaLabel()
    {
        // Arrange
        var options = new CookieBannerOptions { AriaLabel = "" };

        // Act
        var html = (await _componentGenerator.GenerateCookieBannerAsync(options)).GetHtml();

        // Assert
        Assert.Contains("aria-label=\"Cookie banner\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Pagination_EmptyLandmarkLabel_UsesDefaultLandmarkLabel()
    {
        // Arrange
        var options = new PaginationOptions { LandmarkLabel = "" };

        // Act
        var html = (await _componentGenerator.GeneratePaginationAsync(options)).GetHtml();

        // Assert
        Assert.Contains("aria-label=\"Pagination\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task WarningText_EmptyIconFallbackText_UsesDefaultIconFallbackText()
    {
        // Arrange
        var options = new WarningTextOptions { Text = "Text", IconFallbackText = "" };

        // Act
        var html = (await _componentGenerator.GenerateWarningTextAsync(options)).GetHtml();

        // Assert
        Assert.Contains("<span class=\"govuk-visually-hidden\">Warning</span>", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task SkipLink_EmptyHref_UsesDefaultHref()
    {
        // Arrange
        var options = new SkipLinkOptions { Text = "Skip to main content", Href = "" };

        // Act
        var html = (await _componentGenerator.GenerateSkipLinkAsync(options)).GetHtml();

        // Assert
        Assert.Contains("href=\"#content\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Input_EmptyType_UsesDefaultType()
    {
        // Arrange
        var options = new InputOptions { Name = "name", Type = "" };

        // Act
        var html = (await _componentGenerator.GenerateInputAsync(options)).GetHtml();

        // Assert
        Assert.Contains("type=\"text\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Button_EmptyType_UsesDefaultType()
    {
        // Arrange
        var options = new ButtonOptions { Text = "Save", Type = "" };

        // Act
        var html = (await _componentGenerator.GenerateButtonAsync(options)).GetHtml();

        // Assert
        Assert.Contains("type=\"submit\"", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CookieBanner_ActionWithoutType_RendersButtonType(string? type)
    {
        // Arrange
        var options = new CookieBannerOptions
        {
            Messages =
            [
                new CookieBannerOptionsMessage
                {
                    Text = "Message",
                    Actions = [new CookieBannerOptionsMessageAction { Text = "Accept", Type = type }]
                }
            ]
        };

        // Act
        var html = (await _componentGenerator.GenerateCookieBannerAsync(options)).GetHtml();

        // Assert
        Assert.Contains("<button type=\"button\"", html, StringComparison.Ordinal);
    }
}
