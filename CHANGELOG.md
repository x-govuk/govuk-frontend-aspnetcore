# Changelog

## 3.3.0

Targets GOV.UK Frontend v5.12.0.

## 3.2.3

Targets GOV.UK Frontend v5.11.1.

### Fixes

Don't add 'Error: ' to the `<title>` element inside the header `<svg>`.

## 3.2.2

Further improvements for build-time GOV.UK Frontend NPM package restore.

## 3.2.1

### Fixes

Fixes GOV.UK Frontend NPM package restore when using Visual Studio.

## 3.2.0

Targets GOV.UK Frontend v5.11.0.

### Asset hosting changes

An additional call is now required to add the middleware that hosts the govuk-frontend assets.
In your `Program.cs` file, add the following line after `var app = builder.Build();`:
```csharp
app.UseGovUkFrontend();
```

A new mechanism is available to copy assets from the `govuk-frontend` package into your application.
This is particularly useful for applications that are using SASS and want to reference scss files from the `govuk-frontend` package.
See [the SASS sample](samples/Samples.Sass) for an example of how to use this.

### Tag helper changes

#### `<govuk-service-navigation-nav>` tag helper
A `collapse-navigation-on-mobile` attribute has been added to control whether the service navigation is collapsed on mobile devices.

#### `<govuk-service-navigation-nav-item>` tag helper
If not specified, the `current` attribute will be deduced by comparing the `href` attribute to the current request path.

## 3.1.2

### Fixes

Fixes asset path in `_GovUkPageTemplate` when Rebrand is `true`.

## 3.1.1

### Fixes

Fixes rendering tag helpers that have a `<table>` element in their content.

## 3.1.0

Targets GOV.UK Frontend v5.10.2.

### New features

#### Rebrand support
The `_GovUkPageTemplate` view and tag helpers for the GOV.UK header and footer components now support the GOV.UK rebrand.

#### New tag helpers
Tag helpers to create
a [GOV.UK header component](docs/components/header.md),
a [GOV.UK footer component](docs/components/footer.md) and
a [service navigation component](docs/components/service-navigation.md)
have been added.

#### `FrontendPackageHostingOptions`
The `CompiledContentPath` and `StaticAssetsContentPath` properties on `GovUkFrontendOptions` have been deprecated and replaced by `FrontendPackageHostingOptions`.

### Fixes

#### `DefaultFileUploadJavaScriptEnhancements`
Setting the `DefaultFileUploadJavaScriptEnhancements` to `true` now actually does something.

## 3.0.1

### Fixes

#### Date input error items
Fixes getting error item types when `name-prefix` is specified and `for` is not.

## 3.0.0

### Changes to defaults

#### Error summary generation on `<form>`s
Error summaries are no longer prepended to `<form>` elements by default; they are prepended to the `<main>` element instead.
You can restore the old behaviour by setting `ErrorSummaryGeneration` to `PrependToFormElements` on `GovUkFrontendOptions`.

### New features

#### Error summary
- Any errors from partial views or view components will now be included in the generated error summary.
- `<govuk-error-summary>` will populate its items automatically if no `<govuk-error-summary-item>`s are specified.

### Breaking changes

#### `GovUkFrontendAspNetCoreOptions` is renamed to `GovUkFrontendOptions`

#### `asp-for` attributes
The `asp-for` attribute is now obsolete; the `for` attribute should be used in its place.

#### `gfa-` attributes
- The `gfa-prepend-error-summary` attribute on `<form>`s is now named `prepend-error-summary`.
- The `gfa-error-prefix` attribute on `<title>` is now named `error-prefix`.

#### `is-current` on `<govuk-pagination-item>`
`is-current` has been renamed to `current`.

#### Date inputs
- The `TryCreateModelFromErrors` method on `DateInputModelConverter` has been removed; model binding when there are parse errors is no longer supported.
- `GovUk.Frontend.AspNetCore.ModelBinding.DateInputErrorComponents` has been replaced with `GovUk.Frontend.AspNetCore.DateInputItemTypes`.
- The `value` attribute on `<govuk-date-input-day>`, `<govuk-date-input-month>` and `<govuk-date-input-year>` has changed from `int?` to `string`.
- The shape of `DateInputModelConverter` has been changed to support binding partial date inputs.
- Custom `DateInputModelConverter`s are now registered with `RegisterDateInputModelConverter()` on `GovUkFrontendAspNetCoreOptions`.
Only one converter per model type is permitted.

#### `appendVersion`
The `appendVersion` parameter on the `GenerateScriptImports()` and `GenerateStyleImports()` methods on `PageTemplateHelper` has been removed.
Similarly, `appendVersion` parameter on the `GovUkFrontendScriptImports()` and `GovUkFrontendStyleImports()` extension methods on `IHtmlHelper` has been removed.

### Fixes

#### Source map errors
The hosted CSS and JavaScript files no longer have source maps.
Any console errors from browsers failing to download the referenced files should be eliminated.

## 2.9.1

### Fixes

Fix rendering nested form elements.

## 2.9.0

Targets GOV.UK Frontend v5.9.0.

