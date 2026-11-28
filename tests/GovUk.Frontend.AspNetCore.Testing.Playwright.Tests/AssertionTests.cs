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

    [Theory]
    [InlineData("Name", "Enter your full name")]
    [InlineData("Detail", "Enter more detail")]
    [InlineData("JobDescription", "Job description must be 200 characters or fewer")]
    [InlineData("Country", "Select a country")]
    [InlineData("Password", "Enter a password")]
    [InlineData("Upload", "Select a file")]
    [InlineData("DateOfBirth", "Date of birth must be in the past")]
    [InlineData("Contact", "Select how you would like to be contacted")]
    [InlineData("Email", "Enter an email address")]
    [InlineData("Nationality", "Select your nationality")]
    public async Task ExpectErrorAsync_GeneratedErrorSummary_LinksToEachKindOfField(string name, string error)
    {
        var page = await GotoAsync("/Fields?errors=true");

        await page.GetFieldByName(name).ExpectErrorAsync(error);
    }

    [Fact]
    public async Task ExpectErrorAsync_SameErrorForTwoFields_PassesWhenOneItemLinksToTheField()
    {
        var page = await SetContentAsync(
            ErrorSummaryHtml(("#name", "Enter an email address"), ("#email", "Enter an email address")));

        await page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address", options: Quickly);
    }

    [Fact]
    public async Task ExpectErrorAsync_NotInErrorSummary_PassesWithoutErrorSummary()
    {
        var page = await SetContentAsync(summaryHtml: "");

        await page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address", inErrorSummary: false, Quickly);
    }

    [Fact]
    public async Task ExpectErrorAsync_WhenTheMessageDiffers_FailsAsToHaveErrorMessageAsyncDoes()
    {
        var page = await SetContentAsync(ErrorSummaryHtml(("#email", "Enter an email address")));

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            page.GetFieldByName("Email").ExpectErrorAsync("Enter your email address", options: Quickly));
        output.WriteLine(ex.Message);

        Assert.StartsWith("GOV.UK error message assertion for \"Enter your email address\"", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExpectErrorAsync_WhenThereIsNoErrorSummary_SaysHowToOptOut()
    {
        var page = await SetContentAsync(summaryHtml: "");

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address", options: Quickly));
        output.WriteLine(ex.Message);

        var lines = ex.Message.Split('\n');
        Assert.Equal("GOV.UK field error assertion: the page has no error summary listing \"Enter an email address\".", lines[0]);
        Assert.Equal("If the error shouldn't be in the error summary, pass inErrorSummary: false to ExpectErrorAsync.", lines[1]);
    }

    [Fact]
    public async Task ExpectErrorAsync_WhenTheErrorSummaryListsOtherErrors_Fails()
    {
        var page = await SetContentAsync(ErrorSummaryHtml(("#name", "Enter your full name")));

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address", options: Quickly));
        output.WriteLine(ex.Message);

        Assert.StartsWith(
            "GOV.UK field error assertion: the page has no error summary listing \"Enter an email address\".",
            ex.Message,
            StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("#name", "a link to \"#name\"")]
    [InlineData("#missing", "a link to \"#missing\"")]
    [InlineData("/elsewhere#email", "a link to \"/elsewhere#email\"")]
    [InlineData(null, "no link")]
    public async Task ExpectErrorAsync_WhenTheErrorSummaryDoesNotLinkToTheField_Fails(string? href, string actual)
    {
        var page = await SetContentAsync(ErrorSummaryHtml((href, "Enter an email address")));

        var ex = await Assert.ThrowsAsync<PlaywrightException>(() =>
            page.GetFieldByName("Email").ExpectErrorAsync("Enter an email address", options: Quickly));
        output.WriteLine(ex.Message);

        Assert.Equal(
            $"""
            GOV.UK field error assertion: the error summary lists "Enter an email address", but doesn't link it to the field.
            Expected: a link to an element in the field
            Actual:   {actual}
            """,
            ex.Message);
    }

    private static string ErrorSummaryHtml(params (string? Href, string Text)[] items) =>
        $"""
        <div class="govuk-error-summary"><div role="alert">
          <h2 class="govuk-error-summary__title">There is a problem</h2>
          <div class="govuk-error-summary__body"><ul class="govuk-list govuk-error-summary__list">
            {string.Concat(items.Select(i => i.Href is null ? $"<li>{i.Text}</li>" : $"<li><a href=\"{i.Href}\">{i.Text}</a></li>"))}
          </ul></div>
        </div></div>
        """;

    /// <summary>
    /// Shows <paramref name="summaryHtml"/>, then a name field and an email field whose error is
    /// <c>Enter an email address</c>.
    /// </summary>
    private async Task<IPage> SetContentAsync(string summaryHtml)
    {
        var page = await app.NewPageAsync();
        await page.SetContentAsync(
            $"""
            <main>
              {summaryHtml}
              <div class="govuk-form-group">
                <label class="govuk-label" for="name">Full name</label>
                <input class="govuk-input" id="name" name="Name" type="text">
              </div>
              <div class="govuk-form-group govuk-form-group--error">
                <label class="govuk-label" for="email">Email address</label>
                <p class="govuk-error-message"><span class="govuk-visually-hidden">Error:</span> Enter an email address</p>
                <input class="govuk-input govuk-input--error" id="email" name="Email" type="text">
              </div>
            </main>
            """);
        return page;
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
