# Playwright

`GovUk.Frontend.AspNetCore.Testing.Playwright` adds locators, actions and assertions for GOV.UK components to [Playwright](https://playwright.dev/dotnet/), for testing in a browser.

```shell
dotnet add package GovUk.Frontend.AspNetCore.Testing.Playwright
```

It depends on `Microsoft.Playwright` only, so it works with any test framework and can test an app that's already deployed.
The package adds a global using for `GovUk.Frontend.AspNetCore.Testing.Playwright`. To opt out, add `<Using Remove="GovUk.Frontend.AspNetCore.Testing.Playwright" />` to an `ItemGroup` in your project file.

See [Testing](README.md) for which package to use and how versions work.

## Example

```cs
await page.GotoAsync("/apply/name");

await page.GetTextInputByLabel("Full name").Input.FillAsync("Sarah Philips");
await page.FillDateInputByLegendAsync("Date of birth", new DateOnly(2000, 3, 27));
await page.CheckRadioByLabelAsync("How would you like to be contacted?", "Email");
await page.SubmitFormByButtonTextAsync("Continue");

await Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter an email address"]);
await Expect(page.GetFieldByLabelOrLegend("Email address").ErrorMessage).ToHaveErrorMessageAsync("Enter an email address");
```

After the form is fixed and submitted:

```cs
await Expect(page.ErrorSummary.Locator).ToHaveCountAsync(0);
await Expect(page.GetSummaryListRowByKey("Name").Value).ToHaveTextAsync("Sarah Philips");
await page.GetSummaryListRowByKey("Name").ChangeLink.ClickAsync();
```

## How components are found

Locators find components by the text a user sees, as Playwright's own `GetByLabel` and `GetByRole` do. Text is matched exactly, allowing for differences in whitespace.
Every locator is an extension member on both `IPage` and `ILocator`, so you can search within part of a page, e.g. `page.GetSummaryCardByTitle("Director 1").Locator.GetSummaryListRowByKey("Email address")`.

Locators are lazy, as Playwright's are, so there are only `Get…` lookups. Check that something isn't there with `ToHaveCountAsync(0)` on its `Locator`.
If a locator matches more than one element, Playwright's strict mode fails the action or assertion that uses it.

Each locator returns a wrapper with the component's `Locator` and an `ILocator` for each of its parts, which you check with Playwright's own matchers, e.g. `Expect(row.Value).ToHaveTextAsync("…")`.
The wrappers aren't `ILocator`s themselves. `BackLink`, `GetButtonByText()`, `GetTagByText()`, `InsetText` and `Tags` return a plain `ILocator`.

### Components

| Locator | Parts |
| --- | --- |
| `ErrorSummary` | `Title`, `Description`, `Items`, `GetLinkByText()` |
| `NotificationBanner` | `Title`, `Heading`, `Content`, `GetLinkByText()` |
| `GetSummaryListRowByKey()` | `Key`, `Value`, `Actions`, `ChangeLink`, `GetActionByText()` |
| `GetSummaryCardByTitle()` | `Title`, `Actions`, `GetActionByText()`, `GetRowByKey()` |
| `Table`, `GetTableByCaption()` | `Caption`, `Headers`, `Rows`, `GetRowByFirstCellText()`; a row's `Cells` and `GetCellByHeader()` |
| `GetTaskListItemByName()` | `Name`, `Link`, `Hint`, `Status` |
| `Pagination` | `Previous`, `Next`, `Items`, `Current`, `GetLinkByNumber()` |
| `Breadcrumbs` | `Items`, `Current`, `GetLinkByText()` |
| `Panel` | `Title`, `Body` |
| `WarningText` | `Text`, without the icon or its visually hidden fallback |
| `GetDetailsBySummary()` | `Summary`, `Text`, `OpenAsync()` |
| `GetAccordionSectionByHeading()` | `Heading`, `Button`, `Summary`, `Content`, `ExpandAsync()` |
| `GetTabByLabel()` | `Tab`, `Panel`, `SelectAsync()` |
| `BackLink`, `GetButtonByText()`, `GetTagByText()`, `InsetText`, `Tags` | |

A summary list action is matched by its visible text, e.g. `Change`, whatever visually hidden text follows it.

### Fields

Find a field by its label, or by its legend for date inputs, radios and checkboxes:

| Locator | Parts |
| --- | --- |
| `GetTextInputByLabel()` | `Input` |
| `GetTextareaByLabel()` | `Textarea` |
| `GetCharacterCountByLabel()` | `Textarea`, `Status` |
| `GetSelectByLabel()` | `Select`, `SelectOptionByTextAsync()` |
| `GetPasswordInputByLabel()` | `Input`, `ShowHideButton` |
| `GetFileUploadByLabel()` | `Input` |
| `GetDateInputByLegend()` | `Inputs`, `Day`, `Month`, `Year`, `FillAsync()`, `FillMonthAndYearAsync()`, `FillDayAndMonthAsync()` |
| `GetRadiosByLegend()`, `GetCheckboxesByLegend()` | `Items`, `GetItemByLabel()`, `GetItemByValue()`; an item's `Input`, `Label`, `Hint` and `ConditionalContent` |
| `GetFieldByLabelOrLegend()` | for a field of any kind |

Every field also has a `Label`, `Hint` and `ErrorMessage`.
A date input's parts are found by the end of their names, so they work with localised labels. `FillAsync(day, month, year)` takes strings, for filling in invalid dates.

`GetFieldByName()` finds a field of any kind by the name it's submitted with, and `GetFieldByName<TLocator>()` finds one of a particular kind, e.g. `page.GetFieldByName<RadiosLocator>("ContactPreference")`.
A date input's name is the prefix of its inputs' names, e.g. `DateOfBirth`. Radios and checkboxes are found whether or not they're in a fieldset, and by the name of any of their inputs.

## Actions

| Action | Does |
| --- | --- |
| `FillFieldByLabelAsync(label, value)`, `FillFieldByNameAsync(name, value)` | Fills in a text input, password input, textarea or character count, whichever kind it is. |
| `FillDateInputByLegendAsync(legend, date)` | Fills in a date input's day, month and year. |
| `CheckRadioByLabelAsync(legend, label)` | Checks a radio. |
| `CheckCheckboxByLabelAsync(legend, label)`, `UncheckCheckboxByLabelAsync(legend, label)` | Checks or unchecks a checkbox. |
| `SelectOptionByTextAsync(label, optionText)` | Selects an option by its text. |
| `SubmitFormByButtonTextAsync(buttonText)` | Clicks a button and waits for the response to the navigation it starts. |
| `SubmitFormAsync(submit)` | Runs `submit`, which must submit a form, and waits for the response to the navigation it starts. |

Clicking a submit button returns as soon as the click has been dispatched, so without waiting, the assertions that follow can run against the page being navigated away from.
The submit actions wait for the response, then let Playwright's web assertions (`Expect(...)`) retry until the new page has rendered. Use web assertions after them, not one-off reads such as `TextContentAsync()`.

## Assertions

Two assertions are added to Playwright's `ILocatorAssertions`:

| Assertion | Call it on | Checks |
| --- | --- | --- |
| `ToHaveErrorMessageAsync(expected)` | a field's `ErrorMessage` | The error message, ignoring the visually hidden `Error:` before it. |
| `ToHaveErrorSummaryErrorsAsync(expected)` | `ErrorSummary.Items` | The error summary's errors are exactly these, in order. |

Both use Playwright's own matchers, so they retry and work with `.Not`. When they fail, they add a line to Playwright's message that says which component was checked, and another when nothing matched the locator:

```
GOV.UK error message assertion for "Enter an email address", ignoring any visually hidden prefix such as "Error:".
No error message was found: the field may not have an error, or the locator may not be a field's ErrorMessage.
```

Playwright's own message follows.

With `.Not`, Playwright's matchers still fail when nothing matches the locator. To check that a field has no error, use `Expect(field.ErrorMessage).ToHaveCountAsync(0)`.
