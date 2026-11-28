# Testing helpers for GovUk.Frontend.AspNetCore

Helpers for testing ASP.NET Core applications that use [GovUk.Frontend.AspNetCore](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore) to render the GOV.UK Design System.

The package reads the components on a rendered page, finding them by their labels, legends, keys and headings, and submits forms over HTTP. It works with any test framework:

```cs
var submission = new FormSubmission("/apply/name") { { "Name", "" } };
await submission.AddAntiforgeryTokenAsync(client);

using var response = await client.SubmitAsync(submission);
var doc = await response.GetHtmlDocumentAsync();

Assert.Equal(["Enter your name"], doc.ErrorSummary!.Items.Select(i => i.Text));
Assert.Equal("Enter your name", doc.GetFieldByName("Name").ErrorMessage);
```

When a lookup fails, the exception says what was looked for and what was on the page:

```
Summary list row not found.
Expected: a row with the key "Email address"
Found:    rows with the keys "Name", "Phone"
```

For xUnit v3 assertions built on this package, use [GovUk.Frontend.AspNetCore.Testing.Xunit.v3](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore.Testing.Xunit.v3).

This package is versioned in lockstep with GovUk.Frontend.AspNetCore and depends on exactly the same version, since the helpers read the markup that version renders.
Upgrade both packages together.

See the [testing package docs](https://github.com/x-govuk/govuk-frontend-aspnetcore/blob/main/docs/testing/testing.md) for every component, field and form helper.
