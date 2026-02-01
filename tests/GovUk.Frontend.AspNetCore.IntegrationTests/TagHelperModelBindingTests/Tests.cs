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
        await page.PauseAsync();

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
        await page.PauseAsync();

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
