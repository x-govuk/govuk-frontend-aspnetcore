using System.Reflection;
using System.Text.RegularExpressions;
using GovUk.Frontend.AspNetCore.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public partial class CharacterCountTests(CharacterCountTestsFixture fixture) : IClassFixture<CharacterCountTestsFixture>
{
    // Recent versions of Chromium split words at a full stop or colon between letters ("e.g."), unlike other browsers,
    // and the validator counts as the others do, so the text has none.
    private const string Text =
        "Hello, world! It's a well-known fact: 3.14 isn't π — café crème, 1,000 a·b 👍🏽🇬🇧 צה\"ל a_b ...\n" +
        "second line\n\nfourth - line";

    public IBrowser Browser { get; } = fixture.Browser!;

    [Theory]
    [InlineData("Length", CharacterCountType.Length)]
    [InlineData("Characters", CharacterCountType.Characters)]
    [InlineData("Words", CharacterCountType.Words)]
    public async Task CharacterCountValidator_CountsTheSameAsTheComponent(string field, CharacterCountType countType)
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });
        await page.GotoAsync("/CharacterCountTests");

        var browserCount = await FillAndGetBrowserCountAsync(page, field);

        // The browser submits line breaks as \r\n
        var validator = new CharacterCountValidator(CharacterCountTestsModel.Limit, countType);
        Assert.Equal(browserCount, validator.GetCount(Text.Replace("\n", "\r\n", StringComparison.Ordinal)));
    }

    [Fact]
    public async Task MaxWordsValidator_CountsTheSameAsTheComponent()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });
        await page.GotoAsync("/CharacterCountTests");

        var browserCount = await FillAndGetBrowserCountAsync(page, "MaxWords");

        Assert.True(new MaxWordsValidator(browserCount).IsValid(Text));
        Assert.False(new MaxWordsValidator(browserCount - 1).IsValid(Text));
    }

    [Fact]
    public async Task TextWithLineBreaksAtTheLimitInTheBrowser_PassesValidation()
    {
        var page = await Browser.NewPageAsync(new BrowserNewPageOptions() { BaseURL = ServerFixture.BaseUrl });
        await page.GotoAsync("/CharacterCountTests");

        // 5 characters in the browser, but submitted as 7
        var text = "a\nb\nc";
        await page.FillAsync("[name='ShortLength']", text);
        await Expect(page.Locator("[name='ShortLength'] ~ .govuk-character-count__status")).ToHaveTextAsync("You have 0 characters remaining");

        await page.SubmitFormAsync("Save");

        await Expect(page.Locator("[name='ShortLength']")).ToHaveValueAsync(text);
        await Expect(page.Locator(".govuk-error-message")).ToHaveCountAsync(0);
    }

    private static async Task<int> FillAndGetBrowserCountAsync(IPage page, string field)
    {
        await page.FillAsync($"[name='{field}']", Text);

        var status = page.Locator($"[name='{field}'] ~ .govuk-character-count__status");
        await Expect(status).Not.ToHaveTextAsync("");
        var message = await status.TextContentAsync();
        var match = CountMessagePattern().Match(message!);
        Assert.True(match.Success, $"Unexpected count message '{message}'.");

        var remaining = int.Parse(match.Groups["count"].Value);
        return CharacterCountTestsModel.Limit - (match.Groups["over"].Success ? -remaining : remaining);
    }

    [GeneratedRegex(@"You have (?<count>\d+) (characters?|words?) (remaining|(?<over>too many))")]
    private static partial Regex CountMessagePattern();
}

public class CharacterCountTestsFixture : ServerFixture
{
    protected override void Configure(IApplicationBuilder app)
    {
        // The tests need the component's JavaScript, which an app gets from the package rather than a project reference
        var govUkFrontendLibrary = typeof(CharacterCountTestsFixture).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == "GovUkFrontendLibrary")
            .Value!;
        app.UseStaticFiles(new StaticFileOptions { FileProvider = new PhysicalFileProvider(govUkFrontendLibrary) });

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
                options.ViewLocationFormats.Add("/CharacterCountTestsViews/{0}.cshtml");
            });
    }
}

[Route("CharacterCountTests")]
public class CharacterCountTestsController : Controller
{
    [HttpGet("")]
    public IActionResult Get() => View("Index", new CharacterCountTestsModel());

    [HttpPost("")]
    public IActionResult Post(CharacterCountTestsModel model) => View("Index", model);
}

public class CharacterCountTestsModel
{
    public const int Limit = 1000;
    public const int ShortLimit = 5;

    [CharacterCount(ShortLimit, ErrorMessage = "ShortLength is too long")]
    public string? ShortLength { get; set; }

    [CharacterCount(Limit, ErrorMessage = "Length is too long")]
    public string? Length { get; set; }

    [CharacterCount(Limit, CountType = CharacterCountType.Characters, ErrorMessage = "Characters is too long")]
    public string? Characters { get; set; }

    [CharacterCount(Limit, CountType = CharacterCountType.Words, ErrorMessage = "Words is too long")]
    public string? Words { get; set; }

    [MaxWords(Limit, ErrorMessage = "MaxWords is too long")]
    public string? MaxWords { get; set; }
}
