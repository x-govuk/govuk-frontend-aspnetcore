using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class NavigationAndContentComponentTests
{
    [Fact]
    public async Task Buttons_ReadsEachKindOfButton()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Save and continue" }),
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Cancel", Href = "/cancel", Classes = "govuk-button--secondary" }),
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Delete account", Classes = "govuk-button--warning", Disabled = true }),
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Start now", Href = "/start", IsStartButton = true }),
            g => g.GenerateButtonAsync(new ButtonOptions { Element = "input", Text = "Search" }));

        Assert.Equal(["Save and continue", "Cancel", "Delete account", "Start now", "Search"], doc.Buttons.Select(b => b.Text));

        var save = doc.GetButtonByText("Save and continue");
        Assert.Null(save.Href);
        Assert.Null(save.Variant);
        Assert.False(save.IsStartButton);
        Assert.False(save.IsDisabled);

        var cancel = doc.GetButtonByText("Cancel");
        Assert.Equal("/cancel", cancel.Href);
        Assert.Equal("secondary", cancel.Variant);

        var delete = doc.GetButtonByText("Delete account");
        Assert.Equal("warning", delete.Variant);
        Assert.True(delete.IsDisabled);

        var start = doc.GetButtonByText("Start now");
        Assert.True(start.IsStartButton);
        Assert.Null(start.Variant);
        Assert.Equal("/start", start.Href);
    }

    [Fact]
    public async Task GetButtonByText_NotFound_ThrowsWithTextsOnThePage()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Continue" }),
            g => g.GenerateButtonAsync(new ButtonOptions { Text = "Cancel", Href = "/cancel" }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetButtonByText("Delete"));

        Assert.Equal("a button with the text \"Delete\"", ex.Failure.Expected);
        Assert.Equal("buttons with the texts \"Continue\", \"Cancel\"", ex.Failure.Found);
        Assert.Null(doc.FindButtonByText("Delete"));
    }

    [Fact]
    public async Task Details_ReadsSummaryTextAndOpenState()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateDetailsAsync(new DetailsOptions
            {
                SummaryText = "Help with nationality",
                Html = new HtmlString("<p>We need to know your nationality.</p><p>You can apply for a passport.</p>")
            }),
            g => g.GenerateDetailsAsync(new DetailsOptions { SummaryText = "Opened", Text = "Already open", Open = true }));

        Assert.Equal(["Help with nationality", "Opened"], doc.AllDetails.Select(d => d.Summary));

        var help = doc.GetDetailsBySummary("Help with nationality");
        Assert.Equal(["We need to know your nationality.", "You can apply for a passport."], help.TextLines);
        Assert.Equal("We need to know your nationality. You can apply for a passport.", help.Text);
        Assert.False(help.IsOpen);

        Assert.True(doc.GetDetailsBySummary("Opened").IsOpen);
        Assert.Null(doc.FindDetailsBySummary("Missing"));
    }

    [Fact]
    public async Task WarningTexts_ReadTextWithoutTheIconOrItsFallbackText()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateWarningTextAsync(new WarningTextOptions
        {
            Text = "You can be fined up to £5,000 if you do not register.",
            IconFallbackText = "Warning"
        }));

        var warning = Assert.Single(doc.WarningTexts);
        Assert.Equal("You can be fined up to £5,000 if you do not register.", warning.Text);
        Assert.Equal("Warning", warning.IconFallbackText);
    }

    [Fact]
    public async Task InsetTexts_ReadText()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateInsetTextAsync(new InsetTextOptions
        {
            Html = new HtmlString("<p>It can take up to 8 weeks.</p><p>Do not call us.</p>")
        }));

        var inset = Assert.Single(doc.InsetTexts);
        Assert.Equal(["It can take up to 8 weeks.", "Do not call us."], inset.TextLines);
        Assert.Equal("It can take up to 8 weeks. Do not call us.", inset.Text);
    }

    [Fact]
    public async Task Breadcrumbs_ReadsItemsAndTheCurrentPage()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateBreadcrumbsAsync(new BreadcrumbsOptions
        {
            Items =
            [
                new BreadcrumbsOptionsItem { Text = "Home", Href = "/" },
                new BreadcrumbsOptionsItem { Text = "Passports, travel and living abroad", Href = "/browse/abroad" },
                new BreadcrumbsOptionsItem { Text = "Travel abroad" }
            ]
        }));

        var breadcrumbs = doc.Breadcrumbs;

        Assert.NotNull(breadcrumbs);
        Assert.Equal(["Home", "Passports, travel and living abroad", "Travel abroad"], breadcrumbs.Items.Select(i => i.Text));
        Assert.Equal(["/", "/browse/abroad", null], breadcrumbs.Items.Select(i => i.Href));
        Assert.Equal("Travel abroad", breadcrumbs.Current?.Text);
    }

    [Fact]
    public void Breadcrumbs_None_IsNull()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        Assert.Null(doc.Breadcrumbs);
    }

    [Fact]
    public async Task Accordion_ReadsSections()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateAccordionAsync(new AccordionOptions
        {
            Id = "accordion",
            Items =
            [
                new AccordionOptionsItem
                {
                    Heading = new AccordionOptionsItemHeading { Text = "Writing well for the web" },
                    Summary = new AccordionOptionsItemSummary { Text = "Understanding how people read" },
                    Content = new AccordionOptionsItemContent { Html = new HtmlString("<p>People read differently online.</p><p>Keep it short.</p>") },
                    Expanded = true
                },
                new AccordionOptionsItem
                {
                    Heading = new AccordionOptionsItemHeading { Text = "Writing well for specialists" },
                    Content = new AccordionOptionsItemContent { Text = "Specialists still prefer plain English." }
                }
            ]
        }));

        var accordion = doc.Accordion;

        Assert.NotNull(accordion);
        Assert.Equal(["Writing well for the web", "Writing well for specialists"], accordion.Sections.Select(s => s.Heading));

        var web = accordion.GetSectionByHeading("Writing well for the web");
        Assert.Equal("Understanding how people read", web.Summary);
        Assert.Equal(["People read differently online.", "Keep it short."], web.TextLines);
        Assert.True(web.IsExpanded);
        Assert.NotNull(web.ContentElement);

        var specialists = accordion.GetSectionByHeading("Writing well for specialists");
        Assert.Null(specialists.Summary);
        Assert.Equal("Specialists still prefer plain English.", specialists.Text);
        Assert.False(specialists.IsExpanded);

        var ex = Assert.Throws<ComponentLookupException>(() => accordion.GetSectionByHeading("Missing"));
        Assert.Equal("sections with the headings \"Writing well for the web\", \"Writing well for specialists\"", ex.Failure.Found);
        Assert.Null(accordion.FindSectionByHeading("Missing"));
    }

    [Fact]
    public async Task Tabs_ReadsTabsAndTheirPanels()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateTabsAsync(new TabsOptions
        {
            Title = "Contents",
            Items =
            [
                new TabsOptionsItem { Id = "past-day", Label = "Past day", Panel = new TabsOptionsItemPanel { Text = "3 cases" } },
                new TabsOptionsItem
                {
                    Id = "past-week",
                    Label = "Past week",
                    Panel = new TabsOptionsItemPanel { Html = new HtmlString("<p>24 cases</p><p>Up 10%</p>") }
                }
            ]
        }));

        var tabs = doc.Tabs;

        Assert.NotNull(tabs);
        Assert.Equal("Contents", tabs.Title);
        Assert.Equal(["Past day", "Past week"], tabs.Items.Select(t => t.Label));
        Assert.Equal("Past day", tabs.Selected?.Label);

        var pastDay = tabs.GetTabByLabel("Past day");
        Assert.Equal("past-day", pastDay.Id);
        Assert.Equal("3 cases", pastDay.Text);
        Assert.True(pastDay.IsSelected);

        var pastWeek = tabs.GetTabByLabel("Past week");
        Assert.Equal(["24 cases", "Up 10%"], pastWeek.TextLines);
        Assert.Equal("past-week", pastWeek.PanelElement?.Id);
        Assert.False(pastWeek.IsSelected);

        Assert.Null(tabs.FindTabByLabel("Past month"));
        Assert.Same(tabs.Element, Assert.Single(doc.AllTabs).Element);
    }
}
