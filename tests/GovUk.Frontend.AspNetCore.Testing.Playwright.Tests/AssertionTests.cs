namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

public class AssertionTests(BrowserApp app, ITestOutputHelper output)
{
    private static readonly LocatorAssertionsToHaveTextOptions Quickly = new() { Timeout = 500 };

    [Fact]
    public async Task ToHaveErrorMessageAsync_IgnoresTheVisuallyHiddenPrefix()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var errorMessage = page.GetFieldByLabelOrLegend("Full name").ErrorMessage;

        await Expect(errorMessage).ToContainTextAsync("Error:");
        await Expect(errorMessage).ToHaveErrorMessageAsync("Enter your full name");
        await Expect(errorMessage).ToHaveErrorMessageAsync("  Enter   your full name ");
    }

    [Fact]
    public async Task ToHaveErrorMessageAsync_DoesNotMatchPartOfTheMessage()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var errorMessage = page.GetFieldByLabelOrLegend("Full name").ErrorMessage;

        await Expect(errorMessage).Not.ToHaveErrorMessageAsync("full name", Quickly);
        await Expect(errorMessage).Not.ToHaveErrorMessageAsync("Enter your full", Quickly);
    }

    [Fact]
    public async Task ToHaveErrorMessageAsync_WhenTheMessageDiffers_SaysWhichComponentWasChecked()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage)
                .ToHaveErrorMessageAsync("Enter your name", Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal(
            "GOV.UK error message assertion for \"Enter your name\", ignoring any visually hidden prefix such as \"Error:\".",
            lines[0]);
        Assert.StartsWith("Locator expected to have text matching regex", lines[1], StringComparison.Ordinal);
        Assert.Contains("Error: Enter your full name", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("No error message was found", ex.Message, StringComparison.Ordinal);
        Assert.IsType<PlaywrightException>(ex.InnerException, exactMatch: false);
    }

    [Fact]
    public async Task ToHaveErrorMessageAsync_WhenThereIsNoError_SaysSo()
    {
        var page = await GotoAsync("/Fields");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage)
                .ToHaveErrorMessageAsync("Enter your full name", Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal(
            "GOV.UK error message assertion for \"Enter your full name\", ignoring any visually hidden prefix such as \"Error:\".",
            lines[0]);
        Assert.Equal(
            "No error message was found: the field may not have an error, or the locator may not be a field's ErrorMessage.",
            lines[1]);
        Assert.Contains("element(s) not found", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ToHaveErrorMessageAsync_WithNot_WhenTheMessageMatches_ReadsCorrectly()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage)
                .Not.ToHaveErrorMessageAsync("Enter your full name", Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal(
            "GOV.UK error message assertion for \"Enter your full name\", ignoring any visually hidden prefix such as \"Error:\".",
            lines[0]);
        Assert.StartsWith("Locator expected not to have text matching regex", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ToHaveErrorMessageAsync_WithNot_WhenThereIsNoError_FailsAsPlaywrightsMatchersDo()
    {
        var page = await GotoAsync("/Fields");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage)
                .Not.ToHaveErrorMessageAsync("Enter your full name", Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal(
            "No error message was found: the field may not have an error, or the locator may not be a field's ErrorMessage.",
            lines[1]);
    }

    [Fact]
    public async Task ToHaveErrorSummaryErrorsAsync_MatchesEveryErrorInOrder()
    {
        var page = await GotoAsync("/Components");

        var items = page.ErrorSummary.Items;

        await Expect(items).ToHaveErrorSummaryErrorsAsync(["Enter your full name", "Enter an email address"]);
        await Expect(items).Not.ToHaveErrorSummaryErrorsAsync(["Enter an email address", "Enter your full name"], Quickly);
        await Expect(items).Not.ToHaveErrorSummaryErrorsAsync(["Enter your full name"], Quickly);
        await Expect(items).Not.ToHaveErrorSummaryErrorsAsync(["Enter your full", "Enter an email address"], Quickly);
    }

    [Fact]
    public async Task ToHaveErrorSummaryErrorsAsync_WhenTheErrorsDiffer_SaysWhichComponentWasChecked()
    {
        var page = await GotoAsync("/Components");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter your full name"], Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal("GOV.UK error summary assertion for the errors [\"Enter your full name\"].", lines[0]);
        Assert.StartsWith("Locator expected to have text", lines[1], StringComparison.Ordinal);
        Assert.Contains("Enter an email address", ex.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("No errors were found", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ToHaveErrorSummaryErrorsAsync_WhenThereIsNoErrorSummary_SaysSo()
    {
        var page = await GotoAsync("/Empty");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter your full name"], Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal("GOV.UK error summary assertion for the errors [\"Enter your full name\"].", lines[0]);
        Assert.Equal(
            "No errors were found: the page may not have an error summary, or the locator may not be an error summary's Items.",
            lines[1]);
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
