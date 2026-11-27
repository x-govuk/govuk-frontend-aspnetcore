using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class FieldAssertionTests(TestApp app) : IClassFixture<TestApp>
{
    [Fact]
    public async Task FieldHasError_ErrorMatches_ReturnsField()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        var field = Assert.FieldHasError(doc, "Email", "Enter an email address");

        Assert.IsType<TextInput>(field);
        Assert.Equal("Email address", field.Label);
    }

    [Fact]
    public async Task FieldHasError_FieldsBoundWithFor_FindsEachKindByName()
    {
        var doc = await GetAsync("/Fields?errors=true");

        foreach (var field in doc.Fields)
        {
            Assert.Same(field.Element, Assert.FieldHasError(doc, field.Name!, field.ErrorMessage!).Element);
        }
    }

    [Fact]
    public async Task FieldHasError_ItemName_FindsTheField()
    {
        var doc = await GetAsync("/ItemNames");

        Assert.IsType<Checkboxes>(Assert.FieldHasError(doc, "SmsUpdates", "Select how you would like to get updates"));
        Assert.IsType<DateInput>(Assert.FieldHasError(doc, "BirthDay", "Enter your date of birth"));
    }

    [Fact]
    public async Task FieldHasError_DifferentError_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address in the correct format");

        var ex = AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Error message differs
            Field:    Text input: Email address
            Expected: "Enter an email address"
            Actual:   "Enter an email address in the correct format"
            """,
            () => Assert.FieldHasError(doc, "Email", "Enter an email address"),
            htmlOf: doc.GetFieldByName("Email").Element);

        AssertFailure.StackTraceStartsIn(nameof(FieldHasError_DifferentError_FailsWithFieldAndHtml), ex);
    }

    [Fact]
    public async Task FieldHasError_NoError_Fails()
    {
        var doc = await RenderAsync();

        AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Field has no error
            Field:    Text input: Email address
            Expected: "Enter an email address"
            Actual:   no error message
            """,
            () => Assert.FieldHasError(doc, "Email", "Enter an email address"),
            htmlOf: doc.GetFieldByName("Email").Element);
    }

    [Fact]
    public async Task FieldHasError_FieldNotFound_FailsWithNamesOnThePage()
    {
        var doc = await RenderAsync();

        var ex = AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Field not found
            Expected: a field named "Phone"
            Actual:   fields named "Name" (text input), "Email" (text input)
            """,
            () => Assert.FieldHasError(doc, "Phone", "Enter a phone number"));

        AssertFailure.StackTraceStartsIn(nameof(FieldHasError_FieldNotFound_FailsWithNamesOnThePage), ex);
    }

    [Fact]
    public async Task FieldHasError_LabelInsteadOfName_FailsWithHint()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Field not found
            Expected: a field named "Email address"
            Actual:   fields named "Name" (text input), "Email" (text input)
            Hint:     there's a field labelled "Email address"; to find it by its label or legend, pass GetFieldByLabelOrLegend("Email address") to FieldHasError
            """,
            () => Assert.FieldHasError(doc, "Email address", "Enter an email address"));
    }

    [Fact]
    public async Task FieldHasError_Field_DifferentError_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address in the correct format");
        var field = doc.Fields[1];

        var ex = AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Error message differs
            Field:    Text input: Email address
            Expected: "Enter an email address"
            Actual:   "Enter an email address in the correct format"
            """,
            () => Assert.FieldHasError(field, "Enter an email address"),
            htmlOf: field.Element);

        AssertFailure.StackTraceStartsIn(nameof(FieldHasError_Field_DifferentError_FailsWithFieldAndHtml), ex);
    }

    [Fact]
    public async Task FieldHasNoError_NoError_ReturnsField()
    {
        var doc = await RenderAsync();

        var field = Assert.FieldHasNoError(doc, "Email");

        Assert.Equal("Email address", field.Label);
    }

    [Fact]
    public async Task FieldHasNoError_Error_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.FieldHasNoError() Failure: Field has an error
            Field:    Text input: Email address
            Expected: no error message
            Actual:   "Enter an email address"
            """,
            () => Assert.FieldHasNoError(doc, "Email"),
            htmlOf: doc.GetFieldByName("Email").Element);
    }

    [Fact]
    public async Task FieldHasNoError_LabelInsteadOfName_FailsWithHint()
    {
        var doc = await RenderAsync();

        AssertFailure.Throws(
            """
            Assert.FieldHasNoError() Failure: Field not found
            Expected: a field named "Email address"
            Actual:   fields named "Name" (text input), "Email" (text input)
            Hint:     there's a field labelled "Email address"; to find it by its label or legend, pass GetFieldByLabelOrLegend("Email address") to FieldHasNoError
            """,
            () => Assert.FieldHasNoError(doc, "Email address"));
    }

    [Fact]
    public async Task FieldHasNoError_Field_NoError_ReturnsField()
    {
        var doc = await RenderAsync();
        var field = doc.Fields[1];

        Assert.Same(field, Assert.FieldHasNoError(field));
    }

    [Fact]
    public async Task FieldHasNoError_Field_Error_Fails()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");
        var field = doc.Fields[1];

        AssertFailure.Throws(
            """
            Assert.FieldHasNoError() Failure: Field has an error
            Field:    Text input: Email address
            Expected: no error message
            Actual:   "Enter an email address"
            """,
            () => Assert.FieldHasNoError(field),
            htmlOf: field.Element);
    }

    [Fact]
    public async Task FieldHasError_FieldsFoundByLabelOrLegend_ChecksEachKind()
    {
        var doc = await GetAsync("/Fields?errors=true");

        foreach (var field in doc.Fields.Where(f => f.Label is not null))
        {
            Assert.Same(field.Element, Assert.FieldHasError(doc.GetFieldByLabelOrLegend(field.Label!), field.ErrorMessage!).Element);
        }
    }

    private async Task<IHtmlDocument> GetAsync(string url)
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri(url, UriKind.Relative), TestContext.Current.CancellationToken);

        return await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);
    }

    private static Task<IHtmlDocument> RenderAsync(string? emailError = null) =>
        Render.DocumentAsync(
            g => g.GenerateInputAsync(new InputOptions
            {
                Id = "name",
                Name = "Name",
                Label = new LabelOptions { Text = "Full name" }
            }),
            g => g.GenerateInputAsync(new InputOptions
            {
                Id = "email",
                Name = "Email",
                Label = new LabelOptions { Text = "Email address" },
                ErrorMessage = emailError is null ? null : new ErrorMessageOptions { Text = emailError }
            }));
}
