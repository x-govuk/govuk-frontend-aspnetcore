namespace GovUk.Frontend.AspNetCore.Testing.Playwright.Tests;

public class FieldLocatorTests(BrowserApp app)
{
    [Fact]
    public async Task GetTextInputByLabel_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetTextInputByLabel("Full name");

        await Expect(field.Label).ToHaveTextAsync("Full name");
        await Expect(field.Hint).ToHaveTextAsync("As it appears on your passport");
        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Enter your full name");
        await Expect(field.Input).ToHaveValueAsync("Sarah Philips");
    }

    [Fact]
    public async Task GetTextInputByLabel_MatchesTheWholeLabel()
    {
        var page = await GotoAsync("/Fields");

        await Expect(page.GetTextInputByLabel("Full").Input).ToHaveCountAsync(0);
        await Expect(page.GetTextInputByLabel("full name").Input).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetTextInputByLabel_DoesNotFindOtherKindsOfField()
    {
        var page = await GotoAsync("/Fields");

        await Expect(page.GetTextInputByLabel("Password").Input).ToHaveCountAsync(0);
        await Expect(page.GetTextInputByLabel("Country").Input).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetTextareaByLabel_FindsTextareasAndCharacterCounts()
    {
        var page = await GotoAsync("/Fields");

        await Expect(page.GetTextareaByLabel("Can you provide more detail?").Textarea).ToHaveValueAsync("Some detail");
        await Expect(page.GetTextareaByLabel("Describe your job").Textarea).ToHaveValueAsync("Teacher");
        await Expect(page.GetCharacterCountByLabel("Can you provide more detail?").Textarea).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetCharacterCountByLabel_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetCharacterCountByLabel("Describe your job");

        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Job description must be 200 characters or fewer");
        // The textarea description isn't the field's hint
        await Expect(field.Hint).ToHaveCountAsync(0);
        await Expect(field.Status).ToHaveTextAsync("You have 193 characters remaining");
    }

    [Fact]
    public async Task GetSelectByLabel_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetSelectByLabel("Country");

        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Select a country");
        await Expect(field.Select).ToHaveValueAsync("wales");
    }

    [Fact]
    public async Task GetPasswordInputByLabel_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetPasswordInputByLabel("Password");

        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Enter a password");
        await Expect(field.Input).ToHaveAttributeAsync("type", "password");

        await field.ShowHideButton.ClickAsync();

        await Expect(field.Input).ToHaveAttributeAsync("type", "text");
    }

    [Fact]
    public async Task GetFileUploadByLabel_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetFileUploadByLabel("Upload a file");

        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Select a file");
        await Expect(field.Input).ToHaveAttributeAsync("type", "file");
    }

    [Fact]
    public async Task GetDateInputByLegend_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetDateInputByLegend("Date of birth");

        await Expect(field.Label).ToHaveTextAsync("Date of birth");
        await Expect(field.Hint).ToHaveTextAsync("For example, 27 3 2007");
        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Date of birth must be in the past");
        await Expect(field.Day).ToHaveValueAsync("27");
        await Expect(field.Month).ToHaveValueAsync("3");
        await Expect(field.Year).ToHaveValueAsync("2000");
        await Expect(field.Inputs).ToHaveCountAsync(3);
    }

    [Fact]
    public async Task GetDateInputByLegend_MonthAndYear_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields");

        var field = page.GetDateInputByLegend("When was your passport issued?");

        await Expect(field.Inputs).ToHaveCountAsync(2);
        await Expect(field.Day).ToHaveCountAsync(0);
        await Expect(field.Month).ToHaveValueAsync("6");
        await Expect(field.Year).ToHaveValueAsync("2018");
    }

    [Fact]
    public async Task GetDateInputByLegend_DayAndMonth_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields");

        var field = page.GetDateInputByLegend("When is your birthday?");

        await Expect(field.Inputs).ToHaveCountAsync(2);
        await Expect(field.Day).ToHaveValueAsync("27");
        await Expect(field.Month).ToHaveValueAsync("3");
        await Expect(field.Year).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetRadiosByLegend_LocatesItsParts()
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetRadiosByLegend("How would you like to be contacted?");

        await Expect(field.Label).ToHaveTextAsync("How would you like to be contacted?");
        await Expect(field.Items).ToHaveCountAsync(2);
        // Not the error of the field in the conditional reveal
        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync("Select how you would like to be contacted");

        var email = field.GetItemByLabel("Email");
        await Expect(email.Input).ToBeCheckedAsync();
        await Expect(email.ConditionalContent).ToBeVisibleAsync();
        await Expect(email.ConditionalContent.GetTextInputByLabel("Email address").ErrorMessage)
            .ToHaveErrorMessageAsync("Enter an email address");

        var phone = field.GetItemByLabel("Phone");
        await Expect(phone.Input).Not.ToBeCheckedAsync();
        await Expect(phone.Hint).ToHaveTextAsync("We'll text you");
        await Expect(phone.ConditionalContent).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task GetCheckboxesByLegend_LocatesItsParts()
    {
        // Not ?errors=true: a model state error without an attempted value leaves every item unchecked
        var page = await GotoAsync("/Fields");

        var field = page.GetCheckboxesByLegend("What is your nationality?");

        await Expect(field.Label).ToHaveTextAsync("What is your nationality?");
        await Expect(field.Items).ToHaveCountAsync(3);
        await Expect(field.Items.Locator("input:checked")).ToHaveCountAsync(2);
        await Expect(field.GetItemByLabel("Citizen of another country").Input).Not.ToBeCheckedAsync();
        await Expect(field.GetItemByLabel("British").Label).ToHaveTextAsync("British");
    }

    [Fact]
    public async Task GetRadiosByLegend_DoesNotFindOtherKindsOfFieldset()
    {
        var page = await GotoAsync("/Fields");

        await Expect(page.GetRadiosByLegend("What is your nationality?").Locator).ToHaveCountAsync(0);
        await Expect(page.GetCheckboxesByLegend("Date of birth").Locator).ToHaveCountAsync(0);
    }

    [Theory]
    [InlineData("Full name", "Enter your full name")]
    [InlineData("Country", "Select a country")]
    [InlineData("Date of birth", "Date of birth must be in the past")]
    [InlineData("How would you like to be contacted?", "Select how you would like to be contacted")]
    [InlineData("Email address", "Enter an email address")]
    [InlineData("What is your nationality?", "Select your nationality")]
    public async Task GetFieldByLabelOrLegend_FindsAnyKindOfField(string labelOrLegend, string expectedError)
    {
        var page = await GotoAsync("/Fields?errors=true");

        var field = page.GetFieldByLabelOrLegend(labelOrLegend);

        await Expect(field.Label).ToHaveTextAsync(labelOrLegend);
        await Expect(field.ErrorMessage).ToHaveErrorMessageAsync(expectedError);
    }

    [Fact]
    public async Task ErrorMessage_WithoutErrors_HasNoElements()
    {
        var page = await GotoAsync("/Fields");

        await Expect(page.GetFieldByLabelOrLegend("Full name").ErrorMessage).ToHaveCountAsync(0);
        await Expect(page.GetRadiosByLegend("How would you like to be contacted?").ErrorMessage).ToHaveCountAsync(0);
    }

    private async Task<IPage> GotoAsync(string url)
    {
        var page = await app.NewPageAsync();
        await page.GotoAsync(url);
        return page;
    }
}
