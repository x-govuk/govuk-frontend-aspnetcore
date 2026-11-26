using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class ErrorSummaryTests
{
    [Fact]
    public async Task ErrorSummary_ReadsTitleDescriptionAndItems()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            DescriptionText = "Check the following",
            ErrorList =
            [
                new ErrorSummaryOptionsErrorItem { Text = "Enter your name", Href = "#Name" },
                new ErrorSummaryOptionsErrorItem { Html = new HtmlString("Enter an <em>email</em> address") }
            ]
        }));

        var summary = doc.ErrorSummary;

        Assert.NotNull(summary);
        Assert.Equal("There is a problem", summary.Title);
        Assert.Equal("Check the following", summary.Description);
        Assert.Collection(
            summary.Items,
            item =>
            {
                Assert.Equal("Enter your name", item.Text);
                Assert.Equal("#Name", item.Href);
            },
            item =>
            {
                Assert.Equal("Enter an email address", item.Text);
                Assert.Null(item.Href);
            });
    }

    [Fact]
    public async Task ErrorSummary_WithoutDescription_HasNullDescription()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            ErrorList = [new ErrorSummaryOptionsErrorItem { Text = "Enter your name", Href = "#Name" }]
        }));

        Assert.Null(doc.ErrorSummary!.Description);
    }

    [Fact]
    public void ErrorSummary_NoneOnPage_ReturnsNull()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        Assert.Null(doc.ErrorSummary);
    }

    [Fact]
    public async Task ErrorSummary_MoreThanOne_Throws()
    {
        var options = new ErrorSummaryOptions { TitleText = "There is a problem" };
        var doc = await Render.DocumentAsync(g => g.GenerateErrorSummaryAsync(options), g => g.GenerateErrorSummaryAsync(options));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.ErrorSummary);

        Assert.Equal(
            """
            More than one error summary found.
            Expected: at most one error summary
            Found:    2 error summaries
            """,
            ex.Message);
    }
}
