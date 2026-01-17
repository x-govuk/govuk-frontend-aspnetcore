# ASP.NET Core integration for GOV.UK Design System

This library simplifies setting up an ASP.NET Core application to use the [GOV.UK Design System](https://design-system.service.gov.uk/).
It provides intuitive tag helpers that integrate seamlessly with ASP.NET Core's model binding, enabling you to build accessible, compliant services quickly.
All front-end assets—including fonts, images, CSS and JavaScript, are automatically hosted, so you can focus on building your application.

```razor
@* Build a complete form with labels, hints and error handling in just a few lines *@

<govuk-input for="EmailAddress">
    <govuk-input-label>Email address</govuk-input-label>
    <govuk-input-hint>We'll only use this to send you a receipt</govuk-input-hint>
</govuk-input>

<govuk-button>Submit</govuk-button>
```

See the [full documentation](https://github.com/x-govuk/govuk-frontend-aspnetcore/blob/main/README.md) for installation and usage guides.
