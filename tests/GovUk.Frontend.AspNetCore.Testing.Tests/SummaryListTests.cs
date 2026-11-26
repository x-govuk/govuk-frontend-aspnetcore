using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class SummaryListTests
{
    [Fact]
    public async Task Rows_ReadsKeyValueAndActions()
    {
        var doc = await RenderListAsync(
            Row("Name", "Sarah Philips", Action("Change", "/name", "name")),
            Row("Address", new HtmlString("72 Guild Street<br>London<br>SE23 6FH")),
            Row("Contact details", new HtmlString("<p class=\"govuk-body\">07700 900457</p><p class=\"govuk-body\">sarah@example.com</p>"),
                Action("Change", "/contact", "contact details"),
                Action("Remove", "/contact/remove", "contact details")));

        var list = Assert.Single(doc.SummaryLists);

        Assert.Equal(["Name", "Address", "Contact details"], list.Rows.Select(r => r.Key));

        var name = list.GetRow("Name");
        Assert.Equal("Sarah Philips", name.Value);
        Assert.Equal(["Sarah Philips"], name.ValueLines);
        var change = Assert.Single(name.Actions);
        Assert.Equal("Change", change.Text);
        Assert.Equal("name", change.VisuallyHiddenText);
        Assert.Equal("/name", change.Href);
        Assert.Same(change.Element, name.ChangeLink!.Element);

        var address = list.GetRow("Address");
        Assert.Equal("72 Guild Street London SE23 6FH", address.Value);
        Assert.Equal(["72 Guild Street", "London", "SE23 6FH"], address.ValueLines);
        Assert.Empty(address.Actions);
        Assert.Null(address.ChangeLink);

        var contact = list.GetRow("Contact details");
        Assert.Equal(["07700 900457", "sarah@example.com"], contact.ValueLines);
        Assert.Equal(["Change", "Remove"], contact.Actions.Select(a => a.Text));
        Assert.Equal("/contact/remove", contact.GetAction("Remove").Href);
        Assert.Null(contact.FindAction("Delete"));
    }

    [Fact]
    public async Task GetRow_NotFound_ThrowsWithKeysOnTheList()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"), Row("Phone", "07700 900457"));
        var list = doc.SummaryLists.Single();

        var ex = Assert.Throws<ComponentLookupException>(() => list.GetRow("Email address"));

        Assert.Equal("Summary list row not found.", ex.Failure.Summary);
        Assert.Equal("a row with the key \"Email address\"", ex.Failure.Expected);
        Assert.Equal("rows with the keys \"Name\", \"Phone\"", ex.Failure.Found);
        Assert.StartsWith("<dl class=\"govuk-summary-list\">", ex.Failure.Html);
        Assert.StartsWith(
            """
            Summary list row not found.
            Expected: a row with the key "Email address"
            Found:    rows with the keys "Name", "Phone"

            HTML:
            <dl class="govuk-summary-list">
            """,
            ex.Message);
    }

    [Fact]
    public async Task FindRow_NotFound_ReturnsNull()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        Assert.Null(doc.SummaryLists.Single().FindRow("Email address"));
    }

    [Fact]
    public async Task DuplicateKeys_GetRowThrowsAndGetRowsWithKeyReturnsAll()
    {
        var doc = await RenderListAsync(Row("Director", "Sarah Philips"), Row("Director", "Ben Jones"));
        var list = doc.SummaryLists.Single();

        var ex = Assert.Throws<ComponentLookupException>(() => list.GetRow("Director"));
        Assert.Equal("More than one summary list row found.", ex.Failure.Summary);
        Assert.Equal("2 rows", ex.Failure.Found);

        Assert.Throws<ComponentLookupException>(() => list.FindRow("Director"));

        Assert.Equal(["Sarah Philips", "Ben Jones"], list.GetRowsWithKey("Director").Select(r => r.Value));
    }

    [Fact]
    public async Task GetAction_NotFound_ThrowsWithActionsOnTheRow()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips", Action("Change", "/name", "name")));
        var row = doc.GetSummaryListRow("Name");

        var ex = Assert.Throws<ComponentLookupException>(() => row.GetAction("Remove"));

        Assert.Equal("Summary list action not found.", ex.Failure.Summary);
        Assert.Equal("an action with the text \"Remove\"", ex.Failure.Expected);
        Assert.Equal("actions with the text \"Change\"", ex.Failure.Found);
    }

    [Fact]
    public async Task SummaryCards_ReadTitleActionsAndRows()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateSummaryListAsync(new SummaryListOptions
            {
                Card = new SummaryListOptionsCard
                {
                    Title = new SummaryListOptionsCardTitle { Text = "Director 1" },
                    Actions = new SummaryListOptionsCardActions
                    {
                        Items = [new SummaryListOptionsCardActionsItem { Text = "Delete director", Href = "/directors/1/delete" }]
                    }
                },
                Rows = [Row("Name", "Sarah Philips", Action("Change", "/directors/1/name", "name"))]
            }),
            g => g.GenerateSummaryListAsync(new SummaryListOptions
            {
                Card = new SummaryListOptionsCard
                {
                    Title = new SummaryListOptionsCardTitle { Text = "Director 2" },
                    Actions = new SummaryListOptionsCardActions
                    {
                        Items =
                        [
                            new SummaryListOptionsCardActionsItem { Text = "Delete director", Href = "/directors/2/delete" },
                            new SummaryListOptionsCardActionsItem { Text = "Withdraw", Href = "/directors/2/withdraw" }
                        ]
                    }
                },
                Rows = [Row("Name", "Ben Jones")]
            }));

        Assert.Equal(["Director 1", "Director 2"], doc.SummaryCards.Select(c => c.Title));
        Assert.Equal(2, doc.SummaryLists.Count);

        var first = doc.GetSummaryCard("Director 1");
        var delete = Assert.Single(first.Actions);
        Assert.Equal("Delete director", delete.Text);
        Assert.Equal("(Director 1)", delete.VisuallyHiddenText);

        var name = first.GetRow("Name");
        Assert.Equal("Sarah Philips", name.Value);
        Assert.Equal("name (Director 1)", name.ChangeLink!.VisuallyHiddenText);
        Assert.Equal("Director 1", first.SummaryList!.Card!.Title);

        var second = doc.GetSummaryCard("Director 2");
        Assert.Equal(["Delete director", "Withdraw"], second.Actions.Select(a => a.Text));
        Assert.Equal("/directors/2/withdraw", second.GetAction("Withdraw").Href);
        Assert.Equal("Ben Jones", second.GetRow("Name").Value);

        // The same key in two cards is ambiguous across the page but not within a card.
        Assert.Throws<ComponentLookupException>(() => doc.GetSummaryListRow("Name"));
        Assert.Equal("Ben Jones", second.Element.GetSummaryListRow("Name").Value);
    }

    [Fact]
    public async Task GetSummaryCard_NotFound_ThrowsWithTitlesOnThePage()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateSummaryListAsync(new SummaryListOptions
        {
            Card = new SummaryListOptionsCard { Title = new SummaryListOptionsCardTitle { Text = "Director 1" } },
            Rows = [Row("Name", "Sarah Philips")]
        }));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetSummaryCard("Director 2"));

        Assert.Equal(
            """
            Summary card not found.
            Expected: a summary card with the title "Director 2"
            Found:    summary cards with the titles "Director 1"
            """,
            ex.Message);
        Assert.Null(doc.FindSummaryCard("Director 2"));
    }

    [Fact]
    public async Task SummaryList_NotInCard_HasNullCard()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        Assert.Null(doc.SummaryLists.Single().Card);
        Assert.Empty(doc.SummaryCards);
    }

    [Fact]
    public void GetSummaryListRow_NoSummaryLists_ThrowsWithNoRows()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetSummaryListRow("Name"));

        Assert.Equal("no rows", ex.Failure.Found);
        Assert.Null(ex.Failure.Html);
        Assert.Null(doc.FindSummaryListRow("Name"));
    }

    private static Task<IHtmlDocument> RenderListAsync(params SummaryListOptionsRow[] rows) =>
        Render.DocumentAsync(g => g.GenerateSummaryListAsync(new SummaryListOptions { Rows = rows }));

    private static SummaryListOptionsRow Row(string key, string value, params SummaryListOptionsRowActionsItem[] actions) =>
        new()
        {
            Key = new SummaryListOptionsRowKey { Text = key },
            Value = new SummaryListOptionsRowValue { Text = value },
            Actions = actions.Length > 0 ? new SummaryListOptionsRowActions { Items = actions } : null
        };

    private static SummaryListOptionsRow Row(string key, IHtmlContent value, params SummaryListOptionsRowActionsItem[] actions) =>
        Row(key, "", actions) with { Value = new SummaryListOptionsRowValue { Html = value } };

    private static SummaryListOptionsRowActionsItem Action(string text, string href, string visuallyHiddenText) =>
        new() { Text = text, Href = href, VisuallyHiddenText = visuallyHiddenText };
}
