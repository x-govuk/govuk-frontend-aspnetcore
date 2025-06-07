
[![GOV.UK Design System version](https://img.shields.io/badge/GOV.UK%20Design%20System-5.9.0-brightgreen)](https://github.com/alphagov/govuk-frontend/releases/tag/v5.9.0)
[![CI](https://github.com/x-govuk/govuk-frontend-aspnetcore/workflows/ci/badge.svg)](https://github.com/x-govuk/govuk-frontend-aspnetcore/actions/workflows/ci.yml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/GovUk.Frontend.AspNetCore)](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore)

This library simplifies setting up an ASP.NET Core application to use the [GOV.UK Design System](https://design-system.service.gov.uk/).
It includes tag helpers to produce GDS components which integrate with ASP.NET Core's model binding system.

## Installation

### 1. Install NuGet package

Install the [GovUk.Frontend.AspNetCore NuGet package](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore/):

    Install-Package GovUk.Frontend.AspNetCore

Or via the .NET Core command line interface:

    dotnet add package GovUk.Frontend.AspNetCore

### 2. Configure your ASP.NET Core application

Add services to your application:

```cs
using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
//...
builder.Services.AddGovUkFrontend();
```

### 3. Register tag helpers

In your `_ViewImports.cshtml` file:

```razor
@using GovUk.Frontend.AspNetCore
@addTagHelper *, GovUk.Frontend.AspNetCore
```

### 4. Configure your page template

You have several options for configuring your [page template](https://design-system.service.gov.uk/styles/page-template/).

#### Using the `_GovUkPageTemplate` Razor view

A Razor view is provided with the standard page template markup and Razor sections where you can add in your header, footer and any custom markup you require.

In your `_Layout.cshtml` file:

```razor
@{
    Layout = "_GovUkPageTemplate";
}

@section BodyStart {
    <govuk-cookie-banner aria-label="Cookie on [name of service]">
        <govuk-cookie-banner-message>
            <govuk-cookie-banner-message-heading>Cookies on [name of service]</govuk-cookie-banner-message-heading>
            <govuk-cookie-banner-message-content>
                <p class="govuk-body">We use some essential cookies to make this service work.</p>
                <p class="govuk-body">We’d also like to use analytics cookies so we can understand how you use the service and make improvements.</p>
            </govuk-cookie-banner-message-content>
            <govuk-cookie-banner-message-actions>
                <govuk-cookie-banner-message-action text="Accept analytics cookies" type="button"/>
                <govuk-cookie-banner-message-action text="Reject analytics cookies" type="button"/>
                <govuk-cookie-banner-message-action-link text="View cookies" href="#"/>
            </govuk-cookie-banner-message-actions>
        </govuk-cookie-banner-message>
    </govuk-cookie-banner>
}

@section Header {
    <govuk-header home-page-url="https://gov.uk/" />

    <govuk-service-navigation service-name="Service name" service-url="#">
        <govuk-service-navigation-nav>
            <govuk-service-navigation-nav-item href="#">Navigation item 1</govuk-service-navigation-nav-item>
            <govuk-service-navigation-nav-item href="#" active="true">Navigation item 2</govuk-service-navigation-nav-item>
            <govuk-service-navigation-nav-item href="#">Navigation item 3</govuk-service-navigation-nav-item>
        </govuk-service-navigation-nav>
    </govuk-service-navigation>
}

@RenderBody()

@section Footer {
    <govuk-footer>
        <govuk-footer-meta>
            <govuk-footer-meta-items>
                <govuk-footer-meta-item href="#">Item 1</govuk-footer-meta-item>
                <govuk-footer-meta-item href="#">Item 2</govuk-footer-meta-item>
                <govuk-footer-meta-item href="#">Item 3</govuk-footer-meta-item>
            </govuk-footer-meta-items>
        </govuk-footer-meta>
    </govuk-footer>
}
```

The view can be customised by defining the following sections and `ViewData`/`ViewBag` variables.

| Section name  | Description                                                                                                                                                                                                                                               |
|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BeforeContent | Add content that needs to appear outside <main> element. <br /> For example: The [back link](docs/components/back-link.md) component, [breadcrumbs](docs/components/breadcrumbs.md) component, [phase banner](docs/components/phase-banner.md) component. |
| BodyEnd       | Add content just before the closing `</body>` element.                                                                                                                                                                                                    |
| BodyStart     | Add content after the opening `<body>` element. <br/> For example: The cookie banner component.                                                                                                                                                           |
| Footer        | Defines the footer content.                                                                                                                                                                                                                               |
| Head          | Add additional items inside the <head> element. <br /> For example: `<meta name="description" content="My page description">`                                                                                                                             |
| Header        | Defines the header content.                                                                                                                                                                                                                               |
| HeadIcons     | Override the default icons used for GOV.UK branded pages. <br /> For example: `<link rel="shortcut icon" href="favicon.ico" type="image/x-icon" />`                                                                                                       |
| SkipLink      | Override the default [skip link](docs/components/skip-link.md) component.                                                                                                                                                                                 |

| `ViewData` key    | Type     | Description                                                                                                                                                                                     |
|-------------------|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BodyClasses       | `string` | Add class(es) to the `<body>` element.                                                                                                                                                          |
| ContainerClasses  | `string` | Add class(es) to the container. This is useful if you want to make the page wrapper a fixed width.                                                                                              |
| HtmlClasses       | `string` | Add class(es) to the `<html>` element.                                                                                                                                                          |
| HtmlLang          | `string` | Set the language of the whole document. If your `<title>` and `<main>` element are in a different language to the rest of the page, use `HtmlLang` to set the language of the rest of the page. |
| MainClasses       | `string` | Add class(es) to the `<main>` element.                                                                                                                                                          |
| MainLang          | `string` | Set the language of the `<main>` element if it's different to `HtmlLang`.                                                                                                                       |
| OpengraphImageUrl | `string` | Set the URL for the Open Graph image meta tag. The URL must be absolute, including the protocol and domain name.                                                                                |
| Title             | `string` | Override the default page title (`<title>` element).                                                                                                                                            |
| ThemeColor        | `string` | Set the toolbar [colour on some devices](https://developers.google.com/web/updates/2014/11/Support-for-theme-color-in-Chrome-39-for-Android).                                                   |

#### Create your own Razor view

If the standard template above is not sufficient, you can create your own Razor view.

Extension methods are provided on `IHtmlHelper` that simplify the CSS and script imports.
`GovUkFrontendStyleImports` imports CSS stylesheets and should be added to `<head>`.
`GovUkFrontendJsEnabledScript` declares some inline JavaScript that adds the `js-enabled` class to the `<body>` and should be placed at the start of `<body>`.
`GovUkFrontendScriptImports` imports JavaScript files and should be added to the end of `<body>`.

The latter two methods take an optional `cspNonce` parameter; when provided a `nonce` attribute will be added to the inline scripts.

Example `_Layout.cshtml` snippet:
```razor
@using GovUk.Frontend.AspNetCore

<!DOCTYPE html>
<html>
<head>
    @Html.GovUkFrontendStyleImports()
</head>
<body>
    @Html.GovUkFrontendJsEnabledScript()

    @RenderBody()

    @Html.GovUkFrontendScriptImports()
</body>
</html>
```

#### Content security policy (CSP)

There are two built-in mechanisms to help in generating a `script-src` CSP directive that works correctly with the inline scripts used by the page template.

The preferred option is to use the `GetCspScriptHashes` extension method on `IHtmlHelper`. This will return a string that can be inserted directly into the `script-src` directive in your CSP.

Alternatively, a CSP nonce can be appended to the generated `script` tags. A delegate must be configured on `GovUkFrontendOptions` that retrieves a nonce for a given `HttpContext`.
```cs
services.AddGovUkFrontend(options =>
{
    options.GetCspNonceForRequest = context =>
    {
        // Return your nonce here
    };
});
```

See the `Samples.MvcStarter` project for an example of this working.


## GOV.UK Frontend assets

By default, static assets (fonts, images, icons etc.) and the compiled JavaScript and CSS from the GOV.UK Frontend package will be hosted automatically.

To disable hosting of these assets or to only serve a subset of the assets, override `FrontendPackageHostingOptions`:
```cs
services.AddGovUkFrontend(options =>
{
    // Don't host anything
    options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.None;

    // Only host static assets
    options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostAssets;

    // Only host compiled assets (JavaScript and CSS)
    options.FrontendPackageHostingOptions = FrontendPackageHostingOptions.HostCompiledFiles;
});
```

## Components

- [Accordion](docs/components/accordion.md)
- [Back link](docs/components/back-link.md)
- [Breadcrumbs](docs/components/breadcrumbs.md)
- [Button](docs/components/button.md)
- [Checkboxes](docs/components/checkboxes.md)
- [Character count](docs/components/character-count.md)
- [Date input](docs/components/date-input.md)
- [Details](docs/components/details.md)
- [Error message](docs/components/error-message.md)
- [Error summary](docs/components/error-summary.md)
- [Fieldset](docs/components/fieldset.md)
- [File upload](docs/components/file-upload.md)
- [GOV.UK header](docs/components/header.md)
- [GOV.UK footer](docs/components/footer.md)
- [Inset text](docs/components/inset-text.md)
- [Notification banner](docs/components/notification-banner.md)
- [Pagination](docs/components/pagination.md)
- [Panel](docs/components/panel.md)
- [Phase banner](docs/components/phase-banner.md)
- [Radios](docs/components/radios.md)
- [Select](docs/components/select.md)
- [Service navigation](docs/components/service-navigation.md)
- [Skip link](docs/components/skip-link.md)
- [Summary list](docs/components/summary-list.md)
- [Tabs](docs/components/tabs.md)
- [Tag](docs/components/tag.md)
- [Textarea](docs/components/textarea.md)
- [Text input](docs/components/text-input.md)
- [Warning text](docs/components/warning-text.md)

## Validators

- [Max words validator](docs/validation/maxwords.md)


## Building the library

Install [just](https://github.com/casey/just?tab=readme-ov-file#installation) and make sure it's in your `PATH` then run:

```shell
just install-tools
```

From there you can run `just build` to build the library and `just test` to run the tests.
