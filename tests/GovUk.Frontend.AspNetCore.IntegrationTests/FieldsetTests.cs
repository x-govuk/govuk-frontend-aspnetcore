using System.ComponentModel.DataAnnotations;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class FieldsetTests(FieldsetTestsFixture fixture) : IClassFixture<FieldsetTestsFixture>
{
    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task ExplicitFieldsetAndLegend_GeneratesFieldsetFromBothElements(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "explicit-fieldset");

        // Assert
        Assert.NotNull(fieldset);
        Assert.Contains("explicit-fieldset-class", fieldset.ClassList);
        Assert.Equal("explicit", fieldset.GetAttribute("data-fieldset"));

        var legend = fieldset.QuerySelector("legend");
        Assert.NotNull(legend);
        Assert.Equal("Explicit legend", legend.TextContent.Trim());
        Assert.Contains("explicit-legend-class", legend.ClassList);
        Assert.Equal("explicit", legend.GetAttribute("data-legend"));
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task ExplicitFieldsetWithoutLegendElement_GeneratesLegendFromModelMetadata(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "explicit-fieldset-legend-from-model");

        // Assert
        Assert.NotNull(fieldset);
        Assert.Equal(GetExpectedDisplayName(component), fieldset.QuerySelector("legend")?.TextContent.Trim());
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task LegendElementWithoutFieldsetElement_GeneratesFieldset(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "implicit-fieldset");

        // Assert
        Assert.NotNull(fieldset);

        var legend = fieldset.QuerySelector("legend");
        Assert.NotNull(legend);
        Assert.Equal("Implicit legend", legend.TextContent.Trim());
        Assert.Contains("implicit-legend-class", legend.ClassList);
        Assert.Equal("implicit", legend.GetAttribute("data-legend"));
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task LegendElementWithoutFieldsetElement_AddsFieldsetAttributesToGeneratedFieldset(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "implicit-fieldset-with-attributes");

        // Assert
        Assert.NotNull(fieldset);
        Assert.Contains("generated-fieldset-class", fieldset.ClassList);
        Assert.Equal("generated", fieldset.GetAttribute("data-fieldset"));

        var legend = fieldset.QuerySelector("legend");
        Assert.NotNull(legend);
        Assert.Equal("Implicit legend", legend.TextContent.Trim());

        // The 'legend-*' attributes are combined with those on the legend element
        Assert.Contains("root-legend-class", legend.ClassList);
        Assert.Contains("element-legend-class", legend.ClassList);
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task FieldsetAttributeWithFor_GeneratesFieldsetWithLegendFromModelMetadata(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "fieldset-from-model-metadata");

        // Assert
        Assert.NotNull(fieldset);
        Assert.Equal(GetExpectedDisplayName(component), fieldset.QuerySelector("legend")?.TextContent.Trim());
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task LegendAttributesWithFor_GeneratesFieldsetWithLegendFromModelMetadata(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "fieldset-from-model-metadata-with-legend-class");

        // Assert
        Assert.NotNull(fieldset);

        var legend = fieldset.QuerySelector("legend");
        Assert.NotNull(legend);
        Assert.Equal(GetExpectedDisplayName(component), legend.TextContent.Trim());
        Assert.Contains("govuk-fieldset__legend--l", legend.ClassList);
        Assert.Equal("generated", legend.GetAttribute("data-legend"));

        // 'legend-is-page-heading' is bound to its own attribute, not added to the legend's attributes
        Assert.Null(legend.GetAttribute("is-page-heading"));
        Assert.NotNull(legend.QuerySelector("h1.govuk-fieldset__heading"));
    }

    [Theory]
    [InlineData("Checkboxes")]
    [InlineData("Radios")]
    [InlineData("DateInput")]
    public async Task ForWithoutAnyFieldsetAttributesOrElements_DoesNotGenerateFieldset(string component)
    {
        // Act
        var fieldset = await GetFieldsetAsync(component, "no-fieldset");

        // Assert
        Assert.Null(fieldset);
    }

    private static string GetExpectedDisplayName(string component) =>
        component == "DateInput" ? "Date of birth" : "Where do you live?";

    private async Task<IElement?> GetFieldsetAsync(string component, string testId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/FieldsetTests/{component}");
        var response = await fixture.HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var document = await response.GetHtmlDocument();

        var container = document.QuerySelector($"[data-testid='{testId}']") ??
            throw new InvalidOperationException($"No element found with the test ID '{testId}'.");

        return container.QuerySelector("fieldset");
    }
}

public class FieldsetTestsFixture : ServerFixture
{
    public FieldsetTestsFixture()
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
            .AddRazorOptions(options => options.ViewLocationFormats.Add("/FieldsetTestsViews/{0}.cshtml"));
    }
}

[Route("FieldsetTests")]
public class FieldsetTestsController : Controller
{
    [HttpGet("Checkboxes")]
    public IActionResult GetCheckboxes() => View("Checkboxes", new FieldsetTestsModel());

    [HttpGet("Radios")]
    public IActionResult GetRadios() => View("Radios", new FieldsetTestsModel());

    [HttpGet("DateInput")]
    public IActionResult GetDateInput() => View("DateInput", new FieldsetTestsModel());
}

public class FieldsetTestsModel
{
    [Display(Name = "Where do you live?")]
    public string[]? Countries { get; set; }

    [Display(Name = "Where do you live?")]
    public string? Country { get; set; }

    [Display(Name = "Date of birth")]
    public DateOnly? DateOfBirth { get; set; }
}
