namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

public class ComponentLocatorTests(BrowserApp app)
{
    [Fact]
    public async Task ErrorSummary_LocatesItsParts()
    {
        var page = await GotoAsync("/Components");

        var summary = page.ErrorSummary;

        await Expect(summary.Title).ToHaveTextAsync("There is a problem");
        await Expect(summary.Description).ToHaveTextAsync("Check the following");
        await Expect(summary.Items).ToHaveTextAsync(["Enter your full name", "Enter an email address"]);
        await Expect(summary.GetLinkByText("Enter an email address")).ToHaveAttributeAsync("href", "#email");
    }

    [Fact]
    public async Task ErrorSummary_WhenAbsent_HasNoElements()
    {
        var page = await GotoAsync("/Empty");

        await Expect(page.ErrorSummary.Locator).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task NotificationBanner_LocatesItsParts()
    {
        var page = await GotoAsync("/Components");

        var banner = page.NotificationBanner;

        await Expect(banner.Locator).ToContainClassAsync("govuk-notification-banner--success");
        await Expect(banner.Title).ToHaveTextAsync("Success");
        await Expect(banner.Heading).ToHaveTextAsync("Application submitted");
        await Expect(banner.Content).ToContainTextAsync("We've sent you an email.");
    }

    [Fact]
    public async Task GetSummaryListRowByKey_MatchesTheWholeKey()
    {
        var page = await GotoAsync("/Components");

        // "Name" is also the start of "Name of school"
        var row = page.GetSummaryListRowByKey("Name");

        await Expect(row.Key).ToHaveTextAsync("Name");
        await Expect(row.Value).ToHaveTextAsync("Sarah Philips");
        await Expect(row.Actions).ToHaveCountAsync(1);
    }

    [Fact]
    public async Task SummaryListRow_ChangeLink_IgnoresVisuallyHiddenText()
    {
        var page = await GotoAsync("/Components");

        var row = page.GetSummaryListRowByKey("Name of school");

        await Expect(row.ChangeLink).ToHaveAttributeAsync("href", "/Fields#school");
        await Expect(row.GetActionByText("Remove")).ToHaveAttributeAsync("href", "/Fields#remove");
        await Expect(row.GetActionByText("Delete")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetSummaryCardByTitle_ScopesRowLookups()
    {
        var page = await GotoAsync("/Components");

        var card = page.GetSummaryCardByTitle("University of Gloucestershire");

        await Expect(card.Title).ToHaveTextAsync("University of Gloucestershire");
        await Expect(card.GetRowByKey("Course").Value).ToHaveTextAsync("History (V1X1)");
        await Expect(page.GetSummaryCardByTitle("University of Bristol").GetActionByText("Delete choice"))
            .ToHaveCountAsync(1);
    }

    [Fact]
    public async Task GetSummaryListRowByKey_WithDuplicateKeys_FailsInStrictMode()
    {
        var page = await GotoAsync("/Components");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            page.GetSummaryListRowByKey("Course").Value.TextContentAsync(new() { Timeout = 1_000 }));

        Assert.Contains("strict mode violation", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task BackLink_LocatesTheBackLink()
    {
        var page = await GotoAsync("/Components");

        await Expect(page.BackLink).ToHaveAttributeAsync("href", "/Fields");
    }

    [Fact]
    public async Task GetButtonByText_FindsGovUkButtonsAndLinksStyledAsButtons()
    {
        var page = await GotoAsync("/Components");

        // The page also has a <button> with this text that isn't a GOV.UK button
        await Expect(page.GetButtonByText("Save and continue")).ToHaveCountAsync(1);
        await Expect(page.GetButtonByText("Start now")).ToHaveAttributeAsync("href", "/Fields");
    }

    [Fact]
    public async Task Lookups_OnALocator_AreScopedToIt()
    {
        var page = await GotoAsync("/Components");

        var main = page.Locator("main");

        await Expect(main.ErrorSummary.Items).ToHaveCountAsync(2);
        await Expect(main.GetSummaryListRowByKey("Name").Value).ToHaveTextAsync("Sarah Philips");
        await Expect(page.Locator("footer").ErrorSummary.Locator).ToHaveCountAsync(0);
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
