using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class TableTests
{
    [Fact]
    public async Task Table_ReadsCaptionHeadersAndRows()
    {
        var doc = await RenderTableAsync();

        var table = Assert.Single(doc.Tables);

        Assert.Equal("Dates and amounts", table.Caption);
        Assert.Equal(["Date", "Amount"], table.Headers);
        Assert.Equal(2, table.Rows.Count);

        var first = table.Rows[0];
        Assert.Equal(["First 6 weeks", "£109.80 per week"], first.Cells.Select(c => c.Text));
        Assert.True(first.Cells[0].IsHeader);
        Assert.False(first.Cells[1].IsHeader);
        Assert.Equal("£109.80 per week", first["Amount"].Text);

        Assert.Equal("£109.80 per week", table.GetRow("Next 33 weeks").GetCell("Amount").Text);
    }

    [Fact]
    public async Task GetCell_UnknownHeader_ThrowsWithHeaders()
    {
        var doc = await RenderTableAsync();
        var row = doc.Tables.Single().Rows[0];

        var ex = Assert.Throws<ComponentLookupException>(() => row["Total"]);

        Assert.Equal("Table column not found.", ex.Failure.Summary);
        Assert.Equal("a column with the header \"Total\"", ex.Failure.Expected);
        Assert.Equal("columns with the headers \"Date\", \"Amount\"", ex.Failure.Found);
        Assert.StartsWith("<table class=\"govuk-table\">", ex.Failure.Html);
    }

    [Fact]
    public async Task GetCell_WithColSpan_MatchesColumnsByPosition()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateTableAsync(new TableOptions
        {
            Head = [new TableOptionsHead { Text = "Name" }, new TableOptionsHead { Text = "Q1" }, new TableOptionsHead { Text = "Q2" }, new TableOptionsHead { Text = "Total" }],
            Rows =
            [
                new TableOptionsRow { Cells = [new TableOptionsColumn { Text = "A" }, new TableOptionsColumn { Text = "Both", ColSpan = 2 }, new TableOptionsColumn { Text = "10" }] }
            ]
        }));

        var row = doc.Tables.Single().Rows.Single();

        Assert.Equal("Both", row["Q1"].Text);
        Assert.Equal("Both", row["Q2"].Text);
        Assert.Equal("10", row["Total"].Text);
    }

    [Fact]
    public async Task GetTable_ByCaption()
    {
        var doc = await RenderTableAsync();

        Assert.Equal(["Date", "Amount"], doc.GetTable("Dates and amounts").Headers);
        Assert.Null(doc.FindTable("Other"));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetTable("Other"));
        Assert.Equal("tables with the captions \"Dates and amounts\"", ex.Failure.Found);
    }

    [Fact]
    public async Task GetRow_NotFound_ThrowsWithFirstCells()
    {
        var doc = await RenderTableAsync();

        var ex = Assert.Throws<ComponentLookupException>(() => doc.Tables.Single().GetRow("Last week"));

        Assert.Equal("Table row not found.", ex.Failure.Summary);
        Assert.Equal("rows with the first cells \"First 6 weeks\", \"Next 33 weeks\"", ex.Failure.Found);
    }

    private static Task<IHtmlDocument> RenderTableAsync() =>
        Render.DocumentAsync(g => g.GenerateTableAsync(new TableOptions
        {
            Caption = "Dates and amounts",
            FirstCellIsHeader = true,
            Head = [new TableOptionsHead { Text = "Date" }, new TableOptionsHead { Text = "Amount" }],
            Rows =
            [
                new TableOptionsRow { Cells = [new TableOptionsColumn { Text = "First 6 weeks" }, new TableOptionsColumn { Text = "£109.80 per week" }] },
                new TableOptionsRow { Cells = [new TableOptionsColumn { Text = "Next 33 weeks" }, new TableOptionsColumn { Text = "£109.80 per week" }] }
            ]
        }));
}