### Tag helper changes

#### `<govuk-file-upload>` tag helper
JavaScript enhancements can be enabled by setting the `javascript-enhancements` attribute to `true`.
This can be configured globally by setting the `DefaultFileUploadJavaScriptEnhancements` property on `GovUkFrontendAspNetCoreOptions`.

A `multiple` attribute has also been added.

## 2.8.1

### Fixes

#### Attribute encoding
Newly-refactored tag helpers now correctly encode their attributes.

## 2.8.0

Targets GOV.UK Frontend v5.8.0.

## 2.7.1

Targets GOV.UK Frontend v5.7.1.

## 2.7.0

Targets GOV.UK Frontend v5.7.0.

## 2.6.0

Targets GOV.UK Frontend v5.6.0.

## 2.5.0

Targets GOV.UK Frontend v5.5.0.

## 2.4.0

Targets GOV.UK Frontend v5.4.1.

### Tag helper changes

#### `<govuk-breadcrumbs>` tag helper
A `label-text` attribute has been added.

## 2.3.0

Targets GOV.UK Frontend v5.3.1 and .NET 8.

### New features

#### `DateInputAttribute`
This attribute can be added to properties that are model bound from date input components. It allows overriding the prefix used for error messages e.g.
```cs
[DateInput(ErrorMessagePrefix = "Your date of birth")]
public DateOnly? DateOfBirth { get; set; }
```

### Tag helper changes

#### `<govuk-input>` tag helper
An `autocapitalize` attribute has been added.
Attributes can be set on the input wrapper element by specifying `input-wrapper-*` attributes.

### Fixes

#### Page template
Fix duplicate `PathBase` in OpengraphImageUrl in page template view.

## 2.2.0

Targets GOV.UK Frontend v5.2.0.

## 2.1.0

#### Page template

The `StaticAssetsContentPath` and `CompiledContentPath` properties on `GovUkFrontendOptions` have been changed from `string` to `PathString?`.

The `GenerateScriptImports`, `GenerateStyleImports` and `GetCspScriptHashes` methods on `PageTemplateHelper` and the corresponding extension methods over `IHtmlHelper`
have had overloads added that take a `PathString pathBase` parameter.

The `_GovUkPageTemplate.cshtml` view has been fixed to respect `HttpRequest.PathBase`.

Middleware has been added to rewrite the URL references in `all.min.css` to respect `HttpRequest.PathBase` and the `StaticAssetsContentPath`.

## 2.0.1

#### Page template

New overloads of `GenerateScriptImports` and `GenerateStyleImports` have been added that accept an `appendVersion` parameter.
This appends a query string with a hash of the file's contents so that content changes following upgrades are seen by end users.

A `GetCspScriptHashes` extension method on `IHtmlHelper` has been added that forwards to the same method on `PageTemplateHelper`.

## 2.0.0

Targets GOV.UK Frontend v5.1.0.

### New features

#### GOV.UK Frontend hosting options

Previously the GOV.UK Frontend library's assets were always hosted at the root of the application.
Many applications generate their own CSS and/or JavaScript bundles and don't need the standard versions at all, though they likely still need the static assets (fonts, images etc.).
There are now two properties on `GovUkFrontendOptions` to control the hosting of the static assets and the compiled assets - `StaticAssetsContentPath` (default `/assets`) and `CompiledContentPath` (default `/govuk`), respectively.
Applications that build and reference their own CSS and JavaScript can set `CompiledContentPath` to `null` to skip hosting the standard compiled assets. Similarly, setting `StaticAssetsContentPath` to `null` will skip hosting the static assets.

#### Page template

`PageTemplateHelper` and the `_GovUkPageTemplate.cshtml` view have been updated to respect the `StaticAssetsContentPath` and `CompiledContentPath` paths set on `GovUkFrontendOptions`.

An additional `ViewData` key can now be passed to `_GovUkPageTemplate.cshtml` - `AssetPath`. When specified, it will be used in place of the `StaticAssetsContentPath` value from `GovUkFrontendOptions` for referencing static asserts.

`GovUkFrontendJsEnabledScript`, `GovUkFrontendScriptImports` and `GovUkFrontendStyleImports` extension methods have been added over `IHtmlHelper` that wrap the
`GenerateJsEnabledScript`, `GovUkFrontendScriptImports` and `GovUkFrontendStyleImports` methods on `PageTemplateHelper`, respectively.

### Tag helper changes

`bool` tag helper properties have been changed to `bool?`.
This is so that it's possible to differentiate between properties that have been explicitly initialized and those that have been left at the default values.
With this, other tag helpers or tag helper initializers can be created that assign default values to these properties.

### Breaking changes

#### `AddImportsToHtml`

This option was used to automatically add style and JavaScript imports to all Razor views.
`PageTemplateHelper` and the `_GovUkPageTemplate.cshtml` layout view are better ways to generate a full page template now so this option, along with the backing tag helper component, have been removed.

### Fixes

#### Page template
The `og:image` `meta` tag in the `_GovUkPageTemplate.cshtml` view is now an absolute URL.
