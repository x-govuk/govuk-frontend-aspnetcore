using System.Text.RegularExpressions;
using Microsoft.Playwright.Xunit.v3;

namespace Samples.ApplyForALibraryCard.BrowserTests;

/// <summary>
/// Tests the journey in a browser, with govuk-frontend's JavaScript running.
/// </summary>
public class JourneyTests(JourneyApp app) : PageTest
{
    // Each test gets a new browser context and page, so its own cookies and session
    public override BrowserNewContextOptions ContextOptions() => new() { BaseURL = app.BaseUrl };

    [Fact]
    public async Task CompleteJourney_SendsApplication()
    {
        await Page.GotoAsync("/");

        await Page.GetButtonByText("Start now").ClickAsync();

        await Page.FillFieldByLabelAsync("What is your full name?", "Sarah Philips");
        await Page.SubmitFormByButtonTextAsync("Continue");

        await Page.FillDateInputByLegendAsync("What is your date of birth?", new DateOnly(2000, 3, 27));
        await Page.SubmitFormByButtonTextAsync("Continue");

        await Page.CheckRadioByLabelAsync("How would you like to be contacted?", "Email");
        await Page.GetTextInputByLabel("Email address").Input.FillAsync("sarah@example.com");
        await Page.SubmitFormByButtonTextAsync("Continue");

        await Expect(Page.GetSummaryListRowByKey("Full name").Value).ToHaveTextAsync("Sarah Philips");
        await Expect(Page.GetSummaryListRowByKey("Date of birth").Value).ToHaveTextAsync("27 March 2000");
        await Expect(Page.GetSummaryListRowByKey("Contact by").Value).ToHaveTextAsync("Email sarah@example.com");
        await Page.SubmitFormByButtonTextAsync("Accept and send");

        await Expect(Page.Panel.Title).ToHaveTextAsync("Application complete");
        await Expect(Page.Panel.Body).ToContainTextAsync("Your reference number");
    }

    [Fact]
    public async Task Contact_PhoneWithoutNumber_ShowsErrorInConditionalReveal()
    {
        await Page.GotoAsync("/contact");

        var radios = Page.GetRadiosByLegend("How would you like to be contacted?");
        var phone = radios.GetItemByLabel("Phone");

        // The phone number field is only revealed once Phone is checked
        await Expect(phone.ConditionalContent).ToBeHiddenAsync();
        await phone.Input.CheckAsync();
        await Expect(phone.ConditionalContent).ToBeVisibleAsync();

        await Page.SubmitFormByButtonTextAsync("Continue");

        await Expect(Page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter a phone number"]);
        await Expect(Page.GetTextInputByLabel("Phone number").ErrorMessage).ToHaveErrorMessageAsync("Enter a phone number");
        await Expect(radios.ErrorMessage).ToHaveCountAsync(0);

        // The error summary links to the field with the error
        await Page.ErrorSummary.GetLinkByText("Enter a phone number").ClickAsync();
        await Expect(Page.GetTextInputByLabel("Phone number").Input).ToBeFocusedAsync();
    }

    [Fact]
    public async Task ChangeLink_ReturnsToCheckAnswers()
    {
        await Page.GotoAsync("/name");

        await Page.FillFieldByLabelAsync("What is your full name?", "Sarah Philips");
        await Page.SubmitFormByButtonTextAsync("Continue");
        await Page.FillDateInputByLegendAsync("What is your date of birth?", new DateOnly(2000, 3, 27));
        await Page.SubmitFormByButtonTextAsync("Continue");
        await Page.CheckRadioByLabelAsync("How would you like to be contacted?", "Post");
        await Page.SubmitFormByButtonTextAsync("Continue");

        await Page.GetSummaryListRowByKey("Full name").ChangeLink.ClickAsync();

        // The answer that was given is filled in
        var fullName = Page.GetTextInputByLabel("What is your full name?");
        await Expect(fullName.Input).ToHaveValueAsync("Sarah Philips");

        await fullName.Input.FillAsync("Sarah Phillips");
        await Page.SubmitFormByButtonTextAsync("Continue");

        await Expect(Page).ToHaveURLAsync(new Regex("/check-answers$"));
        await Expect(Page.GetSummaryListRowByKey("Full name").Value).ToHaveTextAsync("Sarah Phillips");
    }
}
