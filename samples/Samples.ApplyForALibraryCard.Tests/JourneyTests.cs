using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Samples.ApplyForALibraryCard.Tests;

/// <summary>
/// Tests the journey over HTTP, submitting each page's form as a browser would.
/// </summary>
public class JourneyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    // Each test has its own client, so its own cookies and session. Redirects aren't followed automatically, so the
    // tests can check where each page goes.
    private readonly HttpClient _client = factory.CreateClient(new() { AllowAutoRedirect = false });

    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task CompleteJourney_SendsApplication()
    {
        var start = await GetPageAsync("/");
        var name = await GetPageAsync(start.GetButtonByText("Start now").Href!);

        // Each submission starts from the form on the page, so it has the antiforgery token
        var dateOfBirth = await SubmitAsync(
            name.GetFormByButtonText("Continue").CreateSubmission()
                .Set("FullName", "Sarah Philips"),
            expectedNextPage: "/date-of-birth");

        var contact = await SubmitAsync(
            dateOfBirth.GetFormByButtonText("Continue").CreateSubmission()
                .Set("DateOfBirth", new DateOnly(2000, 3, 27)),
            expectedNextPage: "/contact");

        var checkAnswers = await SubmitAsync(
            contact.GetFormByButtonText("Continue").CreateSubmission()
                .Set("ContactMethod", ContactMethod.Email)
                .Set("Email", "sarah@example.com"),
            expectedNextPage: "/check-answers");

        Assert.SummaryListHasRows(
            checkAnswers,
            ("Full name", "Sarah Philips"),
            ("Date of birth", "27 March 2000"),
            ("Contact by", "Email sarah@example.com"));

        var confirmation = await SubmitAsync(
            checkAnswers.GetFormByButtonText("Accept and send").CreateSubmission(),
            expectedNextPage: "/confirmation");

        var panel = Assert.IsType<Panel>(confirmation.Panel);
        Assert.Equal("Application complete", panel.Title);
        Assert.StartsWith("Your reference number", panel.Body);
    }

    [Fact]
    public async Task Name_NotAnswered_ShowsError()
    {
        var name = await GetPageAsync("/name");

        var page = await SubmitWithErrorsAsync(name.GetFormByButtonText("Continue").CreateSubmission());

        Assert.ErrorSummaryIs(page, "Enter your full name");
        Assert.FieldHasError(page, "FullName", "Enter your full name");
    }

    [Fact]
    public async Task DateOfBirth_MissingYear_ShowsErrorAndKeepsWhatWasEntered()
    {
        await AnswerNameAsync();
        var dateOfBirth = await GetPageAsync("/date-of-birth");

        var page = await SubmitWithErrorsAsync(
            dateOfBirth.GetFormByButtonText("Continue").CreateSubmission()
                .Set("DateOfBirth.Day", "27")
                .Set("DateOfBirth.Month", "3"));

        Assert.ErrorSummaryIs(page, "Date of birth must include a year");
        Assert.FieldHasError(page, "DateOfBirth", "Date of birth must include a year");

        // What was entered is shown again, and only the missing part is highlighted
        var dateInput = page.GetDateInputByLegend("What is your date of birth?");
        Assert.Equal("27", dateInput.Day?.Value);
        Assert.Equal("3", dateInput.Month?.Value);
        Assert.True(dateInput.Year?.HasError);
        Assert.False(dateInput.Day?.HasError);
    }

    [Fact]
    public async Task DateOfBirth_InFuture_ShowsError()
    {
        await AnswerNameAsync();
        var dateOfBirth = await GetPageAsync("/date-of-birth");

        var page = await SubmitWithErrorsAsync(
            dateOfBirth.GetFormByButtonText("Continue").CreateSubmission()
                .Set("DateOfBirth", DateOnly.FromDateTime(DateTime.Today).AddYears(1)));

        Assert.FieldHasError(page, "DateOfBirth", "Date of birth must be in the past");
    }

    [Theory]
    [InlineData(ContactMethod.Email, "Email", "Enter an email address")]
    [InlineData(ContactMethod.Phone, "PhoneNumber", "Enter a phone number")]
    public async Task Contact_RevealedFieldNotAnswered_ShowsErrorOnThatField(
        ContactMethod contactMethod,
        string fieldName,
        string expectedError)
    {
        var contact = await GetPageAsync("/contact");

        var page = await SubmitWithErrorsAsync(
            contact.GetFormByButtonText("Continue").CreateSubmission()
                .Set("ContactMethod", contactMethod));

        Assert.ErrorSummaryIs(page, expectedError);
        Assert.FieldHasError(page, fieldName, expectedError);
        Assert.FieldHasNoError(page, "ContactMethod");

        // The option stays selected, so its conditional reveal with the error is shown
        Assert.Equal(contactMethod.ToString(), page.GetRadiosByLegend("How would you like to be contacted?").Value);
    }

    [Fact]
    public async Task ChangeLink_ReturnsToCheckAnswersWithNewAnswer()
    {
        var checkAnswers = await CompleteQuestionsAsync();

        var changeLink = checkAnswers.GetSummaryListRowByKey("Contact by").ChangeLink!;
        Assert.Equal("how you would like to be contacted", changeLink.VisuallyHiddenText);

        var contact = await GetPageAsync(changeLink.Href!);

        // The page shows the answer that was given, and its back link goes back to check answers
        Assert.Equal("Email", contact.GetRadiosByLegend("How would you like to be contacted?").Value);
        Assert.Equal("sarah@example.com", contact.GetTextInputByLabel("Email address").Value);
        Assert.Equal("/check-answers", contact.BackLink?.Href);

        checkAnswers = await SubmitAsync(
            contact.GetFormByButtonText("Continue").CreateSubmission()
                .Set("ContactMethod", ContactMethod.Post),
            expectedNextPage: "/check-answers");

        Assert.SummaryListHasRow(checkAnswers, "Contact by", "Post");
    }

    [Fact]
    public async Task CheckAnswers_BeforeQuestionsAnswered_RedirectsToStart()
    {
        using var response = await _client.GetAsync("/check-answers", CancellationToken);

        Assert.RedirectsTo(response, "/");
    }

    private async Task<IHtmlDocument> GetPageAsync(string url)
    {
        using var response = await _client.GetAsync(url, CancellationToken);
        return await Assert.HtmlResponseAsync(response, cancellationToken: CancellationToken);
    }

    /// <summary>
    /// Submits a form that should be accepted, checks where it redirects to and follows the redirect.
    /// </summary>
    private async Task<IHtmlDocument> SubmitAsync(FormSubmission submission, string expectedNextPage)
    {
        using var response = await _client.SubmitAsync(submission, cancellationToken: CancellationToken);
        Assert.RedirectsTo(response, expectedNextPage);

        using var next = await response.FollowRedirectAsync(_client, CancellationToken);
        return await Assert.HtmlResponseAsync(next, cancellationToken: CancellationToken);
    }

    /// <summary>
    /// Submits a form that should be rejected, so the same page is shown again with its errors.
    /// </summary>
    private async Task<IHtmlDocument> SubmitWithErrorsAsync(FormSubmission submission)
    {
        using var response = await _client.SubmitAsync(submission, cancellationToken: CancellationToken);
        return await Assert.HtmlResponseAsync(response, cancellationToken: CancellationToken);
    }

    private async Task AnswerNameAsync()
    {
        var name = await GetPageAsync("/name");
        await SubmitAsync(
            name.GetFormByButtonText("Continue").CreateSubmission().Set("FullName", "Sarah Philips"),
            expectedNextPage: "/date-of-birth");
    }

    /// <summary>
    /// Answers every question, posting straight to each page, and returns the check answers page.
    /// </summary>
    private async Task<IHtmlDocument> CompleteQuestionsAsync()
    {
        await PostAsync(
            new FormSubmission("/name") { { "FullName", "Sarah Philips" } },
            expectedNextPage: "/date-of-birth");

        await PostAsync(
            new FormSubmission("/date-of-birth") { { "DateOfBirth", new DateOnly(2000, 3, 27) } },
            expectedNextPage: "/contact");

        await PostAsync(
            new FormSubmission("/contact")
            {
                { "ContactMethod", ContactMethod.Email },
                { "Email", "sarah@example.com" },
            },
            expectedNextPage: "/check-answers");

        return await GetPageAsync("/check-answers");

        async Task PostAsync(FormSubmission submission, string expectedNextPage)
        {
            // Posting without requesting the page first needs a token from the page
            await submission.AddAntiforgeryTokenAsync(_client, cancellationToken: CancellationToken);

            using var response = await _client.SubmitAsync(submission, cancellationToken: CancellationToken);
            Assert.RedirectsTo(response, expectedNextPage);
        }
    }
}
