# ASP.NET Core integration for GOV.UK Design System

This library handles two things for you when using the [GOV.UK Design System](https://design-system.service.gov.uk/) in an ASP.NET Core application:
getting the GOV.UK Frontend assets into your project, and rendering the components.
The assets are copied in when your project builds, and the components are written as tag helpers that work with your view model.

```razor
@* Labels, hints and error messages come from the model *@

<govuk-input for="EmailAddress">
    <govuk-input-label>Email address</govuk-input-label>
    <govuk-input-hint>We'll only use this to send you a receipt</govuk-input-hint>
</govuk-input>

<govuk-button>Submit</govuk-button>
```

See the [full documentation](https://github.com/x-govuk/govuk-frontend-aspnetcore/blob/main/README.md) for installation and usage guides.

Built and maintained by [X-GOVUK](https://x-govuk.org/).
