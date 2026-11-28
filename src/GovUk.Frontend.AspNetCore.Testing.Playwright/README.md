# Playwright helpers for GovUk.Frontend.AspNetCore

Locators, actions and assertions for testing ASP.NET Core applications that use [GovUk.Frontend.AspNetCore](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore) to render the GOV.UK Design System, in a browser with [Playwright](https://playwright.dev/dotnet/).

The locators are extension members on `IPage` and `ILocator`, and find components by the text a user sees:

```cs
await page.GetTextInputByLabel("Full name").Input.FillAsync("Sarah Philips");
await page.FillDateInputByLegendAsync("Date of birth", new DateOnly(2000, 3, 27));
await page.CheckRadioByLabelAsync("How would you like to be contacted?", "Email");
await page.SubmitFormByButtonTextAsync("Continue");

await Expect(page.ErrorSummary.Items).ToHaveErrorSummaryErrorsAsync(["Enter an email address"]);
await Expect(page.GetFieldByLabelOrLegend("Email address").ErrorMessage).ToHaveErrorMessageAsync("Enter an email address");
await Expect(page.GetSummaryListRowByKey("Name").Value).ToHaveTextAsync("Sarah Philips");
```

Each locator is precise enough for Playwright's own matchers to give useful failure messages.
`ToHaveErrorMessageAsync` ignores the visually hidden `Error:` before a field's error message, and it and `ToHaveErrorSummaryErrorsAsync` add a line to Playwright's failure message that says which component was checked.

The package adds a global using for `GovUk.Frontend.AspNetCore.Testing.Playwright`. To opt out, add `<Using Remove="GovUk.Frontend.AspNetCore.Testing.Playwright" />` to an `ItemGroup` in your project file.

This package is versioned in lockstep with GovUk.Frontend.AspNetCore, since the locators find the markup that version renders.
Upgrade the packages together.

See the [Playwright docs](https://github.com/x-govuk/govuk-frontend-aspnetcore/blob/main/docs/testing/playwright.md) for every locator, action and assertion.
