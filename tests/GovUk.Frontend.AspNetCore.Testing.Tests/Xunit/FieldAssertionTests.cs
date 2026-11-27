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
            Hint:     there's a field labelled "Email address"; use FieldByLabelHasError to find it by its label or legend
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
            Hint:     there's a field labelled "Email address"; use FieldByLabelHasNoError to find it by its label or legend
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
    public async Task FieldByLabelHasError_ErrorMatches_ReturnsField()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        var field = Assert.FieldByLabelHasError(doc, "Email address", "Enter an email address");

        Assert.IsType<TextInput>(field);
    }

    [Fact]
    public async Task FieldByLabelHasError_FieldsBoundWithFor_FindsEachKindByLabelOrLegend()
    {
        var doc = await GetAsync("/Fields?errors=true");

        foreach (var field in doc.Fields.Where(f => f.Label is not null))
        {
            Assert.Same(field.Element, Assert.FieldByLabelHasError(doc, field.Label!, field.ErrorMessage!).Element);
        }
    }

    [Fact]
    public async Task FieldByLabelHasError_DifferentError_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address in the correct format");

        var ex = AssertFailure.Throws(
            """
            Assert.FieldByLabelHasError() Failure: Error message differs
            Field:    Text input: Email address
            Expected: "Enter an email address"
            Actual:   "Enter an email address in the correct format"
            """,
            () => Assert.FieldByLabelHasError(doc, "Email address", "Enter an email address"),
            htmlOf: doc.GetTextInputByLabel("Email address").Element);

        AssertFailure.StackTraceStartsIn(nameof(FieldByLabelHasError_DifferentError_FailsWithFieldAndHtml), ex);
    }

    [Fact]
    public async Task FieldByLabelHasError_FieldNotFound_FailsWithFieldsOnThePage()
    {
        var doc = await RenderAsync();

        var ex = AssertFailure.Throws(
            """
            Assert.FieldByLabelHasError() Failure: Field not found
            Expected: a field with the label or legend "Phone"
            Actual:   fields labelled "Full name" (text input), "Email address" (text input)
            """,
            () => Assert.FieldByLabelHasError(doc, "Phone", "Enter a phone number"));

        AssertFailure.StackTraceStartsIn(nameof(FieldByLabelHasError_FieldNotFound_FailsWithFieldsOnThePage), ex);
    }

    [Fact]
    public async Task FieldByLabelHasError_NameInsteadOfLabel_FailsWithHint()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.FieldByLabelHasError() Failure: Field not found
            Expected: a field with the label or legend "Email"
            Actual:   fields labelled "Full name" (text input), "Email address" (text input)
            Hint:     there's a field named "Email"; use FieldHasError to find it by its name
            """,
            () => Assert.FieldByLabelHasError(doc, "Email", "Enter an email address"));
    }

    [Fact]
    public async Task FieldByLabelHasNoError_NoError_ReturnsField()
    {
        var doc = await RenderAsync();

        var field = Assert.FieldByLabelHasNoError(doc, "Email address");

        Assert.Equal("Email", field.Name);
    }

    [Fact]
    public async Task FieldByLabelHasNoError_Error_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        AssertFailure.Throws(
            """
            Assert.FieldByLabelHasNoError() Failure: Field has an error
            Field:    Text input: Email address
            Expected: no error message
            Actual:   "Enter an email address"
            """,
            () => Assert.FieldByLabelHasNoError(doc, "Email address"),
            htmlOf: doc.GetTextInputByLabel("Email address").Element);
    }

    [Fact]
    public async Task FieldByLabelHasNoError_NameInsteadOfLabel_FailsWithHint()
    {
        var doc = await RenderAsync();

        AssertFailure.Throws(
            """
            Assert.FieldByLabelHasNoError() Failure: Field not found
            Expected: a field with the label or legend "Email"
            Actual:   fields labelled "Full name" (text input), "Email address" (text input)
            Hint:     there's a field named "Email"; use FieldHasNoError to find it by its name
            """,
            () => Assert.FieldByLabelHasNoError(doc, "Email"));
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
