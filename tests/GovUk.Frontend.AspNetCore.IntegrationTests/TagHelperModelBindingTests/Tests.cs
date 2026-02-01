using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Frontend.AspNetCore.IntegrationTests.TagHelperModelBindingTests;

public class Tests(TagHelperModelBindingTestsFixture fixture) : IClassFixture<TagHelperModelBindingTestsFixture>
{
    [Fact]
    public async Task TextInput_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/TextInput");

        var input = page.Locator("input").First;
        Assert.Equal("Text", await input.GetAttributeAsync("name"));
        Assert.Equal("Text", await input.GetAttributeAsync("id"));
        Assert.Equal("Model value", await input.GetAttributeAsync("value"));

        var label = page.Locator("label").First;
        Assert.Equal("Text", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task TextInputOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/TextInputOverridden");

        var input = page.Locator("input").First;
        Assert.Equal("OverriddenName", await input.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await input.GetAttributeAsync("id"));
        Assert.Equal("Overridden value", await input.GetAttributeAsync("value"));

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task PasswordInput_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/PasswordInput");

        var input = page.Locator("input[type='password']").First;
        Assert.Equal("Password", await input.GetAttributeAsync("name"));
        Assert.Equal("Password", await input.GetAttributeAsync("id"));
        Assert.Equal("Model value", await input.GetAttributeAsync("value"));

        var label = page.Locator("label").First;
        Assert.Equal("Password", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task PasswordInputOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/PasswordInputOverridden");

        var input = page.Locator("input[type='password']").First;
        Assert.Equal("OverriddenName", await input.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await input.GetAttributeAsync("id"));
        Assert.Equal("Overridden value", await input.GetAttributeAsync("value"));

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task DateInput_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/DateInput");

        var dayInput = page.Locator("input[name='Date.Day']").First;
        Assert.Equal("Date.Day", await dayInput.GetAttributeAsync("name"));
        Assert.Equal("Date.Day", await dayInput.GetAttributeAsync("id"));
        Assert.Equal("15", await dayInput.GetAttributeAsync("value"));

        var monthInput = page.Locator("input[name='Date.Month']").First;
        Assert.Equal("Date.Month", await monthInput.GetAttributeAsync("name"));
        Assert.Equal("Date.Month", await monthInput.GetAttributeAsync("id"));
        Assert.Equal("3", await monthInput.GetAttributeAsync("value"));

        var yearInput = page.Locator("input[name='Date.Year']").First;
        Assert.Equal("Date.Year", await yearInput.GetAttributeAsync("name"));
        Assert.Equal("Date.Year", await yearInput.GetAttributeAsync("id"));
        Assert.Equal("2024", await yearInput.GetAttributeAsync("value"));

        var legend = page.Locator(".govuk-fieldset__legend").First;
        Assert.Equal("ModelMetadata display name", await legend.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task DateInputOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/DateInputOverridden");

        var dayInput = page.Locator("input[name='OverriddenName.Day']").First;
        Assert.Equal("OverriddenName.Day", await dayInput.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId.Day", await dayInput.GetAttributeAsync("id"));
        Assert.Equal("15", await dayInput.GetAttributeAsync("value"));

        var monthInput = page.Locator("input[name='OverriddenName.Month']").First;
        Assert.Equal("OverriddenName.Month", await monthInput.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId.Month", await monthInput.GetAttributeAsync("id"));
        Assert.Equal("3", await monthInput.GetAttributeAsync("value"));

        var yearInput = page.Locator("input[name='OverriddenName.Year']").First;
        Assert.Equal("OverriddenName.Year", await yearInput.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId.Year", await yearInput.GetAttributeAsync("id"));
        Assert.Equal("2024", await yearInput.GetAttributeAsync("value"));

        var legend = page.Locator(".govuk-fieldset__legend").First;
        Assert.Equal("Overridden legend", await legend.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task DateInputOverriddenItems_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/DateInputOverriddenItems");

        var dayInput = page.Locator("input[name='OverriddenDayName']").First;
        Assert.Equal("OverriddenDayId", await dayInput.GetAttributeAsync("id"));
        Assert.Equal("1", await dayInput.GetAttributeAsync("value"));

        var monthInput = page.Locator("input[name='OverriddenMonthName']").First;
        Assert.Equal("OverriddenMonthId", await monthInput.GetAttributeAsync("id"));
        Assert.Equal("2", await monthInput.GetAttributeAsync("value"));

        var yearInput = page.Locator("input[name='OverriddenYearName']").First;
        Assert.Equal("OverriddenYearId", await yearInput.GetAttributeAsync("id"));
        Assert.Equal("2020", await yearInput.GetAttributeAsync("value"));
    }

    [Fact]
    public async Task Textarea_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/Textarea");

        var textarea = page.Locator("textarea").First;
        Assert.Equal("Text", await textarea.GetAttributeAsync("name"));
        Assert.Equal("Text", await textarea.GetAttributeAsync("id"));
        Assert.Equal("Model value", await textarea.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("Text", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task TextareaOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/TextareaOverridden");

        var textarea = page.Locator("textarea").First;
        Assert.Equal("OverriddenName", await textarea.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await textarea.GetAttributeAsync("id"));
        Assert.Equal("Overridden value", await textarea.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task CharacterCount_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/CharacterCount");

        var textarea = page.Locator("textarea").First;
        Assert.Equal("Text", await textarea.GetAttributeAsync("name"));
        Assert.Equal("Text", await textarea.GetAttributeAsync("id"));
        Assert.Equal("Model value", await textarea.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("Text", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task CharacterCountOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/CharacterCountOverridden");

        var textarea = page.Locator("textarea").First;
        Assert.Equal("OverriddenName", await textarea.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await textarea.GetAttributeAsync("id"));
        Assert.Equal("Overridden value", await textarea.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task FileUpload_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/FileUpload");

        var input = page.Locator("input[type='file']").First;
        Assert.Equal("File", await input.GetAttributeAsync("name"));
        Assert.Equal("File", await input.GetAttributeAsync("id"));

        var label = page.Locator("label").First;
        Assert.Equal("File", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task FileUploadOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/FileUploadOverridden");

        var input = page.Locator("input[type='file']").First;
        Assert.Equal("OverriddenName", await input.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await input.GetAttributeAsync("id"));

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task Select_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/Select");

        var select = page.Locator("select").First;
        Assert.Equal("Option", await select.GetAttributeAsync("name"));
        Assert.Equal("Option", await select.GetAttributeAsync("id"));
        Assert.Equal("option2", await select.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("Option", await label.GetAttributeAsync("for"));
        Assert.Equal("ModelMetadata display name", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task SelectOverridden_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/SelectOverridden");

        var select = page.Locator("select").First;
        Assert.Equal("OverriddenName", await select.GetAttributeAsync("name"));
        Assert.Equal("OverriddenId", await select.GetAttributeAsync("id"));
        Assert.Equal("overridden-value", await select.InputValueAsync());

        var label = page.Locator("label").First;
        Assert.Equal("OverriddenId", await label.GetAttributeAsync("for"));
        Assert.Equal("Overridden label", await label.InnerTextAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("Overridden hint", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Overridden error message", await errorMessage.TextContentAsync());
    }

    [Fact]
    public async Task Checkboxes_RendersCorrectly()
    {
        var page = await fixture.Browser!.NewPageAsync();
        await page.GotoAsync($"{ServerFixture.BaseUrl}/ModelBindingTests/Checkboxes");

        var checkbox1 = page.Locator("input[value='option1']").First;
        Assert.Equal("Options", await checkbox1.GetAttributeAsync("name"));
        Assert.Equal("Options", await checkbox1.GetAttributeAsync("id"));
        Assert.False(await checkbox1.IsCheckedAsync());

        var checkbox2 = page.Locator("input[value='option2']").First;
        Assert.Equal("Options", await checkbox2.GetAttributeAsync("name"));
        Assert.Equal("Options-2", await checkbox2.GetAttributeAsync("id"));
        // This is the critical assertion that should pass with the fix
        Assert.True(await checkbox2.IsCheckedAsync());

        var hint = page.Locator(".govuk-hint").First;
        Assert.Equal("ModelMetadata description", await hint.InnerTextAsync());

        var errorMessage = page.Locator(".govuk-error-message").First;
        Assert.Equal("Error: Model error message", await errorMessage.TextContentAsync());
    }
}

[Route("/[controller]/[action]")]
public class ModelBindingTestsController : Controller
{
    [HttpGet]
    public IActionResult TextInput()
    {
        ModelState.AddModelError(nameof(TextInputTestsModel.Text), "Model error message");
        return View(new TextInputTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult TextInputOverridden()
    {
        ModelState.AddModelError(nameof(TextInputTestsModel.Text), "Model error message");
        return View(new TextInputTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult PasswordInput()
    {
        ModelState.AddModelError(nameof(PasswordInputTestsModel.Password), "Model error message");
        return View(new PasswordInputTestsModel { Password = "Model value" });
    }

    [HttpGet]
    public IActionResult PasswordInputOverridden()
    {
        ModelState.AddModelError(nameof(PasswordInputTestsModel.Password), "Model error message");
        return View(new PasswordInputTestsModel { Password = "Model value" });
    }

    [HttpGet]
    public IActionResult DateInput()
    {
        ModelState.AddModelError(nameof(DateInputTestsModel.Date), "Model error message");
        return View(new DateInputTestsModel { Date = new DateOnly(2024, 3, 15) });
    }

    [HttpGet]
    public IActionResult DateInputOverridden()
    {
        ModelState.AddModelError(nameof(DateInputTestsModel.Date), "Model error message");
        return View(new DateInputTestsModel { Date = new DateOnly(2024, 3, 15) });
    }

    [HttpGet]
    public IActionResult DateInputOverriddenItems()
    {
        ModelState.AddModelError(nameof(DateInputTestsModel.Date), "Model error message");
        return View(new DateInputTestsModel { Date = new DateOnly(2024, 3, 15) });
    }

    [HttpGet]
    public IActionResult Textarea()
    {
        ModelState.AddModelError(nameof(TextareaTestsModel.Text), "Model error message");
        return View(new TextareaTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult TextareaOverridden()
    {
        ModelState.AddModelError(nameof(TextareaTestsModel.Text), "Model error message");
        return View(new TextareaTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult CharacterCount()
    {
        ModelState.AddModelError(nameof(CharacterCountTestsModel.Text), "Model error message");
        return View(new CharacterCountTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult CharacterCountOverridden()
    {
        ModelState.AddModelError(nameof(CharacterCountTestsModel.Text), "Model error message");
        return View(new CharacterCountTestsModel { Text = "Model value" });
    }

    [HttpGet]
    public IActionResult FileUpload()
    {
        ModelState.AddModelError(nameof(FileUploadTestsModel.File), "Model error message");
        return View(new FileUploadTestsModel());
    }

    [HttpGet]
    public IActionResult FileUploadOverridden()
    {
        ModelState.AddModelError(nameof(FileUploadTestsModel.File), "Model error message");
        return View(new FileUploadTestsModel());
    }

    [HttpGet]
    public IActionResult Select()
    {
        ModelState.AddModelError(nameof(SelectTestsModel.Option), "Model error message");
        return View(new SelectTestsModel { Option = "option2" });
    }

    [HttpGet]
    public IActionResult SelectOverridden()
    {
        ModelState.AddModelError(nameof(SelectTestsModel.Option), "Model error message");
        return View(new SelectTestsModel { Option = "option2" });
    }

    [HttpGet]
    public IActionResult Checkboxes()
    {
        ModelState.AddModelError(nameof(CheckboxesTestsModel.Options), "Model error message");
        return View(new CheckboxesTestsModel { Options = ["option2"] });
    }
}

public record TextInputTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public string? Text { get; set; }
}

public record PasswordInputTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public string? Password { get; set; }
}

public record DateInputTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public DateOnly? Date { get; set; }
}

public record TextareaTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public string? Text { get; set; }
}

public record CharacterCountTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public string? Text { get; set; }
}

public record FileUploadTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public Microsoft.AspNetCore.Http.IFormFile? File { get; set; }
}

public record SelectTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public string? Option { get; set; }
}

public record CheckboxesTestsModel
{
    [Display(Name = "ModelMetadata display name", Description = "ModelMetadata description")]
    public List<string>? Options { get; set; }
}
