using System.Net;
using System.Text.Json;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

/// <summary>
/// Submits forms to the test app, both directly and from a page, and follows redirects.
/// </summary>
public class SubmissionTests(TestApp app) : IClassFixture<TestApp>
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task SubmitAsync_Direct_IsBoundByTheApp()
    {
        using var client = app.CreateClientWithCookies();

        var submission = new FormSubmission("/Apply")
        {
            { "Name", "Sarah Philips" },
            { "DateOfBirth", new DateOnly(2000, 3, 27) },
            { "Nationality", ["british", "irish"] },
            { "Contact", Pages.ContactMethod.Email },
            { "Agree", true },
            { "Amount", 12.5m },
        };
        await submission.AddAntiforgeryTokenAsync(client, cancellationToken: CancellationToken);

        using var response = await client.SubmitAsync(submission, cancellationToken: CancellationToken);
        var json = await ReadJsonAsync(response);

        Assert.Equal("default", json.GetProperty("handler").GetString());
        Assert.True(json.GetProperty("valid").GetBoolean());
        Assert.Equal("Sarah Philips", json.GetProperty("name").GetString());
        Assert.Equal("2000-03-27", json.GetProperty("dateOfBirth").GetString());
        Assert.Equal(["british", "irish"], json.GetProperty("nationality").EnumerateArray().Select(e => e.GetString()));
        Assert.Equal("Email", json.GetProperty("contact").GetString());
        Assert.True(json.GetProperty("agree").GetBoolean());
        Assert.Equal(12.5m, json.GetProperty("amount").GetDecimal());
    }

    [Fact]
    public async Task SubmitAsync_DirectWithFile_IsBoundByTheApp()
    {
        using var client = app.CreateClientWithCookies();

        var submission = new FormSubmission("/Apply") { { "Name", "Sarah" } }
            .AddFile("Upload", "hello"u8.ToArray(), "hello.txt", "text/plain");
        await submission.AddAntiforgeryTokenAsync(client, cancellationToken: CancellationToken);

        using var response = await client.SubmitAsync(submission, cancellationToken: CancellationToken);
        var json = await ReadJsonAsync(response);

        Assert.Equal("multipart/form-data", json.GetProperty("contentType").GetString());
        Assert.Equal("hello.txt (text/plain): hello", json.GetProperty("upload").GetString());
        Assert.Equal("Sarah", json.GetProperty("name").GetString());
    }

    [Fact]
    public async Task SubmitAsync_WithoutAntiforgeryToken_IsRejectedByTheApp()
    {
        using var client = app.CreateClientWithCookies();

        using var response = await client.SubmitAsync(new FormSubmission("/Apply") { { "Name", "Sarah" } }, cancellationToken: CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAntiforgeryTokenAsync_ReturnsTheFormsToken()
    {
        using var client = app.CreateClientWithCookies();
        var doc = await GetAsync(client, "/Apply");

        var token = await client.GetAntiforgeryTokenAsync("/Apply", cancellationToken: CancellationToken);

        // Each request gets a new token, so only its shape can be checked.
        Assert.NotEmpty(token);
        Assert.NotEmpty(doc.GetHiddenInputValue(FormSubmission.DefaultAntiforgeryFieldName));
    }

    [Fact]
    public async Task GetAntiforgeryTokenAsync_NoToken_ListsHiddenInputs()
    {
        using var client = app.CreateClient();

        var ex = await Assert.ThrowsAsync<ComponentLookupException>(
            () => client.GetAntiforgeryTokenAsync("/Fields", "Missing", CancellationToken));

        Assert.Equal("Hidden input not found.", ex.Failure.Summary);
        Assert.Equal("a hidden input named \"Missing\"", ex.Failure.Expected);
        Assert.Equal("hidden inputs with the names \"__RequestVerificationToken\"", ex.Failure.Found);
    }

    [Fact]
    public async Task GetAntiforgeryTokenAsync_RequestFails_Throws()
    {
        using var client = app.CreateClient();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.GetAntiforgeryTokenAsync("/Missing", cancellationToken: CancellationToken));

        Assert.Equal(
            "Requesting http://localhost/Missing for an antiforgery token returned status code 404 NotFound.",
            ex.Message);
    }

    [Fact]
    public async Task CreateSubmission_SendsTheFormsCurrentValues()
    {
        using var client = app.CreateClientWithCookies();
        var doc = await GetAsync(client, "/Apply");

        var submission = doc.GetForm("Continue").CreateSubmission();

        Assert.Equal(HttpMethod.Post, submission.Method);
        Assert.Equal("http://localhost/Apply", submission.Url.ToString());
        Assert.False(submission.Multipart);
        Assert.Equal(
            [
                "Reference", "Name", "DateOfBirth.Day", "DateOfBirth.Month", "DateOfBirth.Year", "Nationality", "Contact",
                "Country", "Action", FormSubmission.DefaultAntiforgeryFieldName
            ],
            submission.Values.Select(v => v.Key));
        Assert.Equal(["continue"], submission.GetValuesFor("Action"));

        submission.Set("Name", "Sam Philips").Set("DateOfBirth", new DateOnly(1999, 12, 31));
        using var response = await client.SubmitAsync(submission, cancellationToken: CancellationToken);
        var json = await ReadJsonAsync(response);

        Assert.True(json.GetProperty("valid").GetBoolean());
        Assert.Equal("ABC123", json.GetProperty("reference").GetString());
        Assert.Equal("Sam Philips", json.GetProperty("name").GetString());
        Assert.Equal("1999-12-31", json.GetProperty("dateOfBirth").GetString());
        Assert.Equal(["irish"], json.GetProperty("nationality").EnumerateArray().Select(e => e.GetString()));
        Assert.Equal("Phone", json.GetProperty("contact").GetString());
        Assert.Equal("wales", json.GetProperty("country").GetString());
        Assert.Equal(JsonValueKind.Null, json.GetProperty("notes").ValueKind);
        Assert.Equal("continue", json.GetProperty("action").GetString());

        // The page is unchanged.
        Assert.Equal("Sarah Philips", doc.GetTextInput("Full name").Value);
    }

    [Fact]
    public async Task CreateSubmission_ButtonWithFormAction_UsesItsAction()
    {
        using var client = app.CreateClientWithCookies();
        var doc = await GetAsync(client, "/Apply");

        var form = doc.GetForm("Save as draft");
        Assert.Equal("Save as draft", form.Button!.Text);
        Assert.Equal(["Continue", "Save as draft"], form.Buttons.Select(b => b.Text));

        var submission = form.CreateSubmission();
        Assert.Equal("http://localhost/Apply?handler=Draft", submission.Url.ToString());
        Assert.Empty(submission.GetValuesFor("Action"));

        using var response = await client.SubmitAsync(submission, cancellationToken: CancellationToken);
        Assert.Equal("draft", (await ReadJsonAsync(response)).GetProperty("handler").GetString());
    }

    [Fact]
    public async Task CreateSubmission_ByButtonText_UsesThatButton()
    {
        using var client = app.CreateClientWithCookies();
        var doc = await GetAsync(client, "/Apply");

        var submission = doc.GetForm("Continue").CreateSubmission("Save as draft");

        Assert.Equal("http://localhost/Apply?handler=Draft", submission.Url.ToString());
    }

    [Fact]
    public async Task CreateSubmission_UnknownButtonText_ListsButtons()
    {
        using var client = app.CreateClient();
        var doc = await GetAsync(client, "/Apply");

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetForm("Continue").CreateSubmission("Submit"));

        Assert.Equal("Submit button not found.", ex.Failure.Summary);
        Assert.Equal("a submit button with the text \"Submit\"", ex.Failure.Expected);
        Assert.Equal("submit buttons with the text \"Continue\", \"Save as draft\"", ex.Failure.Found);
        Assert.StartsWith("<form", ex.Failure.Html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateSubmission_Multipart_ReadsValuesAndLeavesOutEmptyFileUpload()
    {
        using var client = app.CreateClientWithCookies();
        var doc = await GetAsync(client, "/Apply?multipart=true");

        var submission = doc.GetForm("Continue").CreateSubmission();

        Assert.True(submission.Multipart);
        Assert.DoesNotContain(submission.Values, v => v.Key == "Upload");
        Assert.Equal(["Sarah Philips"], submission.GetValuesFor("Name"));

        submission.AddFile("Upload", "hello"u8.ToArray(), "hello.txt", "text/plain");
        using var response = await client.SubmitAsync(submission, cancellationToken: CancellationToken);
        var json = await ReadJsonAsync(response);

        Assert.Equal("multipart/form-data", json.GetProperty("contentType").GetString());
        Assert.Equal("hello.txt (text/plain): hello", json.GetProperty("upload").GetString());
        Assert.Equal("Sarah Philips", json.GetProperty("name").GetString());
        Assert.Equal("ABC123", json.GetProperty("reference").GetString());
    }

    [Fact]
    public async Task CreateSubmission_Get_PutsValuesInQueryString()
    {
        using var client = app.CreateClient();
        var doc = await GetAsync(client, "/Apply");

        var submission = doc.GetForm("Search").CreateSubmission();

        Assert.Equal(HttpMethod.Get, submission.Method);
        Assert.Equal("http://localhost/Search", submission.Url.ToString());
        Assert.Equal([new("q", "teachers")], submission.Values);

        submission.Set("q", "head teacher");
        using var request = submission.ToRequest();
        Assert.Equal("http://localhost/Search?q=head%20teacher", request.RequestUri!.AbsoluteUri);
    }

    [Fact]
    public async Task GetForm_NotFound_ListsButtons()
    {
        using var client = app.CreateClient();
        var doc = await GetAsync(client, "/Apply");

        Assert.Null(doc.FindForm("Submit"));
        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetForm("Submit"));

        Assert.Equal(
            """
            Form not found.
            Expected: a form with the button "Submit"
            Found:    forms with the buttons "Continue", "Save as draft", "Search", "Sign out"
            """.ReplaceLineEndings("\n"),
            ex.Message);
    }

    [Fact]
    public async Task AllForms_ReturnsEveryFormWithoutAButton()
    {
        using var client = app.CreateClient();
        var doc = await GetAsync(client, "/Apply");

        var forms = doc.AllForms;

        Assert.Equal(
            [["Continue", "Save as draft"], ["Search"], ["Sign out"]],
            forms.Select(f => f.Buttons.Select(b => b.Text).ToArray()));
        Assert.All(forms, f => Assert.Null(f.Button));

        // Submitted with the first submit button.
        Assert.Equal(["continue"], forms[0].CreateSubmission().GetValuesFor("Action"));
        Assert.Equal("http://localhost/SignOut", forms[2].CreateSubmission().Url.ToString());
    }

    [Fact]
    public async Task GetForm_ScopedToElement_SearchesOnlyThatElement()
    {
        using var client = app.CreateClient();
        var doc = await GetAsync(client, "/Apply");

        var form = doc.GetForm("Sign out");

        Assert.Null(form.Element.FindForm("Continue"));
        Assert.Same(form.Element, doc.Body!.GetForm("Sign out").Element);
    }

    [Theory]
    [InlineData(301)]
    [InlineData(302)]
    [InlineData(303)]
    public async Task FollowRedirectAsync_SeeOther_FollowsWithGet(int statusCode)
    {
        using var client = app.CreateClient();
        using var response = await client.SubmitAsync(
            new FormSubmission($"/Redirect/{statusCode}/0/done") { { "a", "1" } },
            cancellationToken: CancellationToken);

        Assert.Equal(statusCode, (int)response.StatusCode);
        Assert.Equal("http://localhost/Echo/done", response.GetRedirectLocation()!.ToString());

        using var redirected = await response.FollowRedirectAsync(client, CancellationToken);

        Assert.Equal("GET /Echo/done", await redirected.Content.ReadAsStringAsync(CancellationToken));
    }

    [Theory]
    [InlineData(307)]
    [InlineData(308)]
    public async Task FollowRedirectAsync_Temporary_RepeatsMethodAndBody(int statusCode)
    {
        using var client = app.CreateClient();
        using var response = await client.SubmitAsync(
            new FormSubmission($"/Redirect/{statusCode}/0/done") { { "a", "1" } },
            cancellationToken: CancellationToken);

        using var redirected = await response.FollowRedirectAsync(client, CancellationToken);

        Assert.Equal("POST /Echo/done a=1", await redirected.Content.ReadAsStringAsync(CancellationToken));
    }

    [Fact]
    public async Task FollowRedirectAsync_NotARedirect_Throws()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync("/Echo/x", CancellationToken);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => response.FollowRedirectAsync(client, CancellationToken));

        Assert.Equal("Expected a redirect but the status code was 200 OK.", ex.Message);
        Assert.Null(response.GetRedirectLocation());
    }

    [Fact]
    public async Task FollowRedirectAsync_NoLocation_Throws()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync("/NoLocation", CancellationToken);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => response.FollowRedirectAsync(client, CancellationToken));

        Assert.Equal("The redirect (302 Found) doesn't have a Location header.", ex.Message);
    }

    [Fact]
    public async Task SubmitAsync_FollowRedirects_FollowsAChain()
    {
        using var client = app.CreateClient();

        using var response = await client.SubmitAsync(
            new FormSubmission("/Redirect/307/3/done") { { "a", "1" } },
            followRedirects: true,
            CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("POST /Echo/done a=1", await response.Content.ReadAsStringAsync(CancellationToken));
    }

    [Fact]
    public async Task SubmitAsync_FollowRedirects_StopsAfterLimit()
    {
        using var client = app.CreateClient();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => client.SubmitAsync(
            new FormSubmission($"/Redirect/302/{HttpClientExtensions.MaxRedirects}/done"),
            followRedirects: true,
            CancellationToken));

        Assert.Equal("Stopped after 50 redirects.", ex.Message);
    }

    private static async Task<IHtmlDocument> GetAsync(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url, CancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.GetHtmlDocumentAsync(CancellationToken);
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(CancellationToken),
            cancellationToken: CancellationToken);

        return json.RootElement.Clone();
    }
}
