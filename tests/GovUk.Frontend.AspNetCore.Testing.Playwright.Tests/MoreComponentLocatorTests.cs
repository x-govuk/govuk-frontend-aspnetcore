namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

public class MoreComponentLocatorTests(BrowserApp app)
{
    [Fact]
    public async Task Pagination_LocatesItsParts()
    {
        var page = await GotoAsync("/MoreComponents");

        var pagination = page.Pagination;

        await Expect(pagination.Previous).ToHaveAttributeAsync("href", "/MoreComponents?page=1");
        await Expect(pagination.Next).ToHaveAttributeAsync("href", "/MoreComponents?page=3");
        await Expect(pagination.Items).ToHaveCountAsync(3);
        await Expect(pagination.Current).ToHaveTextAsync("2");
        await Expect(pagination.GetLinkByNumber(3)).ToHaveAttributeAsync("href", "/MoreComponents?page=3");
    }

    [Fact]
    public async Task Breadcrumbs_LocatesItsParts()
    {
        var page = await GotoAsync("/MoreComponents");

        var breadcrumbs = page.Breadcrumbs;

        await Expect(breadcrumbs.Items).ToHaveTextAsync(["Home", "Components", "More components"]);
        await Expect(breadcrumbs.Current).ToHaveTextAsync("More components");
        await Expect(breadcrumbs.GetLinkByText("Components")).ToHaveAttributeAsync("href", "/Components");
    }

    [Fact]
    public async Task NotificationBanner_GetLinkByText_LocatesTheLink()
    {
        var page = await GotoAsync("/MoreComponents");

        await Expect(page.NotificationBanner.GetLinkByText("View application")).ToHaveAttributeAsync("href", "/Fields");
    }

    [Fact]
    public async Task Panel_WarningText_InsetText_AndTags_AreLocated()
    {
        var page = await GotoAsync("/MoreComponents");

        await Expect(page.Panel.Title).ToHaveTextAsync("Application complete");
        await Expect(page.Panel.Body).ToContainTextAsync("HDJ2123F");
        await Expect(page.WarningText.Text).ToContainTextAsync("You can be fined up to £5,000 if you do not register.");
        await Expect(page.InsetText).ToHaveTextAsync("It can take up to 8 weeks to register a lasting power of attorney.");
        await Expect(page.GetTagByText("Incomplete")).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("govuk-tag--blue"));
        await Expect(page.GetTagByText("Incomp")).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetDetailsBySummary_OpensAndReveals()
    {
        var page = await GotoAsync("/MoreComponents");

        var details = page.GetDetailsBySummary("Help with nationality");

        await Expect(details.Text).Not.ToBeVisibleAsync();
        await details.OpenAsync();
        await Expect(details.Text).ToBeVisibleAsync();
        await details.OpenAsync();
        await Expect(details.Text).ToContainTextAsync("We need to know your nationality");
    }

    [Fact]
    public async Task GetAccordionSectionByHeading_ExpandsAndReveals()
    {
        var page = await GotoAsync("/MoreComponents");

        var section = page.GetAccordionSectionByHeading("Writing well for the web");

        await Expect(section.Heading).ToHaveTextAsync("Writing well for the web");
        await Expect(section.Summary).ToHaveTextAsync("Understanding how people read");
        await Expect(section.Content).Not.ToBeVisibleAsync();
        await section.ExpandAsync();
        await Expect(section.Content).ToBeVisibleAsync();
        await section.ExpandAsync();
        await Expect(section.Content).ToHaveTextAsync("This is the content for Writing well for the web.");
        await Expect(page.GetAccordionSectionByHeading("Writing well").Locator).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetTabByLabel_SelectsAndShowsItsPanel()
    {
        var page = await GotoAsync("/MoreComponents");

        var pastWeek = page.GetTabByLabel("Past week");

        await Expect(page.GetTabByLabel("Past day").Panel).ToBeVisibleAsync();
        await Expect(pastWeek.Panel).Not.ToBeVisibleAsync();
        await pastWeek.SelectAsync();
        await Expect(pastWeek.Panel).ToBeVisibleAsync();
        await Expect(pastWeek.Panel).ToHaveTextAsync("24 cases in the past week");
        await Expect(pastWeek.Tab).ToHaveAttributeAsync("aria-selected", "true");
    }

    [Fact]
    public async Task GetTableByCaption_LocatesRowsAndCells()
    {
        var page = await GotoAsync("/MoreComponents");

        var table = page.GetTableByCaption("Dates and amounts");

        await Expect(table.Headers).ToHaveTextAsync(["Date", "Amount"]);
        await Expect(table.Rows).ToHaveCountAsync(3);

        var row = table.GetRowByFirstCellText("Total estimated pay");
        await Expect(row.GetCellByHeader("Amount")).ToHaveTextAsync("£4,282.20");
        await Expect(row.GetCellByHeader("Date")).ToHaveTextAsync("Total estimated pay");
        await Expect(row.GetCellByHeader("Missing")).ToHaveCountAsync(0);
        await Expect(page.Table.Caption).ToHaveTextAsync("Dates and amounts");
    }

    [Fact]
    public async Task GetTaskListItemByName_LocatesItsParts()
    {
        var page = await GotoAsync("/MoreComponents");

        var directors = page.GetTaskListItemByName("Company Directors");
        await Expect(directors.Link).ToHaveAttributeAsync("href", "/Fields");
        await Expect(directors.Status).ToHaveTextAsync("Completed");

        var history = page.GetTaskListItemByName("Financial history");
        await Expect(history.Link).ToHaveCountAsync(0);
        await Expect(history.Hint).ToHaveTextAsync("Include 5 years of the company's relevant financial information");
        await Expect(history.Status.GetTagByText("Incomplete")).ToHaveCountAsync(1);
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
