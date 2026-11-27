using System.Text;
using GovUk.Frontend.AspNetCore.Testing.Tests.Pages;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class FormSubmissionTests
{
    [Fact]
    public void Add_FormatsValuesAsModelBindingReadsThem()
    {
        var submission = new FormSubmission("/apply")
        {
            { "Name", "Sarah Philips" },
            { "DateOfBirth", new DateOnly(2000, 3, 7) },
            { "Nationality", ["british", null, "irish"] },
            { "Agree", true },
            { "Disagree", false },
            { "Contact", ContactMethod.Phone },
            { "Amount", 1234.5m },
            { "Count", 3 },
            { "When", new DateTime(2026, 11, 26, 9, 30, 0, DateTimeKind.Utc) },
            { "Ids", new[] { 1, 2 } },
            { "Missing", (string?)null },
            { "MissingDate", (DateOnly?)null },
        };

        Assert.Equal(
            [
                new("Name", "Sarah Philips"),
                new("DateOfBirth.Day", "7"),
                new("DateOfBirth.Month", "3"),
                new("DateOfBirth.Year", "2000"),
                new("Nationality", "british"),
                new("Nationality", "irish"),
                new("Agree", "true"),
                new("Disagree", "false"),
                new("Contact", "Phone"),
                new("Amount", "1234.5"),
                new("Count", "3"),
                new("When", "2026-11-26T09:30:00.0000000Z"),
                new("Ids", "1"),
                new("Ids", "2"),
            ],
            submission.Values);
    }

    [Fact]
    public void Add_UsesInvariantCulture()
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");

        try
        {
            var submission = new FormSubmission("/apply") { { "Amount", 1234.5m } };

            Assert.Equal(["1234.5"], submission.GetValuesFor("Amount"));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = culture;
        }
    }

    [Fact]
    public void Add_NestedCollection_Throws()
    {
        var submission = new FormSubmission("/apply");

        var ex = Assert.Throws<ArgumentException>(() => submission.Add("Ids", new[] { new[] { 1 } }));

        Assert.StartsWith("The values for 'Ids' can't contain nested collections.", ex.Message);
    }

    [Fact]
    public void Add_PartialDateTuple_Throws()
    {
        var submission = new FormSubmission("/apply");

        var ex = Assert.Throws<ArgumentException>(() => submission.Add("Issued", (3, 2020)));

        Assert.StartsWith(
            "A (int, int) value for 'Issued' doesn't say whether it's a day and month or a month and year. " +
            "Add the 'Issued.Day', 'Issued.Month' or 'Issued.Year' fields separately.",
            ex.Message);
        Assert.Empty(submission.Values);
    }

    [Fact]
    public void Set_PartialDateTuple_Throws()
    {
        var submission = new FormSubmission("/apply") { { "Issued.Month", "3" } };

        Assert.Throws<ArgumentException>(() => submission.Set("Issued", (3, 2020)));
        Assert.Equal(["3"], submission.GetValuesFor("Issued.Month"));
    }

    [Fact]
    public void Set_ReplacesValuesInPlace()
    {
        var submission = new FormSubmission("/apply")
        {
            { "Name", "Sarah" },
            { "Nationality", ["british", "irish"] },
            { "Country", "wales" },
        };

        submission.Set("Nationality", ["other"]).Set("Email", "sarah@example.com");

        Assert.Equal(
            [new("Name", "Sarah"), new("Nationality", "other"), new("Country", "wales"), new("Email", "sarah@example.com")],
            submission.Values);
    }

    [Fact]
    public void Set_Date_ReplacesEachPart()
    {
        var submission = new FormSubmission("/apply")
        {
            { "Name", "Sarah" },
            { "DateOfBirth.Day", "31" },
            { "DateOfBirth.Month", "2" },
            { "DateOfBirth.Year", "2000" },
        };

        submission.Set("DateOfBirth", new DateOnly(2000, 3, 27));

        Assert.Equal(
            [new("Name", "Sarah"), new("DateOfBirth.Day", "27"), new("DateOfBirth.Month", "3"), new("DateOfBirth.Year", "2000")],
            submission.Values);

        submission.Set("DateOfBirth", (DateOnly?)null);

        Assert.Equal([new("Name", "Sarah")], submission.Values);
    }

    [Fact]
    public void Remove_RemovesValuesAndFiles()
    {
        var submission = new FormSubmission("/apply") { { "Name", "Sarah" }, { "Upload", "x" } }
            .AddFile("Upload", "hello"u8.ToArray(), "hello.txt");

        Assert.True(submission.Remove("Upload"));
        Assert.False(submission.Remove("Upload"));

        Assert.Equal([new("Name", "Sarah")], submission.Values);
        Assert.Empty(submission.Files);
    }

    [Fact]
    public void Constructor_OtherMethod_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => new FormSubmission("/apply", HttpMethod.Put));

        Assert.StartsWith("A form can only be submitted with GET or POST.", ex.Message);
    }

    [Fact]
    public async Task ToRequest_Post_IsUrlEncoded()
    {
        var submission = new FormSubmission("/apply") { { "Name", "Sarah Philips" }, { "Note", "a&b=c" } };

        using var request = submission.ToRequest();

        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("/apply", request.RequestUri!.OriginalString);
        Assert.Equal("application/x-www-form-urlencoded", request.Content!.Headers.ContentType!.MediaType);
        Assert.Equal("Name=Sarah+Philips&Note=a%26b%3Dc", await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ToRequest_WithFile_IsMultipart()
    {
        var submission = new FormSubmission("/apply") { { "Name", "Sarah" } }
            .AddFile("Upload", new MemoryStream(Encoding.UTF8.GetBytes("hello")), "hello.txt", "text/plain");

        using var request = submission.ToRequest();

        Assert.True(submission.IsMultipart);
        Assert.Equal("multipart/form-data", request.Content!.Headers.ContentType!.MediaType);
        var body = await request.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("name=Upload; filename=hello.txt", body, StringComparison.Ordinal);
        Assert.Contains("hello", body, StringComparison.Ordinal);
    }

    [Fact]
    public void AddFile_HttpContent_UsesItsContentType()
    {
        using var content = new StringContent("hello", Encoding.UTF8, "text/plain");

        var submission = new FormSubmission("/apply").AddFile("Upload", content, "hello.txt");

        var file = Assert.Single(submission.Files);
        Assert.Equal("Upload", file.Name);
        Assert.Equal("hello.txt", file.FileName);
        Assert.Equal("text/plain", file.ContentType);
        Assert.Equal("hello"u8.ToArray(), file.Content.ToArray());
        Assert.True(submission.IsMultipart);
    }

    [Fact]
    public void AddFile_HttpContentWithContentType_UsesTheGivenContentType()
    {
        using var content = new StringContent("hello", Encoding.UTF8, "text/plain");

        var submission = new FormSubmission("/apply").AddFile("Upload", content, "hello.csv", "text/csv");

        Assert.Equal("text/csv", Assert.Single(submission.Files).ContentType);
    }

    [Fact]
    public void AddFile_HttpContentWithoutContentType_IsOctetStream()
    {
        using var content = new ByteArrayContent([1, 2, 3]);

        var submission = new FormSubmission("/apply").AddFile("Upload", content, "data.bin");

        Assert.Equal("application/octet-stream", Assert.Single(submission.Files).ContentType);
    }

    [Fact]
    public void ToRequest_Get_PutsValuesInQueryString()
    {
        var submission = new FormSubmission("/search?page=2#results", HttpMethod.Get) { { "q", "head teacher" }, { "type", ["a", "b"] } };

        using var request = submission.ToRequest();

        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Null(request.Content);
        Assert.Equal("/search?q=head%20teacher&type=a&type=b", request.RequestUri!.OriginalString);
    }

    [Fact]
    public void ToRequest_GetWithFile_Throws()
    {
        var submission = new FormSubmission("/search", HttpMethod.Get).AddFile("Upload", [1], "a.bin");

        var ex = Assert.Throws<InvalidOperationException>(() => submission.ToRequest());

        Assert.Equal("Files can't be submitted with GET.", ex.Message);
    }
}
