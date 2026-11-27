using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

/// <summary>
/// Reads fields rendered by the tag helpers, bound to a model with <c>for="…"</c>, as an app renders them.
/// </summary>
public class TagHelperFieldTests(TestApp app) : IClassFixture<TestApp>
{
    [Fact]
    public async Task Fields_HaveLabelsAndValuesFromTheModel()
    {
        var doc = await GetAsync("/Fields");

        Assert.Equal(
            [
                "Full name", "Can you provide more detail?", "Describe your job", "Country", "Password", "Upload a file",
                "Date of birth", "How would you like to be contacted?", "Email address", "What is your nationality?"
            ],
            doc.Fields.Select(f => f.Label));
        Assert.All(doc.Fields, f => Assert.Null(f.ErrorMessage));

        var name = doc.GetTextInputByLabel("Full name");
        Assert.Equal("Sarah Philips", name.Value);
        Assert.Equal("Name", name.Name);
        Assert.Equal("As it appears on your passport", name.Hint);

        Assert.Equal("Some detail", doc.GetTextareaByLabel("Can you provide more detail?").Value);

        var job = doc.GetCharacterCountByLabel("Describe your job");
        Assert.Equal("Teacher", job.Value);
        Assert.Equal(200, job.MaxLength);
        Assert.Null(job.Hint);

        Assert.Equal("wales", doc.GetSelectByLabel("Country").Value);
        Assert.Equal("hunter2", doc.GetPasswordInputByLabel("Password").Value);
        Assert.Equal("Upload", doc.GetFileUploadByLabel("Upload a file").Name);

        var dob = doc.GetDateInputByLegend("Date of birth");
        Assert.Equal(new DateOnly(2000, 3, 27), dob.Value);
        Assert.Equal("For example, 27 3 2007", dob.Hint);
        Assert.Equal(["DateOfBirth.Day", "DateOfBirth.Month", "DateOfBirth.Year"], dob.Items.Select(i => i.Name));
        Assert.Equal([DateInputPart.Day, DateInputPart.Month, DateInputPart.Year], dob.Items.Select(i => i.Part));

        var contact = doc.GetRadiosByLegend("How would you like to be contacted?");
        Assert.Equal("email", contact.Value);
        Assert.Equal(["Email", "Phone"], contact.Items.Select(i => i.Label));
        Assert.Equal("We'll text you", contact.GetItemByLabel("Phone").Hint);
        Assert.Equal("sarah@example.com", contact.GetItemByLabel("Email").ConditionalElement!.GetTextInputByLabel("Email address").Value);

        Assert.Equal(["british", "irish"], doc.GetCheckboxesByLegend("What is your nationality?").Values);
    }

    [Fact]
    public async Task Fields_HaveErrorsFromModelState()
    {
        var doc = await GetAsync("/Fields?errors=true");

        Assert.Equal(
            [
                "Enter your full name", "Enter more detail", "Job description must be 200 characters or fewer",
                "Select a country", "Enter a password", "Select a file", "Date of birth must be in the past",
                "Select how you would like to be contacted", "Enter an email address", "Select your nationality"
            ],
            doc.Fields.Select(f => f.ErrorMessage));

        // An error doesn't stop the label, hint or value being read.
        Assert.Equal("As it appears on your passport", doc.GetFieldByLabelOrLegend("Full name").Hint);
        Assert.Equal("For example, 27 3 2007", doc.GetFieldByLabelOrLegend("Date of birth").Hint);
        Assert.Equal("Sarah Philips", doc.GetTextInputByLabel("Full name").Value);
        Assert.Equal("email", doc.GetRadiosByLegend("How would you like to be contacted?").Value);
    }

    [Fact]
    public async Task Fields_HaveNamesFromTheModel()
    {
        var doc = await GetAsync("/Fields?errors=true");

        Assert.Equal(
            ["Name", "Detail", "JobDescription", "Country", "Password", "Upload", "DateOfBirth", "Contact", "Email", "Nationality"],
            doc.Fields.Select(f => f.Name));

        // Each field is found by the name its model state error is keyed by.
        foreach (var field in doc.Fields)
        {
            Assert.Same(field.Element, doc.GetFieldByName(field.Name!).Element);
        }
    }

    private async Task<IHtmlDocument> GetAsync(string url)
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(url, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.GetHtmlDocumentAsync(TestContext.Current.CancellationToken);
    }
}
