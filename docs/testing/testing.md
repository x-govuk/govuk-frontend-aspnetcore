# Testing package

`GovUk.Frontend.AspNetCore.Testing` reads the components on a rendered page and submits forms, for testing over HTTP.
It doesn't depend on a test framework: use it with your framework's own assertions, or with the [xUnit assertions](xunit.md) built on it.

```shell
dotnet add package GovUk.Frontend.AspNetCore.Testing
```

Everything is in the `GovUk.Frontend.AspNetCore.Testing` namespace.
See [Testing](README.md) for which package to use and how versions work.

## Reading a page

`GetHtmlDocumentAsync()` parses a response with [AngleSharp](https://anglesharp.github.io/).
The status code isn't checked. The document's address is the request's URL, so relative links resolve as they would in a browser.

```cs
using var response = await client.GetAsync("/apply/check-answers");
var doc = await response.GetHtmlDocumentAsync();

Assert.Equal("sarah@example.com", doc.GetSummaryListRowByKey("Email address").Value);
```

The components are extension members on AngleSharp's `IParentNode`, so they work on the document or on any element in it, such as a summary card's `Element`, to search only within that element.
An element counts as containing itself, so a summary list's rows can be found from its own `<dl>` element as well as from an element around it.

### `Get` and `Find`

Every lookup that takes some text comes in two forms:

- `Get…` throws a `ComponentLookupException` when nothing matches, or when more than one thing does.
- `Find…` returns `null` when nothing matches, and still throws when more than one thing does.

The exception says what was looked for, what was on the page and the HTML that was searched:

```
Summary list row not found.
Expected: a row with the key "Email address"
Found:    rows with the keys "Name", "Phone"
```

Text is the text a sighted user sees: whitespace is collapsed, `<br>`s and block elements separate lines, and visually hidden text, such as the `Error:` before an error message or the text after `Change` in a summary list action, is left out. Lookups match that text exactly.

The same failure is available as the exception's `Failure` property, a `LookupFailure` with `Summary`, `Expected`, `Found` and `Html`, for building your own assertion messages.

## Components

Components that usually appear once on a page are properties that return `null` when the component isn't there, and throw when there's more than one.
Other components are lists, or are found by their text.

| Component | Lookups | What it gives you |
| --- | --- | --- |
| Accordion | `Accordion`, `Accordions` | `Sections`, `GetSectionByHeading()`; a section's `Heading`, `Summary`, `Text`, `TextLines` and `IsExpanded` |
| Back link | `BackLink` | `Text`, `Href` |
| Breadcrumbs | `Breadcrumbs` | `Items` and `Current`, each with `Text`, `Href` and `IsCurrent` |
| Button | `GetButtonByText()`, `Buttons` | `Text`, `Href` for a link styled as a button, `Variant` (e.g. `secondary`, `warning`), `IsStartButton`, `IsDisabled` |
| Details | `GetDetailsBySummary()`, `AllDetails` | `Summary`, `Text`, `TextLines`, `IsOpen` |
| Error summary | `ErrorSummary` | `Title`, `Description`, and `Items` with `Text` and `Href` |
| Hidden input | `GetHiddenInputValue()`, `HiddenInputs` | `Name`, `Value` |
| Inset text | `InsetTexts` | `Text`, `TextLines` |
| Notification banner | `NotificationBanner`, `NotificationBanners` | `Title`, `Heading`, `Content`, `ContentLines`, `IsSuccess` |
| Pagination | `Pagination` | `Previous`, `Next`, `Items` and `Current`; an item's `Number`, `Href`, `IsCurrent` and `IsEllipsis` |
| Panel | `Panel` | `Title`, `Body` |
| Summary card | `GetSummaryCardByTitle()`, `SummaryCards` | `Title`, `Actions`, `GetActionByText()`, `SummaryList`, `GetRowByKey()` |
| Summary list | `GetSummaryListRowByKey()`, `SummaryLists` | `Rows`, `GetRowByKey()`, `GetRowsByKey()`, `Card` |
| Summary list row | | `Key`, `Value`, `ValueLines`, `ValueElement`, `Actions`, `ChangeLink`, `GetActionByText()` |
| Table | `GetTableByCaption()`, `Tables` | `Caption`, `Headers`, `Rows`, `GetRowByFirstCellText()`; a row's `Cells` and `row["Header"]` |
| Tabs | `Tabs`, `AllTabs` | `Title`, `Items`, `Selected`, `GetTabByLabel()`; a tab's `Label`, `Text`, `TextLines` and `IsSelected` |
| Tag | `Tags` | `Text`, `Colour` |
| Task list | `GetTaskListItemByName()`, `TaskList`, `TaskLists` | `Items`, `GetItemByName()`; an item's `Name`, `Hint`, `Status`, `StatusTag` and `Href` |
| Warning text | `WarningTexts` | `Text`, without the icon or the visually hidden text read in its place |

Every model has an `Element` property for checking anything else about its markup.
A summary list row's `ValueElement` is there for links or other markup in the value, and a multi-line value is split into `ValueLines`.

```cs
var card = doc.GetSummaryCardByTitle("Director 1");
Assert.Equal("sarah@example.com", card.GetRowByKey("Email address").Value);
Assert.Equal("/directors/1/email", card.GetRowByKey("Email address").ChangeLink?.Href);

Assert.Equal("Completed", doc.GetTaskListItemByName("Company directors").Status);
Assert.Equal("£1,250", doc.GetTableByCaption("Payments").GetRowByFirstCellText("April")["Amount"].Text);
```

## Form fields

Find a field by its label, or by its legend for date inputs, radios and checkboxes:

| Field | Lookup | What it gives you |
| --- | --- | --- |
| Text input | `GetTextInputByLabel()` | `Input`, `Type`, `Value`, `Prefix`, `Suffix` |
| Textarea | `GetTextareaByLabel()` | `TextareaElement`, `Value` |
| Character count | `GetCharacterCountByLabel()` | as a textarea, plus `MaxLength`, `MaxWords`, `Threshold` |
| Select | `GetSelectByLabel()` | `Items`, `SelectedItem`, `Value`, `GetItemByText()` |
| Password input | `GetPasswordInputByLabel()` | `Input`, `Value` |
| File upload | `GetFileUploadByLabel()` | `Input`, `AllowsMultiple`, `Accept` |
| Date input | `GetDateInputByLegend()` | `Day`, `Month`, `Year`, `Value` as a `DateOnly`, `MonthAndYear`, `DayAndMonth` |
| Radios | `GetRadiosByLegend()` | `Items`, `CheckedItem`, `Value`, `GetItemByLabel()` |
| Checkboxes | `GetCheckboxesByLegend()` | `Items`, `CheckedItems`, `Values`, `GetItemByLabel()` |
| Any kind | `GetFieldByLabelOrLegend()`, `Fields` | as below |

Every field is a `FormField`, with its `Label`, `Hint`, `ErrorMessage` (without the visually hidden `Error:` prefix), `Name` and `Names`.
A radio or checkbox item has its `Label`, `Hint`, `Value`, `IsChecked`, `IsDisabled` and `ConditionalElement`, and a checkbox also has `IsExclusive`.

```cs
var email = doc.GetTextInputByLabel("Email address");
Assert.Equal("Enter an email address", email.ErrorMessage);

Assert.Equal(new DateOnly(2000, 3, 27), doc.GetDateInputByLegend("Date of birth").Value);
Assert.Equal("email", doc.GetRadiosByLegend("How would you like to be contacted?").Value);
```

When a field isn't found, the exception lists the fields on the page:

```
Text input not found.
Expected: a text input labelled "Email"
Found:    fields labelled "Email address" (text input), "Date of birth" (date input)
```

### By name

`GetFieldByName()` finds a field of any kind by the name it's submitted with. That's the name model state keys its errors by, so it's often the most stable way to find a field in a test:

```cs
Assert.Equal("Enter your date of birth", doc.GetFieldByName("DateOfBirth").ErrorMessage);
Assert.Equal("email", doc.GetFieldByName<Radios>("ContactPreference").Value);
```

`GetFieldByName<T>()` finds a field of one kind and returns it as that type.
A date input's name is the prefix of its inputs' names, e.g. `DateOfBirth` for `DateOfBirth.Day`.
Radios, checkboxes and date inputs whose inputs have names of their own, e.g. checkboxes named `EmailUpdates` and `SmsUpdates`, are found by any of those names, and `Names` lists every name a field is found by.

## Submitting forms

A `FormSubmission` is a URL, a method and the values to send. Send it with `client.SubmitAsync()`.

### Straight to an endpoint

```cs
var submission = new FormSubmission("/apply/name")
{
    { "Name", "Sarah Philips" },
    { "DateOfBirth", new DateOnly(2000, 3, 27) },
    { "Nationality", ["british", "irish"] },
};
await submission.AddAntiforgeryTokenAsync(client);

using var response = await client.SubmitAsync(submission);
```

Values are formatted as model binding reads them back:

- a `DateOnly` becomes the `.Day`, `.Month` and `.Year` fields of a date input
- `bool`s are `true` or `false`, and enums use their names
- numbers and other `IFormattable` values use the invariant culture
- a collection adds a value for each item
- `null` values are left out

Anything else, such as a tuple or a type without `IFormattable`, throws rather than being posted as its `ToString()`; add it as a string instead.

`Add()` adds a value, `Set()` replaces every value with that name, `Remove()` removes them and `GetValuesFor()` reads them.

`AddAntiforgeryTokenAsync(client)` requests the submission's URL and reads the antiforgery token from the page.
To read it from a different page, use `client.GetAntiforgeryTokenAsync(url)` and add it yourself with `Set(FormSubmission.DefaultAntiforgeryFieldName, token)`.
The token is paired with a cookie, so the client must keep cookies, as clients from `WebApplicationFactory.CreateClient()` do.

### From a form on a page

Starting from a form submits what a browser would: the form's current values, its hidden inputs and antiforgery token, and the name and value of the button that was pressed.

```cs
var doc = await response.GetHtmlDocumentAsync();

var submission = doc.GetFormByButtonText("Continue").CreateSubmission()
    .Set("Name", "Sarah Philips");

using var next = await client.SubmitAsync(submission);
```

`GetFormByButtonText()` finds the form containing a button, and `CreateSubmission()` submits it as that button would, including any `formaction`.
For a form you already have, start from its button: `form.GetButtonByText("Save as draft").CreateSubmission()`. `doc.AllForms` lists every form on the page.
The page itself isn't changed.

### Files

`AddFile(name, content, fileName, contentType)` adds a file from a byte array, a `Stream` or an `HttpContent`. A submission with files is sent as `multipart/form-data`.

```cs
submission.AddFile("Evidence", File.ReadAllBytes("passport.pdf"), "passport.pdf", "application/pdf");
```

### Redirects

`WebApplicationFactory`'s clients follow redirects by default. To check where a successful post goes, create the client with `AllowAutoRedirect` turned off:

```cs
var client = factory.CreateClient(new() { AllowAutoRedirect = false });

using var response = await client.SubmitAsync(submission);

Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
Assert.Equal("/apply/date-of-birth", response.GetRedirectLocation()?.AbsolutePath);

using var next = await response.FollowRedirectAsync(client);
```

- `GetRedirectLocation()` gives the `Location` header, resolved against the request's URL.
- `FollowRedirectAsync(client)` follows one redirect as a browser does: a `301`, `302` or `303` with a `GET`, and a `307` or `308` with the same method and content.
- `client.SubmitAsync(submission, followRedirects: true)` follows every redirect and returns the final response.
