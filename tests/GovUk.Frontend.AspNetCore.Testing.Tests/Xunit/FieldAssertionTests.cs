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
    public async Task FieldHasError_FieldFoundByName_ReturnsField()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");

        var field = Assert.FieldHasError(doc.GetFieldByName("Email"), "Enter an email address");

        Assert.Equal("Email address", field.Label);
    }

    [Fact]
    public async Task FieldHasError_FieldsBoundWithFor_FindsEachKindByName()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/Fields?errors=true", UriKind.Relative), TestContext.Current.CancellationToken);
        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        foreach (var field in doc.Fields)
        {
            Assert.Same(field.Element, Assert.FieldHasError(doc.GetFieldByName(field.Name!), field.ErrorMessage!).Element);
        }
    }

    [Fact]
    public async Task FieldHasError_FieldFoundByAnItemName_ReturnsField()
    {
        using var client = app.CreateClient();
        using var response = await client.GetAsync(new Uri("/ItemNames", UriKind.Relative), TestContext.Current.CancellationToken);
        var doc = await Assert.HtmlResponseAsync(response, cancellationToken: TestContext.Current.CancellationToken);

        Assert.IsType<Checkboxes>(Assert.FieldHasError(doc.GetFieldByName("SmsUpdates"), "Select how you would like to get updates"));
        Assert.IsType<DateInput>(Assert.FieldHasError(doc.GetFieldByName("BirthDay"), "Enter your date of birth"));
    }

    [Fact]
    public async Task FieldHasError_Field_DifferentError_FailsWithFieldAndHtml()
    {
        var doc = await RenderAsync(emailError: "Enter an email address in the correct format");
        var field = doc.GetFieldByName("Email");

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
        var field = doc.GetFieldByName("Email");

        Assert.Same(field, Assert.FieldHasNoError(field));
    }

    [Fact]
    public async Task FieldHasNoError_Field_Error_Fails()
    {
        var doc = await RenderAsync(emailError: "Enter an email address");
        var field = doc.GetFieldByName("Email");

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
