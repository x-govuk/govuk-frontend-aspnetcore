using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public class FormElementValidationTests(FormElementValidationTestsFixture fixture) : IClassFixture<FormElementValidationTestsFixture>
{
    public IBrowser Browser { get; } = fixture.Browser!;

    [Fact]
    public async Task AllFormElements_WithValidationErrors_ShowErrorMessagesAndErrorSummary()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });

        // Navigate to the form
        await page.GotoAsync("/FormElementValidation/Index");

        // Fill out the form with values that will trigger validation errors
        await page.FillAsync("[name='TextInput']", "test");
        await page.FillAsync("[name='PasswordInput']", "test");
        await page.FillAsync("[name='TextArea']", "test");
        await page.FillAsync("[name='CharacterCount']", "test");
        await page.SelectOptionAsync("[name='Select']", "option1");
        await page.CheckAsync("[value='checkbox1']");
        await page.CheckAsync("[value='radio1']");
        await page.FillAsync("[name='DateInput.Day']", "1");
        await page.FillAsync("[name='DateInput.Month']", "1");
        await page.FillAsync("[name='DateInput.Year']", "2020");

        // Submit the form
        await page.RunAndWaitForNavigationAsync(() => page.Keyboard.PressAsync("Enter"));

        // Verify error summary is displayed
        var errorSummary = await page.QuerySelectorAsync(".govuk-error-summary");
        Assert.NotNull(errorSummary);

        // Get all error summary links
        var errorLinks = await page.QuerySelectorAllAsync(".govuk-error-summary__list a");
        Assert.Equal(9, errorLinks.Count);

        // Verify each form element has an error message and is linked from error summary
        await VerifyErrorForElement(page, "TextInput", "Enter valid text input");
        await VerifyErrorForElement(page, "PasswordInput", "Enter valid password input");
        await VerifyErrorForElement(page, "TextArea", "Enter valid text area");
        await VerifyErrorForElement(page, "CharacterCount", "Enter valid character count");
        await VerifyErrorForElement(page, "Select", "Select valid option");
        await VerifyErrorForElement(page, "Checkboxes", "Select valid checkboxes");
        await VerifyErrorForElement(page, "Radios", "Select valid radios");
        await VerifyErrorForElement(page, "DateInput", "Enter valid date input");
        await VerifyErrorForElement(page, "FileUpload", "Upload valid file");
    }

    private async Task VerifyErrorForElement(IPage page, string elementId, string expectedErrorMessage)
    {
        // Check that error message is displayed near the element
        var errorMessageSelector = $"#{elementId}-error, [id^='{elementId}'][id$='-error']";
        var errorMessage = await page.QuerySelectorAsync(errorMessageSelector);
        Assert.NotNull(errorMessage);

        var errorText = (await errorMessage.TextContentAsync())?.Trim().Replace("Error: ", "").Replace("Error:\n", "").Trim();
        Assert.Contains(expectedErrorMessage, errorText);

        // Check that error summary link exists and points to the correct element
        var errorLink = await page.QuerySelectorAsync($".govuk-error-summary__list a[href='#{elementId}'], .govuk-error-summary__list a[href^='#{elementId}']");
        Assert.NotNull(errorLink);

        var linkText = (await errorLink.TextContentAsync())?.Trim();
        Assert.Contains(expectedErrorMessage, linkText);
    }
}

public class FormElementValidationTestsFixture : ServerFixture
{
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
            .AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/FormElementValidationTestsViews/{0}.cshtml");
            });
    }
}

[Route("FormElementValidation")]
public class FormElementValidationTestsController : Controller
{
    [HttpGet("Index")]
    public IActionResult GetIndex() => View("Index", new FormElementValidationModel());

    [HttpPost("Index")]
    public IActionResult PostIndex(FormElementValidationModel model)
    {
        // Add validation errors for all fields
        ModelState.AddModelError(nameof(model.TextInput), "Enter valid text input");
        ModelState.AddModelError(nameof(model.PasswordInput), "Enter valid password input");
        ModelState.AddModelError(nameof(model.TextArea), "Enter valid text area");
        ModelState.AddModelError(nameof(model.CharacterCount), "Enter valid character count");
        ModelState.AddModelError(nameof(model.Select), "Select valid option");
        ModelState.AddModelError(nameof(model.Checkboxes), "Select valid checkboxes");
        ModelState.AddModelError(nameof(model.Radios), "Select valid radios");
        ModelState.AddModelError(nameof(model.DateInput), "Enter valid date input");
        ModelState.AddModelError(nameof(model.FileUpload), "Upload valid file");

        return View("Index", model);
    }
}

public class FormElementValidationModel
{
    [Display(Name = "Text input")]
    public string? TextInput { get; set; }

    [Display(Name = "Password input")]
    public string? PasswordInput { get; set; }

    [Display(Name = "Text area")]
    public string? TextArea { get; set; }

    [Display(Name = "Character count")]
    public string? CharacterCount { get; set; }

    [Display(Name = "Select")]
    public string? Select { get; set; }

    [Display(Name = "Checkboxes")]
    public List<string>? Checkboxes { get; set; }

    [Display(Name = "Radios")]
    public string? Radios { get; set; }

    [Display(Name = "Date input")]
    public DateOnly? DateInput { get; set; }

    [Display(Name = "File upload")]
    public IFormFile? FileUpload { get; set; }
}
