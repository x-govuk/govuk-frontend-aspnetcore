using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class ErrorSummaryAssertionTests
{
    [Fact]
    public async Task ErrorSummaryContains_ListedError_ReturnsItem()
    {
        var doc = await RenderAsync("Enter your name", "Enter an email address");

        var item = Assert.ErrorSummaryContains(doc, "Enter an email address");

        Assert.Equal("#Field2", item.Href);
    }

    [Fact]
    public async Task ErrorSummaryContains_ErrorNotListed_FailsWithErrorsAndHtml()
    {
        var doc = await RenderAsync("Enter an email address");

        var ex = AssertFailure.Throws(
            """
            Assert.ErrorSummaryContains() Failure: Error not found
            Expected: "Enter your name"
            Actual:   ["Enter an email address"]
            """,
            () => Assert.ErrorSummaryContains(doc, "Enter your name"),
            htmlOf: doc.ErrorSummary!.Element);

        AssertFailure.StackTraceStartsIn(nameof(ErrorSummaryContains_ErrorNotListed_FailsWithErrorsAndHtml), ex);
    }

    [Fact]
    public void ErrorSummaryContains_NoErrorSummary_Fails()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        AssertFailure.Throws(
            """
            Assert.ErrorSummaryContains() Failure: Error summary not found
            Expected: an error summary containing "Enter your name"
            Actual:   no error summary
            """,
            () => Assert.ErrorSummaryContains(doc, "Enter your name"));
    }

    [Fact]
    public void ErrorSummaryContains_NoErrorSummaryInScope_FailsWithScopeHtml()
    {
        var doc = Render.Document("<main><p>Nothing here</p></main>");
        var main = doc.QuerySelector("main")!;

        AssertFailure.Throws(
            """
            Assert.ErrorSummaryContains() Failure: Error summary not found
            Expected: an error summary containing "Enter your name"
            Actual:   no error summary
            """,
            () => Assert.ErrorSummaryContains(main, "Enter your name"),
            htmlOf: main);
    }

    [Fact]
    public async Task ErrorSummaryContains_MoreThanOneErrorSummary_Fails()
    {
        var summary = (await RenderAsync("Enter your name")).Body!.InnerHtml;
        var doc = Render.Document(summary + summary);

        var ex = AssertFailure.Throws(
            """
            Assert.ErrorSummaryContains() Failure: More than one error summary found
            Expected: at most one error summary
            Actual:   2 error summaries
            """,
            () => Assert.ErrorSummaryContains(doc, "Enter your name"));

        AssertFailure.StackTraceStartsIn(nameof(ErrorSummaryContains_MoreThanOneErrorSummary_Fails), ex);
    }

    [Fact]
    public async Task ErrorSummaryIs_SameErrorsInOrder_ReturnsSummary()
    {
        var doc = await RenderAsync("Enter your name", "Enter an email address");

        var summary = Assert.ErrorSummaryIs(doc, "Enter your name", "Enter an email address");

        Assert.Same(doc.ErrorSummary!.Element, summary.Element);
    }

    [Fact]
    public async Task ErrorSummaryIs_DifferentOrder_FailsWithErrorsAndHtml()
    {
        var doc = await RenderAsync("Enter your name", "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.ErrorSummaryIs() Failure: Errors differ
            Expected: ["Enter an email address", "Enter your name"]
            Actual:   ["Enter your name", "Enter an email address"]
            """,
            () => Assert.ErrorSummaryIs(doc, "Enter an email address", "Enter your name"),
            htmlOf: doc.ErrorSummary!.Element);
    }

    [Fact]
    public async Task ErrorSummaryIs_MissingError_Fails()
    {
        var doc = await RenderAsync("Enter your name");

        AssertFailure.Throws(
            """
            Assert.ErrorSummaryIs() Failure: Errors differ
            Expected: ["Enter your name", "Enter an email address"]
            Actual:   ["Enter your name"]
            """,
            () => Assert.ErrorSummaryIs(doc, "Enter your name", "Enter an email address"),
            htmlOf: doc.ErrorSummary!.Element);
    }

    [Fact]
    public void ErrorSummaryIs_NoErrorSummary_Fails()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        AssertFailure.Throws(
            """
            Assert.ErrorSummaryIs() Failure: Error summary not found
            Expected: an error summary with ["Enter your name"]
            Actual:   no error summary
            """,
            () => Assert.ErrorSummaryIs(doc, "Enter your name"));
    }

    [Fact]
    public void ErrorSummaryIs_NoExpectedErrors_ThrowsArgumentException()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        var ex = Assert.Throws<ArgumentException>(() => Assert.ErrorSummaryIs(doc));

        Assert.Equal("expectedErrors", ex.ParamName);
    }

    [Fact]
    public void NoErrorSummary_NoErrorSummary_Passes()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        Assert.NoErrorSummary(doc);
    }

    [Fact]
    public async Task NoErrorSummary_ErrorSummary_FailsWithErrorsAndHtml()
    {
        var doc = await RenderAsync("Enter your name");

        var ex = AssertFailure.Throws(
            """
            Assert.NoErrorSummary() Failure: Error summary found
            Expected: no error summary
            Actual:   ["Enter your name"]
            """,
            () => Assert.NoErrorSummary(doc),
            htmlOf: doc.ErrorSummary!.Element);

        AssertFailure.StackTraceStartsIn(nameof(NoErrorSummary_ErrorSummary_FailsWithErrorsAndHtml), ex);
    }

    [Fact]
    public async Task NoErrorSummary_MoreThanOneErrorSummary_FailsWithTheFirst()
    {
        var first = (await RenderAsync("Enter your name")).Body!.InnerHtml;
        var second = (await RenderAsync("Enter an email address")).Body!.InnerHtml;
        var doc = Render.Document(first + second);

        AssertFailure.Throws(
            """
            Assert.NoErrorSummary() Failure: Error summary found
            Expected: no error summary
            Actual:   ["Enter your name"]
            """,
            () => Assert.NoErrorSummary(doc),
            htmlOf: doc.QuerySelector(".govuk-error-summary"));
    }

    private static Task<IHtmlDocument> RenderAsync(params string[] errors) =>
        Render.DocumentAsync(g => g.GenerateErrorSummaryAsync(new ErrorSummaryOptions
        {
            TitleText = "There is a problem",
            ErrorList = errors.Select((e, i) => new ErrorSummaryOptionsErrorItem { Text = e, Href = $"#Field{i + 1}" }).ToArray()
        }));
}
