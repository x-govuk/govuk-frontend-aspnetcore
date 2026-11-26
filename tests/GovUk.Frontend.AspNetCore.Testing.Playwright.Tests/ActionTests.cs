namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

public class ActionTests(BrowserApp app)
{
    [Fact]
    public async Task Actions_FillInTheFormAndSubmitIt()
    {
        var page = await GotoAsync("/Fields");

        await page.GetTextInputByLabel("Full name").Input.FillAsync("Ana Lopez");
        await page.FillDateInputByLegendAsync("Date of birth", new DateOnly(1999, 12, 5));
        await page.CheckRadioByLabelAsync("How would you like to be contacted?", "Phone");
        await page.CheckCheckboxByLabelAsync("What is your nationality?", "Citizen of another country");
        await page.UncheckCheckboxByLabelAsync("What is your nationality?", "Irish");
        await page.SelectOptionByTextAsync("Country", "England");

        await page.SubmitFormByButtonTextAsync("Continue");

        // The page re-renders what was posted
        await Expect(page.GetTextInputByLabel("Full name").Input).ToHaveValueAsync("Ana Lopez");
        var date = page.GetDateInputByLegend("Date of birth");
        await Expect(date.Day).ToHaveValueAsync("5");
        await Expect(date.Month).ToHaveValueAsync("12");
        await Expect(date.Year).ToHaveValueAsync("1999");
        await Expect(page.GetRadiosByLegend("How would you like to be contacted?").GetItemByLabel("Phone").Input).ToBeCheckedAsync();
        var nationality = page.GetCheckboxesByLegend("What is your nationality?");
        await Expect(nationality.GetItemByLabel("British").Input).ToBeCheckedAsync();
        await Expect(nationality.GetItemByLabel("Irish").Input).Not.ToBeCheckedAsync();
        await Expect(nationality.GetItemByLabel("Citizen of another country").Input).ToBeCheckedAsync();
        await Expect(page.GetSelectByLabel("Country").Select).ToHaveValueAsync("england");
        await Expect(page.ErrorSummary.Locator).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task SubmitFormByButtonTextAsync_ThenWebAssertions_SeeTheNewPage()
    {
        var page = await GotoAsync("/Fields");

        await page.GetTextInputByLabel("Full name").Input.FillAsync("");
        await page.SubmitFormByButtonTextAsync("Continue");

        await Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter your full name"]);
        await Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage).ToHaveErrorMessageAsync("Enter your full name");
    }

    [Fact]
    public async Task DateInputFillAsync_WithParts_FillsInvalidValues()
    {
        var page = await GotoAsync("/Fields");

        var date = page.GetDateInputByLegend("Date of birth");
        await date.FillAsync("x", null, "2022");

        await Expect(date.Day).ToHaveValueAsync("x");
        await Expect(date.Month).ToHaveValueAsync("3");
        await Expect(date.Year).ToHaveValueAsync("2022");
    }

    [Fact]
    public async Task DateInputFillAsync_PartialDates_FillsAndSubmitsThem()
    {
        var page = await GotoAsync("/Fields");

        await page.GetDateInputByLegend("When was your passport issued?").FillMonthAndYearAsync(11, 2021);
        await page.GetDateInputByLegend("When is your birthday?").FillDayAndMonthAsync(29, 2);
        await page.SubmitFormByButtonTextAsync("Continue");

        // The page re-renders what was bound, so these values went through model binding
        var issued = page.GetDateInputByLegend("When was your passport issued?");
        await Expect(issued.Month).ToHaveValueAsync("11");
        await Expect(issued.Year).ToHaveValueAsync("2021");
        var birthday = page.GetDateInputByLegend("When is your birthday?");
        await Expect(birthday.Day).ToHaveValueAsync("29");
        await Expect(birthday.Month).ToHaveValueAsync("2");
        await Expect(page.ErrorSummary.Locator).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task DateInputFillAsync_WithDate_LeavesOutPartsTheDateInputDoesNotHave()
    {
        var page = await GotoAsync("/Fields");

        var issued = page.GetDateInputByLegend("When was your passport issued?");
        await issued.FillAsync(new DateOnly(2021, 11, 5));

        await Expect(issued.Month).ToHaveValueAsync("11");
        await Expect(issued.Year).ToHaveValueAsync("2021");
    }

    [Fact]
    public async Task DateInputFillAsync_WithPartTheDateInputDoesNotHave_FailsWithoutFillingAnything()
    {
        var page = await GotoAsync("/Fields");

        var birthday = page.GetDateInputByLegend("When is your birthday?");
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => birthday.FillAsync("1", "2", "2020"));

        Assert.Equal("Can't fill in the year of the date input because it doesn't have a year input.", ex.Message);
        await Expect(birthday.Day).ToHaveValueAsync("27");
        await Expect(birthday.Month).ToHaveValueAsync("3");
    }

    [Fact]
    public async Task DateInputFillMonthAndYearAsync_OnDayAndMonthInput_Fails()
    {
        var page = await GotoAsync("/Fields");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            page.GetDateInputByLegend("When is your birthday?").FillMonthAndYearAsync(3, 2020));

        Assert.Equal("Can't fill in the year of the date input because it doesn't have a year input.", ex.Message);
    }

    [Fact]
    public async Task FillDateInputByLegendAsync_WhenThereIsNoSuchDateInput_Fails()
    {
        var page = await GotoAsync("/Fields");
        page.SetDefaultTimeout(500);

        var ex = await Assert.ThrowsAsync<TimeoutException>(() =>
            page.FillDateInputByLegendAsync("Date of death", new DateOnly(2020, 1, 1)));

        Assert.Contains("Timeout 500ms exceeded", ex.Message, StringComparison.Ordinal);
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
