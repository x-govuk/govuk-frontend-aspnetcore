namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class PageTemplateTests(EncodingsTestFixture fixture) : IClassFixture<EncodingsTestFixture>
{
    [Fact]
    public async Task BodyAndMainAttributes_AreAddedToTheElements()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/PageTemplate");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        var doc = await AssertEx.GetHtmlDocument(response);
        Assert.Equal("body-attr", doc.Body!.GetAttribute("data-body"));
        var main = Assert.Single(doc.GetElementsByTagName("main"));
        Assert.Equal("main-attr", main.GetAttribute("data-main"));

        // The tag helper binds this, so it must never reach the output. If the tag helper stops being
        // discovered - it's made internal, say - Razor treats it as a plain attribute and renders it
        // verbatim rather than failing the build.
        Assert.Null(doc.Body.GetAttribute("_govuk-additional-attributes"));
        Assert.Null(main.GetAttribute("_govuk-additional-attributes"));
    }

    [Fact]
    public async Task ComponentHasErrorOutsideOfAForm_PrependsErrorSummaryToMain()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/PageTemplate");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        var doc = await AssertEx.GetHtmlDocument(response);
        var errorSummary = Assert.Single(doc.GetElementsByClassName("govuk-error-summary"));
        var main = Assert.Single(doc.GetElementsByTagName("main"));
        Assert.Equal(main, errorSummary.ParentElement);
        Assert.Equal(errorSummary, main.FirstElementChild);
    }

    [Fact]
    public async Task ComponentHasErrorOutsideOfAForm_PrependsErrorToTitle()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/PageTemplate");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        // The <title> is rendered before the <main> element that the error summary is prepended to,
        // so the prefix can't depend on the summary having been rendered by the time the <title> is processed.
        var doc = await AssertEx.GetHtmlDocument(response);
        Assert.Equal("Error: Page template", doc.Title);
    }

    [Fact]
    public async Task ErrorSummaryPrependedToForm_PrependsErrorToTitleOnce()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/PageTemplateWithFormErrorSummary");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        var doc = await AssertEx.GetHtmlDocument(response);
        Assert.Single(doc.GetElementsByClassName("govuk-error-summary"));
        Assert.Equal("Error: Page template", doc.Title);
    }

    [Fact]
    public async Task NoErrors_DoesNotPrependErrorToTitle()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/PageTemplateWithoutErrors");

        // Act
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        var doc = await AssertEx.GetHtmlDocument(response);
        Assert.Empty(doc.GetElementsByClassName("govuk-error-summary"));
        Assert.Equal("Page template", doc.Title);
    }
}
