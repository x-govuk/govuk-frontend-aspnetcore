# xUnit v3 assertions for GovUk.Frontend.AspNetCore

xUnit v3 assertions for testing ASP.NET Core applications that use [GovUk.Frontend.AspNetCore](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore) to render the GOV.UK Design System.

The assertions are added to xUnit's `Assert` class, and their failure messages say what was expected, what was on the page and the HTML that was checked:

```cs
var doc = await Assert.HtmlResponseAsync(response);

Assert.ErrorSummaryContains(doc, "Enter your name");
Assert.FieldHasError(doc, "Full name", "Enter your name");
Assert.FieldWithNameHasError(doc, "Name", "Enter your name");
```

```
Assert.ErrorSummaryContains() Failure: Error not found
Expected: "Enter your name"
Actual:   ["Enter an email address"]
```

Calling them as `Assert.X(...)` needs C# 14, the default for `net10.0`. On older language versions, call them on `GovUkAssert` instead, e.g. `GovUkAssert.ErrorSummaryContains(doc, "…")`.

The package adds a global using for `GovUk.Frontend.AspNetCore.Testing.Xunit`. To opt out, add `<Using Remove="GovUk.Frontend.AspNetCore.Testing.Xunit" />` to an `ItemGroup` in your project file.

This package is versioned in lockstep with GovUk.Frontend.AspNetCore and GovUk.Frontend.AspNetCore.Testing and depends on exactly the same version, since the assertions read the markup that version renders.
Upgrade all the packages together.

See the [full documentation](https://github.com/x-govuk/govuk-frontend-aspnetcore/blob/main/README.md) for installation and usage guides.
