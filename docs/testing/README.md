# Testing

Three packages help you test an ASP.NET Core app built with this library. They find components the way a user would, by their labels, legends, keys and headings, and when something isn't on the page their failure messages say what was looked for and what was there.

| Package | Use it for | Docs |
| --- | --- | --- |
| `GovUk.Frontend.AspNetCore.Testing` | Reading the components on a rendered page and submitting forms over HTTP, with any test framework | [Testing package](testing.md) |
| `GovUk.Frontend.AspNetCore.Testing.Xunit.v3` | xUnit v3 assertions, added to `Assert`, on top of the testing package | [xUnit assertions](xunit.md) |
| `GovUk.Frontend.AspNetCore.Testing.Playwright` | Locators, actions and assertions for testing in a browser with Playwright | [Playwright](playwright.md) |

Which to reference:

- An xUnit v3 project that tests over HTTP references `GovUk.Frontend.AspNetCore.Testing.Xunit.v3`, which brings in the testing package.
- A project that tests over HTTP with another framework, such as NUnit or MSTest, references `GovUk.Frontend.AspNetCore.Testing` and uses its own framework's assertions.
- A browser test project references `GovUk.Frontend.AspNetCore.Testing.Playwright`.

## Versions

Every package has the same version as `GovUk.Frontend.AspNetCore`, because the helpers find the markup that version renders.
`GovUk.Frontend.AspNetCore.Testing` and `GovUk.Frontend.AspNetCore.Testing.Xunit.v3` depend on exactly that version of the library, so a mismatch is a restore error rather than a test that fails in a confusing way.
`GovUk.Frontend.AspNetCore.Testing.Playwright` doesn't depend on the library, so it can test an app that's already deployed, but you should still keep its version the same as the app's.

The packages need C# 14, the default for .NET 10.

## Testing over HTTP

Host your app with [`WebApplicationFactory`](https://learn.microsoft.com/aspnet/core/test/integration-tests), request a page and read it with the testing package:

```cs
using GovUk.Frontend.AspNetCore.Testing;
using Microsoft.AspNetCore.Mvc.Testing;

public class NameTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Post_WithNoName_ShowsError()
    {
        var client = factory.CreateClient();

        var submission = new FormSubmission("/apply/name") { { "Name", "" } };
        await submission.AddAntiforgeryTokenAsync(client);

        using var response = await client.SubmitAsync(submission);

        var doc = await Assert.HtmlResponseAsync(response);
        Assert.ErrorSummaryIs(doc, "Enter your name");
        Assert.FieldHasError(doc, "Name", "Enter your name");
    }
}
```

`Assert.HtmlResponseAsync`, `Assert.ErrorSummaryIs` and `Assert.FieldHasError` come from the [xUnit package](xunit.md).
Without it, `response.GetHtmlDocumentAsync()` parses the page and `doc.ErrorSummary` and `doc.GetFieldByName("Name")` give you the components to check with your own assertions; see the [testing package](testing.md).

## Testing in a browser

The [Playwright package](playwright.md) adds locators for the components to Playwright's `IPage` and `ILocator`:

```cs
await page.GotoAsync("/apply/name");
await page.SubmitFormByButtonTextAsync("Continue");

await Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter your name"]);
await Expect(page.GetTextInputByLabel("Full name").ErrorMessage).ToHaveErrorMessageAsync("Enter your name");
```
