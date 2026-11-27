using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class SummaryListAssertionTests
{
    [Fact]
    public async Task SummaryListHasRow_ValueMatches_ReturnsRow()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"), Row("Phone", "07700 900457"));

        var row = Assert.SummaryListHasRow(doc, "Phone", "07700 900457");

        Assert.Equal("Phone", row.Key);
    }

    [Fact]
    public async Task SummaryListHasRow_MultiLineValue_ComparesLinesJoinedWithSpaces()
    {
        var doc = await RenderListAsync(Row("Address", new HtmlString("72 Guild Street<br>London")));

        Assert.SummaryListHasRow(doc, "Address", "72 Guild Street London");
    }

    [Fact]
    public async Task SummaryListHasRow_DifferentValue_FailsWithKeyAndHtml()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        var ex = AssertFailure.Throws(
            """
            Assert.SummaryListHasRow() Failure: Value differs
            Key:      "Name"
            Expected: "Ben Jones"
            Actual:   "Sarah Philips"
            """,
            () => Assert.SummaryListHasRow(doc, "Name", "Ben Jones"),
            htmlOf: doc.GetSummaryListRowByKey("Name").Element);

        AssertFailure.StackTraceStartsIn(nameof(SummaryListHasRow_DifferentValue_FailsWithKeyAndHtml), ex);
    }

    [Fact]
    public async Task SummaryListHasRow_RowNotFound_FailsWithKeys()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"), Row("Phone", "07700 900457"));

        AssertFailure.Throws(
            """
            Assert.SummaryListHasRow() Failure: Summary list row not found
            Expected: a row with the key "Email address"
            Actual:   rows with the keys "Name", "Phone"
            """,
            () => Assert.SummaryListHasRow(doc, "Email address", "sarah@example.com"));
    }

    [Fact]
    public async Task SummaryListHasRow_ScopedToCard_ChecksThatCardsRow()
    {
        var doc = await RenderCardsAsync();

        Assert.SummaryListHasRow(doc.GetSummaryCardByTitle("Director 2").Element, "Name", "Ben Jones");
    }

    [Fact]
    public async Task SummaryListHasRows_SameRowsInOrder_ReturnsList()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"), Row("Phone", "07700 900457"));

        var list = Assert.SummaryListHasRows(doc, ("Name", "Sarah Philips"), ("Phone", "07700 900457"));

        Assert.Equal(2, list.Rows.Count);
    }

    [Fact]
    public async Task SummaryListHasRows_PassedTheListsElement_ChecksIt()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        Assert.SummaryListHasRows(doc.SummaryLists[0].Element, ("Name", "Sarah Philips"));
    }

    [Fact]
    public async Task SummaryListHasRows_RowsDiffer_FailsWithRowsAndHtml()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"), Row("Email address", "sarah@example.com"));

        var ex = AssertFailure.Throws(
            """
            Assert.SummaryListHasRows() Failure: Rows differ
            Expected: "Name": "Sarah Philips"
                      "Phone": "07700 900457"
            Actual:   "Name": "Sarah Philips"
                      "Email address": "sarah@example.com"
            """,
            () => Assert.SummaryListHasRows(doc, ("Name", "Sarah Philips"), ("Phone", "07700 900457")),
            htmlOf: doc.SummaryLists[0].Element);

        AssertFailure.StackTraceStartsIn(nameof(SummaryListHasRows_RowsDiffer_FailsWithRowsAndHtml), ex);
    }

    [Fact]
    public async Task SummaryListHasRows_NoRowsExpected_FailsWithNoRows()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        AssertFailure.Throws(
            """
            Assert.SummaryListHasRows() Failure: Rows differ
            Expected: no rows
            Actual:   "Name": "Sarah Philips"
            """,
            () => Assert.SummaryListHasRows(doc),
            htmlOf: doc.SummaryLists[0].Element);
    }

    [Fact]
    public void SummaryListHasRows_NoSummaryList_Fails()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        AssertFailure.Throws(
            """
            Assert.SummaryListHasRows() Failure: Summary list not found
            Expected: a summary list
            Actual:   no summary lists
            """,
            () => Assert.SummaryListHasRows(doc, ("Name", "Sarah Philips")));
    }

    [Fact]
    public async Task SummaryListHasRows_MoreThanOneSummaryList_FailsWithHint()
    {
        var doc = await RenderCardsAsync();

        AssertFailure.Throws(
            """
            Assert.SummaryListHasRows() Failure: More than one summary list found
            Expected: one summary list
            Actual:   2 summary lists
            Hint:     pass a summary card's Element, or another element that contains one summary list
            """,
            () => Assert.SummaryListHasRows(doc, ("Name", "Sarah Philips")));
    }

    [Fact]
    public async Task SummaryListHasRows_ScopedToCard_ChecksThatCardsList()
    {
        var doc = await RenderCardsAsync();

        Assert.SummaryListHasRows(doc.GetSummaryCardByTitle("Director 2").Element, ("Name", "Ben Jones"));
    }

    [Fact]
    public async Task SummaryListHasRow_ScopeIsTheList_ChecksItsRows()
    {
        var doc = await RenderListAsync(Row("Name", "Sarah Philips"));

        Assert.SummaryListHasRow(doc.SummaryLists.Single().Element, "Name", "Sarah Philips");
    }

    private static Task<IHtmlDocument> RenderListAsync(params SummaryListOptionsRow[] rows) =>
        Render.DocumentAsync(g => g.GenerateSummaryListAsync(new SummaryListOptions { Rows = rows }));

    private static Task<IHtmlDocument> RenderCardsAsync() =>
        Render.DocumentAsync(
            g => g.GenerateSummaryListAsync(new SummaryListOptions
            {
                Card = new SummaryListOptionsCard { Title = new SummaryListOptionsCardTitle { Text = "Director 1" } },
                Rows = [Row("Name", "Sarah Philips")]
            }),
            g => g.GenerateSummaryListAsync(new SummaryListOptions
            {
                Card = new SummaryListOptionsCard { Title = new SummaryListOptionsCardTitle { Text = "Director 2" } },
                Rows = [Row("Name", "Ben Jones")]
            }));

    private static SummaryListOptionsRow Row(string key, string value) =>
        new()
        {
            Key = new SummaryListOptionsRowKey { Text = key },
            Value = new SummaryListOptionsRowValue { Text = value }
        };

    private static SummaryListOptionsRow Row(string key, IHtmlContent value) =>
        Row(key, "") with { Value = new SummaryListOptionsRowValue { Html = value } };
}
