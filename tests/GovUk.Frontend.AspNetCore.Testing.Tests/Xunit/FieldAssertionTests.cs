using AngleSharp.Html.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Testing.Tests.Xunit;

public class FieldAssertionTests(TestApp app) : IClassFixture<TestApp>
{
    [Fact]
    public async Task FieldHasError_ErrorMatches_ReturnsField()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        var field = Assert.FieldHasError(doc, "Email address", "Enter an email address");

        Assert.IsType<TextInput>(field);
    }

    [Fact]
    public async Task FieldHasError_FieldsBoundWithFor_FindsEachKindByLabelOrLegend()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields?errors=true", UriKind.Relative), TestContext.Current.CancellationToken);
        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        foreach (var field in doc.Fields.Where(f => f.Label is not null))
        {
            Assert.Same(field.Element, Assert.FieldHasError(doc, field.Label!, field.ErrorMessage!).Element);
        }
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
            () => Assert.FieldHasError(doc, "Email address", "Enter an email address"),
            htmlOf: doc.GetTextInputByLabel("Email address").Element);

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
            () => Assert.FieldHasError(doc, "Email address", "Enter an email address"),
            htmlOf: doc.GetTextInputByLabel("Email address").Element);
    }

    [Fact]
    public async Task FieldHasError_FieldNotFound_FailsWithFieldsOnThePage()
    {
        var doc = await RenderAsync();

        var ex = AssertFailure.Throws(
            """
            Assert.FieldHasError() Failure: Field not found
            Expected: a field with the label or legend "Email"
            Actual:   fields labelled "Full name" (text input), "Email address" (text input)
            """,
            () => Assert.FieldHasError(doc, "Email", "Enter an email address"));

        AssertFailure.StackTraceStartsIn(nameof(FieldHasError_FieldNotFound_FailsWithFieldsOnThePage), ex);
    }

    [Fact]
    public async Task FieldHasNoError_NoError_ReturnsField()
    {
        var doc = await RenderAsync();

        var field = Assert.FieldHasNoError(doc, "Email address");

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
            () => Assert.FieldHasNoError(doc, "Email address"),
            htmlOf: doc.GetTextInputByLabel("Email address").Element);
    }

    [Fact]
    public async Task FieldHasNoError_FieldNotFound_Fails()
    {
        var doc = await RenderAsync();

        AssertFailure.Throws(
            """
            Assert.FieldHasNoError() Failure: Field not found
            Expected: a field with the label or legend "Email"
            Actual:   fields labelled "Full name" (text input), "Email address" (text input)
            """,
            () => Assert.FieldHasNoError(doc, "Email"));
    }

    [Fact]
    public async Task FieldWithNameHasError_ErrorMatches_ReturnsField()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        var field = Assert.FieldWithNameHasError(doc, "Email", "Enter an email address");

        Assert.Equal("Email address", field.Label);
    }

    [Fact]
    public async Task FieldWithNameHasError_FieldsBoundWithFor_FindsEachKindByName()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields?errors=true", UriKind.Relative), TestContext.Current.CancellationToken);
        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        foreach (var field in doc.Fields)
        {
            Assert.Same(field.Element, Assert.FieldWithNameHasError(doc, field.Name!, field.ErrorMessage!).Element);
        }
    }

    [Fact]
    public async Task FieldWithNameHasError_ItemName_FindsTheField()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/ItemNames", UriKind.Relative), TestContext.Current.CancellationToken);
        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        Assert.IsType<Checkboxes>(Assert.FieldWithNameHasError(doc, "SmsUpdates", "Select how you would like to get updates"));
        Assert.IsType<DateInput>(Assert.FieldWithNameHasError(doc, "BirthDay", "Enter your date of birth"));
    }

    [Fact]
    public async Task FieldWithNameHasError_DifferentError_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address in the correct format");

        var ex = AssertFailure.Throws(
            """
            Assert.FieldWithNameHasError() Failure: Error message differs
            Field:    Text input: Email address
            Expected: "Enter an email address"
            Actual:   "Enter an email address in the correct format"
            """,
            () => Assert.FieldWithNameHasError(doc, "Email", "Enter an email address"),
            htmlOf: doc.GetFieldByName("Email").Element);

        AssertFailure.StackTraceStartsIn(nameof(FieldWithNameHasError_DifferentError_FailsWithFieldAndHtml), ex);
    }

    [Fact]
    public async Task FieldWithNameHasError_FieldNotFound_FailsWithNamesOnThePage()
    {
        var doc = await RenderAsync();

        var ex = AssertFailure.Throws(
            """
            Assert.FieldWithNameHasError() Failure: Field not found
            Expected: a field named "EmailAddress"
            Actual:   fields named "Name" (text input), "Email" (text input)
            """,
            () => Assert.FieldWithNameHasError(doc, "EmailAddress", "Enter an email address"));

        AssertFailure.StackTraceStartsIn(nameof(FieldWithNameHasError_FieldNotFound_FailsWithNamesOnThePage), ex);
    }

    [Fact]
    public async Task FieldWithNameHasNoError_NoError_ReturnsField()
    {
        var doc = await RenderAsync();

        var field = Assert.FieldWithNameHasNoError(doc, "Email");

        Assert.Equal("Email address", field.Label);
    }

    [Fact]
    public async Task FieldWithNameHasNoError_Error_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.FieldWithNameHasNoError() Failure: Field has an error
            Field:    Text input: Email address
            Expected: no error message
            Actual:   "Enter an email address"
            """,
            () => Assert.FieldWithNameHasNoError(doc, "Email"),
            htmlOf: doc.GetFieldByName("Email").Element);
    }

    [Fact]
    public async Task FieldWithNameHasNoError_FieldNotFound_Fails()
    {
        var doc = await RenderAsync();

        AssertFailure.Throws(
            """
            Assert.FieldWithNameHasNoError() Failure: Field not found
            Expected: a field named "EmailAddress"
            Actual:   fields named "Name" (text input), "Email" (text input)
            """,
            () => Assert.FieldWithNameHasNoError(doc, "EmailAddress"));
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
