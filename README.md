# GOV.UK Frontend for ASP.NET Core

[![GOV.UK Design System version](https://img.shields.io/badge/GOV.UK%20Design%20System-6.5.0-brightgreen)](https://github.com/alphagov/govuk-frontend/releases/tag/v6.5.0)
[![Build](https://github.com/x-govuk/govuk-frontend-aspnetcore/actions/workflows/build.yml/badge.svg)](https://github.com/x-govuk/govuk-frontend-aspnetcore/actions/workflows/build.yml)
[![NuGet Downloads](https://img.shields.io/nuget/dt/GovUk.Frontend.AspNetCore)](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore)

This library handles two things for you when using the [GOV.UK Design System](https://design-system.service.gov.uk/) in an ASP.NET Core application:
getting the GOV.UK Frontend assets into your project, and rendering the components.
The assets are copied in when your project builds, and the components are written as tag helpers that work with your view model.

Below is an example that generates a text input component and a button:
```razor
@* Labels, hints and error messages come from the model, or can be specified in markup *@

<govuk-input for="EmailAddress">
    <label>Email address</label>
    <hint>We'll only use this to send you a receipt</hint>
</govuk-input>

<govuk-button>Submit</govuk-button>
```

An [X-GOVUK](https://x-govuk.org/) project.

## Installation

### 1. Install NuGet package

Install the [GovUk.Frontend.AspNetCore NuGet package](https://www.nuget.org/packages/GovUk.Frontend.AspNetCore/):

    Install-Package GovUk.Frontend.AspNetCore

Or via the .NET Core command line interface:

    dotnet add package GovUk.Frontend.AspNetCore

### 2. Check the `govuk-frontend` assets are copied into your application

Projects using the `Microsoft.NET.Sdk.Web` SDK get the assets copied into their `wwwroot` folder on build without any further configuration.
Any other project type has to opt in by setting `EnableGovUkFrontendSupport` in its project file:
```diff
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
+    <EnableGovUkFrontendSupport>true</EnableGovUkFrontendSupport>
  </PropertyGroup>
</Project>
```
> [!NOTE]
> Add `wwwroot/assets`, `wwwroot/govuk-frontend.min.css` and `wwwroot/govuk-frontend.min.js` to your `.gitignore` file.

### 3. Configure your ASP.NET Core application

Add services and middleware to your application:

```diff
+using GovUk.Frontend.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

+builder.Services.AddGovUkFrontend();

var app = builder.Build();

app.UseRouting();

app.UseAuthorization();

+app.UseGovUkFrontend();

app.MapStaticAssets();  // or app.UseStaticFiles();

app.MapControllers().WithStaticAssets();  // or app.MapRazorPages().WithStaticAssets();
//...
```

### 4. Register tag helpers

In your `_ViewImports.cshtml` file:

```diff
+@using GovUk.Frontend.AspNetCore
+@addTagHelper *, GovUk.Frontend.AspNetCore
```

### 5. Configure your page template

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
        <message>
            <heading>Cookies on [name of service]</heading>
            <content>
                <p class="govuk-body">We use some essential cookies to make this service work.</p>
                <p class="govuk-body">We’d also like to use analytics cookies so we can understand how you use the service and make improvements.</p>
            </content>
            <message-actions>
                <action-button text="Accept analytics cookies" type="button"/>
                <action-button text="Reject analytics cookies" type="button"/>
                <action-link text="View cookies" href="#"/>
            </message-actions>
        </message>
    </govuk-cookie-banner>
}

@section GovUkHeader {
    <govuk-header home-page-url="https://gov.uk/" />
}

@section GovUkServiceNavigation {
    <govuk-service-navigation service-name="Service name" service-url="#">
        <nav>
            <nav-item href="#">Navigation item 1</nav-item>
            <nav-item href="#" active="true">Navigation item 2</nav-item>
            <nav-item href="#">Navigation item 3</nav-item>
        </nav>
    </govuk-service-navigation>
}

@RenderBody()

@section GovUkFooter {
    <govuk-footer>
        <meta>
            <meta-items>
                <meta-item href="#">Item 1</meta-item>
                <meta-item href="#">Item 2</meta-item>
                <meta-item href="#">Item 3</meta-item>
            </meta-items>
        </meta>
        <content-licence />
        <copyright />
    </govuk-footer>
}
```

The view can be customised by defining the following sections and `ViewData`/`ViewBag` entries.

| Section name           | Description                                                                                                                                                                                                                                                     |
|------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BeforeContent          | Add content that needs to appear outside the `<main>` element. <br /> For example: The [back link](docs/components/back-link.md) component, [breadcrumbs](docs/components/breadcrumbs.md) component, [phase banner](docs/components/phase-banner.md) component. |
| BodyEnd                | Add content just before the closing `</body>` element.                                                                                                                                                                                                          |
| BodyStart              | Add content after the opening `<body>` element. <br/> For example: The cookie banner component.                                                                                                                                                                 |
| Container              | Replaces the entire container. The ContainerStart, BeforeContent and ContainerEnd sections will be ignored if this section is defined.                                                                                                                          |
| ContainerStart         | Add content at the start of the container.                                                                                                                                                                                                                      |
| ContainerEnd           | Add content at the end of the container.                                                                                                                                                                                                                        |
| Footer                 | Defines the footer content. The GovUkFooter, FooterStart and FooterEnd sections will be ignored if this section is defined.                                                                                                                                     |
| FooterStart            | Add content at the start of the footer.                                                                                                                                                                                                                         |
| FooterEnd              | Add content at the end of the footer.                                                                                                                                                                                                                           |
| GovUkFooter            | Defines the [GOV.UK footer](docs/components/footer.md).                                                                                                                                                                                                         |
| GovUkHeader            | Defines the [GOV.UK header](docs/components/header.md).                                                                                                                                                                                                         |
| GovUkServiceNavigation | Defines the [service navigation](docs/components/service-navigation.md).                                                                                                                                                                                        |
| Head                   | Add additional items inside the `<head>` element. <br /> For example: `<meta name="description" content="My page description">`                                                                                                                                 |
| Header                 | Defines the header content. The HeaderStart, HeaderEnd and GovUkServiceNavigation sections will be ignored if this section is defined.                                                                                                                          |
| HeaderStart            | Add content at the start of the header.                                                                                                                                                                                                                         |
| HeaderEnd              | Add content at the end of the header.                                                                                                                                                                                                                           |
| HeadIcons              | Override the default icons used for GOV.UK branded pages. <br /> For example: `<link rel="shortcut icon" href="favicon.ico" type="image/x-icon" />`                                                                                                             |
| SkipLink               | Override the default [skip link](docs/components/skip-link.md) component.                                                                                                                                                                                       |

| `ViewData` key      | Type                  | Description                                                                                                                                                                                     |
|---------------------|-----------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AssetPath           | `string`              | Override the default path (`/assets`) for the GOV.UK Frontend assets.                                                                                                                           |
| BodyAttributes      | `AttributeDictionary` | Add attributes to the `<body>` element.                                                                                                                                                         |
| BodyClasses         | `string`              | Add class(es) to the `<body>` element.                                                                                                                                                          |
| ContainerAttributes | `AttributeDictionary` | Add attributes to the container element.                                                                                                                                                        |
| ContainerClasses    | `string`              | Add class(es) to the container. This is useful if you want to make the page wrapper a fixed width.                                                                                              |
| CspNonce            | `string`              | Set the `nonce` attribute for the inline scripts used by the page template.                                                                                                                     |
| FooterAttributes    | `AttributeDictionary` | Add attributes to the `<footer>` element.                                                                                                                                                       |
| FooterClasses       | `string`              | Add class(es) to the `<footer>` element.                                                                                                                                                        |
| HeaderAttributes    | `AttributeDictionary` | Add attributes to the `<header>` element.                                                                                                                                                       |
| HeaderClasses       | `string`              | Add class(es) to the `<header>` element.                                                                                                                                                        |
| HtmlClasses         | `string`              | Add class(es) to the `<html>` element.                                                                                                                                                          |
| HtmlLang            | `string`              | Set the language of the whole document. If your `<title>` and `<main>` element are in a different language to the rest of the page, use `HtmlLang` to set the language of the rest of the page. |
| MainAttributes      | `AttributeDictionary` | Add attributes to the `<main>` element.                                                                                                                                                         |
| MainClasses         | `string`              | Add class(es) to the `<main>` element.                                                                                                                                                          |
| MainLang            | `string`              | Set the language of the `<main>` element if it's different to `HtmlLang`.                                                                                                                       |
| OpengraphImageUrl   | `string`              | Set the URL for the Open Graph image meta tag. The URL must be absolute, including the protocol and domain name.                                                                                |
| ServiceName         | `string`              | Set the service name used in generated [service navigation](docs/components/service-navigation.md).                                                                                             |
| ServiceUrl          | `string`              | Set the service URL used in generated [service navigation](docs/components/service-navigation.md).                                                                                              |
| ThemeColor          | `string`              | Set the toolbar [colour on some devices](https://developers.google.com/web/updates/2014/11/Support-for-theme-color-in-Chrome-39-for-Android).                                                   |
| Title               | `string`              | Override the default page title (`<title>` element).                                                                                                                                            |
| TitleLang           | `string`              | Set the language of the `<title>` element if it's different to `HtmlLang`.                                                                                                                      |

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

Assets are copied into your project when it builds, provided `EnableGovUkFrontendSupport` is `true`.
That is the default for projects using the `Microsoft.NET.Sdk.Web` SDK; every other project type has to set it explicitly.
The table below shows the MSBuild properties you can set to configure which assets are copied into your project and where they are copied to.
Each category of files has a `Restore*` boolean to enable or disable copying it and a `*Directory` to control where the files are copied to.

| MSBuild property                       | Description                                                                | Default                         |
|----------------------------------------|----------------------------------------------------------------------------|---------------------------------|
| `EnableGovUkFrontendSupport`           | Whether to copy any `govuk-frontend` files into your project.              | `true` for web projects         |
| `RestoreGovUkFrontendAssets`           | Whether to copy the static assets (fonts, images, icons etc.).             | `true`                          |
| `GovUkFrontendAssetsDirectory`         | The directory to copy the static assets into.                              | `wwwroot/assets`                |
| `RestoreGovUkFrontendJavascript`       | Whether to copy the `govuk-frontend.min.js` file.                          | `true`                          |
| `GovUkFrontendJavaScriptDirectory`     | The directory to copy the `govuk-frontend.min.js` file into.               | `wwwroot`                       |
| `RestoreGovUkFrontendStylesheet`       | Whether to copy the `govuk-frontend.min.css` file.                         | `true`                          |
| `GovUkFrontendStylesheetDirectory`     | The directory to copy the `govuk-frontend.min.css` file into.              | `wwwroot`                       |
| `RestoreGovUkFrontendNpmPackage`       | Whether to copy the entire `govuk-frontend` NPM package.                   | `false`                         |
| `GovUkFrontendNpmPackageDirectory`     | The directory to copy the `govuk-frontend` NPM package into.               | `lib/govuk-frontend`            |
| `RestoreGovUkFrontendSupportPackage`   | Whether to copy support files.                                             | `false`                         |
| `GovUkFrontendSupportPackageDirectory` | The directory to copy support files into.                                  | `lib/govuk-frontend-aspnetcore` |

`EnableGovUkFrontendSupport` defaults to `true` for projects using the `Microsoft.NET.Sdk.Web` SDK and to `false` for everything else.
The `Restore*` properties only take effect when it is `true`; setting it to `false` stops the build copying anything at all.

Setting `GovUkFrontendSupportPackageDirectory` turns `RestoreGovUkFrontendSupportPackage` on, so there's no need to set both.

If you want the entire `govuk-frontend` NPM package to be available e.g. so you can reference SASS files from your own stylesheet,
set `RestoreGovUkFrontendNpmPackage` to `true` (and optionally override `GovUkFrontendNpmPackageDirectory`).
See [the SASS sample](samples/Samples.Sass) for a full example of how to set up your project with SASS integration.

> [!IMPORTANT]
> If you're hosting your application within an IIS virtual application, you should follow the SASS integration guide in [the SASS sample](samples/Samples.Sass)
> and ensure the `$govuk-fonts-path` and `$govuk-images-path` SASS variables are set appropriately.
> Typically it is sufficient to use `assets/fonts/` and `assets/images/` (i.e. the default location but without the leading `/`).

> [!NOTE]
> The `CopyGovUkFrontendAssetsToWebRoot` property is deprecated; use `RestoreGovUkFrontendAssets` instead.
> Setting it still works, but the build emits a warning.

The library serves nothing itself, so the copied files need to be served by your application — through `MapStaticAssets()` or `UseStaticFiles()`, as in the installation guide above.
When the library generates URLs for files the build didn't copy, it assumes the default locations — `/assets`, `/govuk-frontend.min.css` and `/govuk-frontend.min.js` —
so anything you're managing yourself should be served from there.

`app.UseGovUkFrontend()` adds middleware that applies long-lived cache headers to the files the build copied, which are requested with the `govuk-frontend` version in the query string.
Files the build didn't copy are left alone, since they can change without the `govuk-frontend` version changing.


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
- [Feedback](docs/components/feedback.md)
- [Fieldset](docs/components/fieldset.md)
- [File upload](docs/components/file-upload.md)
- [Generic header](docs/components/generic-header.md)
- [GOV.UK header](docs/components/header.md)
- [GOV.UK footer](docs/components/footer.md)
- [Inset text](docs/components/inset-text.md)
- [Language navigation](docs/components/language-navigation.md)
- [Notification banner](docs/components/notification-banner.md)
- [Pagination](docs/components/pagination.md)
- [Panel](docs/components/panel.md)
- [Password input](docs/components/password-input.md)
- [Phase banner](docs/components/phase-banner.md)
- [Radios](docs/components/radios.md)
- [Select](docs/components/select.md)
- [Service navigation](docs/components/service-navigation.md)
- [Skip link](docs/components/skip-link.md)
- [Summary list](docs/components/summary-list.md)
- [Table](docs/components/table.md)
- [Tabs](docs/components/tabs.md)
- [Tag](docs/components/tag.md)
- [Textarea](docs/components/textarea.md)
- [Text input](docs/components/text-input.md)
- [Warning text](docs/components/warning-text.md)

## Validators

- [Max words validator](docs/validation/maxwords.md)

## Localization

The content the library renders — `Error`, `There is a problem`, `Back`, the date input validation
messages and so on — can be translated by registering an `IGovUkFrontendLocalizer`. See
[Localization](docs/localization.md).


## Contributing

- [Typing component parameters](docs/component-parameters.md) — how to decide whether a new
  `*Options` property should be `string`, `IHtmlContent` or `TemplateString`, and the encoding traps
  to avoid.

## Building the library

Install [just](https://github.com/casey/just?tab=readme-ov-file#installation) and make sure it's in your `PATH` then run:

```shell
just install-tools
```

From there you can run `just build` to build the library and `just test` to run the tests.
