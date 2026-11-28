# xUnit assertions

`GovUk.Frontend.AspNetCore.Testing.Xunit.v3` adds assertions for GOV.UK components to xUnit v3's `Assert`.
It's built on the [testing package](testing.md), which it brings in, and depends on `xunit.v3.assert` rather than the xUnit runner.

```shell
dotnet add package GovUk.Frontend.AspNetCore.Testing.Xunit.v3
```

The package adds a global using for `GovUk.Frontend.AspNetCore.Testing.Xunit`, so the assertions are found wherever `Assert` is.
To opt out, add `<Using Remove="GovUk.Frontend.AspNetCore.Testing.Xunit" />` to an `ItemGroup` in your project file.
You'll still want `using GovUk.Frontend.AspNetCore.Testing;` for `FormSubmission` and the component lookups.

See [Testing](README.md) for which package to use and how versions work.

## Example

```cs
using GovUk.Frontend.AspNetCore.Testing;
using Microsoft.AspNetCore.Mvc.Testing;

public class ApplyTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task DateOfBirth_WithoutYear_ShowsError()
    {
        var client = factory.CreateClient();

        var submission = new FormSubmission("/apply/date-of-birth")
        {
            { "DateOfBirth.Day", "27" },
            { "DateOfBirth.Month", "3" },
        };
        await submission.AddAntiforgeryTokenAsync(client);

        using var response = await client.SubmitAsync(submission);

        var doc = await Assert.HtmlResponseAsync(response);
        Assert.ErrorSummaryContains(doc, "Date of birth must include a year");
        Assert.FieldHasError(doc, "DateOfBirth", "Date of birth must include a year");
    }

    [Fact]
    public async Task DateOfBirth_Valid_RedirectsToCheckAnswers()
    {
        var client = factory.CreateClient();

        var submission = new FormSubmission("/apply/date-of-birth") { { "DateOfBirth", new DateOnly(2000, 3, 27) } };
        await submission.AddAntiforgeryTokenAsync(client);

        using var response = await client.SubmitAsync(submission);

        Assert.RedirectsTo(response, "/apply/check-answers");

        using var next = await response.FollowRedirectAsync(client);
        var doc = await Assert.HtmlResponseAsync(next);
        Assert.SummaryListHasRow(doc, "Date of birth", "27 March 2000");
    }
}
```

## Assertions

Each assertion takes a document, or an element to search within, such as a summary card's `Element`.
Most return what they matched, so you can check more about it.

### Responses

| Assertion | Checks |
| --- | --- |
| `HtmlResponseAsync(response, expectedStatusCode)` | The status code (`200 OK` by default) and that the response is HTML. Returns the parsed document. |
| `RedirectsTo(response, expectedLocation)` | That the response redirects to `expectedLocation`. A relative location is resolved against the request, as the `Location` header is. |

### Error summary

| Assertion | Checks |
| --- | --- |
| `ErrorSummaryContains(doc, expectedError)` | The error summary lists this error. |
| `ErrorSummaryIs(doc, params expectedErrors)` | The error summary lists exactly these errors, in this order. |
| `NoErrorSummary(doc)` | There's no error summary. |

### Fields

| Assertion | Checks |
| --- | --- |
| `FieldHasError(doc, name, expectedError)` | The field with this name shows this error message. |
| `FieldHasNoError(doc, name)` | The field with this name doesn't show an error message. |
| `FieldHasError(field, expectedError)`, `FieldHasNoError(field)` | The same, for a `FormField` you've already found. |

Fields are found by the name they're submitted with, as `GetFieldByName()` finds them. That's the name your model state error is keyed by, e.g. `DateOfBirth` for a date input, so the same name goes into the `FormSubmission` and comes out in the assertion.
The field can be of any kind, and error messages are compared without their visually hidden `Error:` prefix.

To find a field by its label or legend instead, pass the field in:

```cs
Assert.FieldHasError(doc.GetFieldByLabelOrLegend("Email address"), "Enter an email address");
```

If no field has the name you gave but one has it as its label or legend, the failure says so.

### Summary lists

| Assertion | Checks |
| --- | --- |
| `SummaryListHasRow(doc, key, expectedValue)` | There's one row with this key, and it has this value. |
| `SummaryListHasRows(doc, params expectedRows)` | The summary list has exactly these rows, in this order, e.g. `("Name", "Sarah Philips"), ("Email address", "sarah@example.com")`. |

A `null` or empty expected value is a row with no value.
Values are compared with the row's `Value`, which has its lines joined with spaces.
To check one summary card among several, pass its `Element`:

```cs
Assert.SummaryListHasRow(doc.GetSummaryCardByTitle("Director 1").Element, "Email address", "sarah@example.com");
```

### Notification banners

| Assertion | Checks |
| --- | --- |
| `NotificationBannerContains(doc, expectedText)` | A notification banner contains this text. |
| `SuccessNotificationBannerContains(doc, expectedText)` | A success notification banner contains this text. |

The text has to be a whole line of the banner's content, such as its heading or a paragraph, or all of its content.

### Anything else

Use the [testing package](testing.md)'s lookups with xUnit's own assertions. A lookup that fails throws a `ComponentLookupException` that says what was on the page:

```cs
Assert.Equal("Completed", doc.GetTaskListItemByName("Company directors").Status);
Assert.True(doc.GetCheckboxesByLegend("Nationality").GetItemByLabel("British").IsChecked);
```

## Failure messages

Failures use xUnit's format, and include the HTML that was checked:

```
Assert.FieldHasError() Failure: Error message differs
Field:    Text input: Email address
Expected: "Enter an email address"
Actual:   "Enter an email address in the correct format"
```

## Older C# versions

Calling the assertions as `Assert.X(...)` needs C# 14, the default for .NET 10.
On older language versions, call them on `GovUkAssert` instead, e.g. `GovUkAssert.ErrorSummaryContains(doc, "…")`.
