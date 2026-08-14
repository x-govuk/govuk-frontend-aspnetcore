using System.Net;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class ShortTagNamesTests(ShortTagNamesTestsFixture fixture) : IClassFixture<ShortTagNamesTestsFixture>
{
    [Theory]
    [InlineData("Checkboxes", "short", "long")]
    [InlineData("Checkboxes", "short-error-message", "long-error-message")]
    [InlineData("Radios", "short", "long")]
    [InlineData("Radios", "short-error-message", "long-error-message")]
    [InlineData("DateInput", "short", "long")]
    [InlineData("DateInput", "short-error-message", "long-error-message")]
    [InlineData("TextInput", "short", "long")]
    [InlineData("TextInput", "short-error-message", "long-error-message")]
    [InlineData("TextArea", "short", "long")]
    [InlineData("TextArea", "short-error-message", "long-error-message")]
    [InlineData("FileUpload", "short", "long")]
    [InlineData("FileUpload", "short-error-message", "long-error-message")]
    [InlineData("PasswordInput", "short", "long")]
    [InlineData("PasswordInput", "short-error-message", "long-error-message")]
    [InlineData("Select", "short", "long")]
    [InlineData("Select", "short-error-message", "long-error-message")]
    [InlineData("CharacterCount", "short", "long")]
    [InlineData("CharacterCount", "short-error-message", "long-error-message")]
    [InlineData("Accordion", "short", "long", ".govuk-accordion__section")]
    [InlineData("Breadcrumbs", "short", "long", ".govuk-breadcrumbs__list-item")]
    [InlineData("ServiceNavigation", "short", "long", ".govuk-service-navigation__item")]
    [InlineData("Fieldset", "short", "long")]
    [InlineData("GenericHeader", "short", "long", ".govuk-generic-header__homepage-link")]
    [InlineData("Footer", "short", "long", ".govuk-footer__link")]
    [InlineData("NotificationBanner", "short", "long", ".govuk-notification-banner__title")]
    public async Task ShortTagNames_GenerateTheSameMarkupAsTheGovUkPrefixedNames(
        string component,
        string shortTestId,
        string longTestId,
        // What the component generates from the children written with the short names; the default
        // covers the components built around a form field
        string contentSelector = "legend, label, input, textarea, select")
    {
        // Act
        var document = await GetDocumentAsync(component);

        // Assert
        var shortContainer = GetContainer(document, shortTestId);
        var longContainer = GetContainer(document, longTestId);

        // Guards against both containers being empty, which would make the comparison vacuous
        Assert.NotEmpty(shortContainer.QuerySelectorAll(contentSelector));

        // An unrecognised element is written out as-is, so a short name that never bound to a tag
        // helper would show up here as, say, a <radios-item> element outside the fieldset
        Assert.Equal(longContainer.InnerHtml, shortContainer.InnerHtml);
    }

    [Fact]
    public async Task ServiceNavigation_MixingShortAndGovUkPrefixedTagNames_Throws()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/ShortTagNamesTests/ServiceNavigationMixed");
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        Assert.Contains(
            "<nav> cannot be used alongside <govuk-service-navigation-start>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Footer_MixingShortAndGovUkPrefixedTagNames_Throws()
    {
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/ShortTagNamesTests/FooterMixed");
        var response = await fixture.HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        Assert.Contains(
            "<copyright> cannot be used alongside <govuk-footer-content-licence>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            await response.Content.ReadAsStringAsync());
    }

    private async Task<IDocument> GetDocumentAsync(string component)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/ShortTagNamesTests/{component}");
        var response = await fixture.HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.GetHtmlDocument();
    }

    private static IElement GetContainer(IDocument document, string testId) =>
        document.QuerySelector($"[data-testid='{testId}']") ??
            throw new InvalidOperationException($"No element found with the test ID '{testId}'.");
}

public class ShortTagNamesTestsFixture : ServerFixture
{
    public ShortTagNamesTestsFixture()
    {
        HttpClient = new HttpClient() { BaseAddress = new Uri(BaseUrl) };
    }

    public HttpClient HttpClient { get; }

    public override async ValueTask InitializeAsync()
    {
        // No browser needed for these tests
        await StartHostAsync();
    }

    public override ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return base.DisposeAsync();
    }

    protected override void Configure(IApplicationBuilder app)
    {
        base.Configure(app);

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services
            .AddMvc()
            .AddRazorOptions(options => options.ViewLocationFormats.Add("/ShortTagNamesTestsViews/{0}.cshtml"));
    }
}

[Route("ShortTagNamesTests")]
public class ShortTagNamesTestsController : Controller
{
    [HttpGet("Checkboxes")]
    public IActionResult GetCheckboxes() => View("Checkboxes", new ShortTagNamesTestsModel());

    [HttpGet("Radios")]
    public IActionResult GetRadios() => View("Radios", new ShortTagNamesTestsModel());

    [HttpGet("DateInput")]
    public IActionResult GetDateInput() => View("DateInput", new ShortTagNamesTestsModel());

    [HttpGet("TextInput")]
    public IActionResult GetTextInput() => View("TextInput", new ShortTagNamesTestsModel());

    [HttpGet("TextArea")]
    public IActionResult GetTextArea() => View("TextArea", new ShortTagNamesTestsModel());

    [HttpGet("FileUpload")]
    public IActionResult GetFileUpload() => View("FileUpload", new ShortTagNamesTestsModel());

    [HttpGet("PasswordInput")]
    public IActionResult GetPasswordInput() => View("PasswordInput", new ShortTagNamesTestsModel());

    [HttpGet("Select")]
    public IActionResult GetSelect() => View("Select", new ShortTagNamesTestsModel());

    [HttpGet("CharacterCount")]
    public IActionResult GetCharacterCount() => View("CharacterCount", new ShortTagNamesTestsModel());

    [HttpGet("Accordion")]
    public IActionResult GetAccordion() => View("Accordion", new ShortTagNamesTestsModel());

    [HttpGet("Breadcrumbs")]
    public IActionResult GetBreadcrumbs() => View("Breadcrumbs", new ShortTagNamesTestsModel());

    [HttpGet("ServiceNavigation")]
    public IActionResult GetServiceNavigation() => View("ServiceNavigation", new ShortTagNamesTestsModel());

    [HttpGet("Fieldset")]
    public IActionResult GetFieldset() => View("Fieldset", new ShortTagNamesTestsModel());

    [HttpGet("GenericHeader")]
    public IActionResult GetGenericHeader() => View("GenericHeader", new ShortTagNamesTestsModel());

    [HttpGet("Footer")]
    public IActionResult GetFooter() => View("Footer", new ShortTagNamesTestsModel());

    [HttpGet("FooterMixed")]
    public IActionResult GetFooterMixed() => View("FooterMixed", new ShortTagNamesTestsModel());

    [HttpGet("NotificationBanner")]
    public IActionResult GetNotificationBanner() => View("NotificationBanner", new ShortTagNamesTestsModel());

    [HttpGet("ServiceNavigationMixed")]
    public IActionResult GetServiceNavigationMixed() => View("ServiceNavigationMixed", new ShortTagNamesTestsModel());
}

public class ShortTagNamesTestsModel
{
    public string[]? ContactPreferences { get; set; }

    public string? ContactPreference { get; set; }

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobilePhoneNumber { get; set; }

    [DateInput(ErrorMessagePrefix = "Your date of birth")]
    public DateOnly? DateOfBirth { get; set; }

    public string? MoreDetail { get; set; }

    public IFormFile? Evidence { get; set; }

    public string? Password { get; set; }

    public string? Sort { get; set; }
}
