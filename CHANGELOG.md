# Changelog

## Unreleased — 5.0.0

`<govuk-date-input>` now requires an error message prefix, either from its `error-message-prefix` attribute or from a `DateInput` attribute on the bound property; rendering one without either throws.

The prefix is what makes the model binder's messages read as guidance — `Your date of birth must be a real date` rather than `Date must be a real date` — and, before, forgetting it was only visible once someone submitted an invalid date. The prefix from the model is now used by the component too, so specifying it in one place is enough:

```csharp
[DateInput(ErrorMessagePrefix = "Your date of birth")]
public DateOnly? DateOfBirth { get; set; }
```

The cookie banner's `<govuk-cookie-banner-message-action>` element is now `<govuk-cookie-banner-message-action-button>`, to match the panel component's `<govuk-panel-action-button>` and to sit alongside `<govuk-cookie-banner-message-action-link>`.
The old element still works, with everything it generated before, but using it produces a deprecation warning with the diagnostic ID `GFA0006`.

Both elements can now generate their `formaction` attribute from the `asp-` attributes, as `<govuk-button>` and `<govuk-panel-action-button>` already could:

```razor
<govuk-cookie-banner-message-action-button text="Accept analytics cookies" type="submit" asp-controller="Cookies" asp-action="Accept" />
```

`EnableGovUkFrontendSupport` is now on by default for projects using the `Microsoft.NET.Web.Sdk` SDK.

`<govuk-pagination>` can generate its own items. Specify `current-page`, `total-pages` and `generate-page-href` instead of writing a child element per page:

```razor
<govuk-pagination current-page="@Model.PageNumber" total-pages="@Model.TotalPages" generate-page-href="@(page => Url.Action("Index", "Home", new { pageNumber = page })!)" />
```

As per the guidance, the first page, the pages either side of the current page and the last page are shown, with an ellipsis wherever pages have been skipped, plus Previous and Next links where there is a page to go to. Nothing is rendered at all when there is only one page. Child elements cannot be combined with these attributes.

`TabsOptions.Title` is now `string?` rather than `TemplateString?`. It only ever comes from the `title` attribute, so it is always text.

`SelectOptionsItem.Text` is `TemplateString?` again and its `Html` property is gone. Content written inside `<govuk-select-item>` is markup, and `TemplateString` already carries either kind, so the extra property added nothing.

`Text` and `Html` options are now typed for what they hold: `Text` is a `string` and is always HTML-encoded, `Html` is an `IHtmlContent` and is always emitted as-is.

Both used to be `TemplateString`, which carries either text or markup and decides which by how it was constructed. That made it possible — and, in practice, easy — to put text in an `Html` slot, and the compiler had nothing to say about it. That is how validation messages came to be rendered as markup. Now `Html = someString` doesn't compile.

The library works out which slot to use in the cases where it decides for you. Content deduced from model metadata or ModelState — a label from a display name, a hint from a description, an error message from a validation message — is text, and now goes in the text slot.

### Breaking changes

- `Text` properties on the component options are `string?`. Assigning a `string` still compiles; assigning a `TemplateString` no longer does.
- `Html` properties are `IHtmlContent?`. A `TemplateString` is an `IHtmlContent`, so passing one still compiles; assigning a bare `string` no longer does. To supply markup from a string, use `TemplateString.FromEncoded(...)`; to supply text, use the `Text` property.
- `HttpContext.AddPageError` takes an `IHtmlContent` rather than a `TemplateString`.
- `SelectOptionsItem` gains an `Html` property, for content written inside `<govuk-select-item>`.
- `DateInputOptionsItem.Label` and `FooterOptionsNavigation.Title` are `IHtmlContent?`, since both can hold content written in a view.

`IHtmlContent.Snapshot()` is a new extension for holding on to content past the tag helper that produced it, carrying over the copy that `TemplateString` used to make internally.

Also fixed: several places emitted a text option as raw markup, because `AppendHtml` has a `string` overload that does no encoding. Calling it is now a build error in this project.

`TemplateString` now keeps track of whether it holds text or HTML through composition, instead of rendering eagerly.

Concatenating, joining or interpolating values used to render them immediately using `HtmlEncoder.Default` and label the result as HTML. That discarded the encoder the application had configured — an app registering `HtmlEncoder.Create(UnicodeRanges.All)` got `ŵ` for a value written directly but `&#x175;` for one that had been concatenated — and meant a value built from plain text was no longer recognisable as text. Composition is now deferred until the value is written, using the encoder it is written with.

Two new members go with it. `TryGetText` gets a value's plain text, for the cases that want an identifier, a key or an attribute token rather than markup; it never HTML-decodes, and reports failure for content with no unambiguous text reading. `IsHtml` reports which kind of content a value holds.

Also fixed:

- Reading an attribute back out of an `AttributeCollection` — through the indexer, `Remove` or enumeration — turned the already-encoded value Razor supplied into a string and treated it as text, so it was encoded again when written. An attribute written in a view as `class="a&amp;b"` came back out as `a&amp;amp;b`.
- `GetHashCode` disagreed with `Equals`, which made `TemplateString` unsafe as a dictionary or set key.
- `TemplateString.Empty` was not equal to `new TemplateString("")` or `TemplateString.FromEncoded("")`.
- `Join` inserted its separator without encoding it.
- The character count's `%{count}` substitution, the date input's field labels and lookups, and the panel's and date input's CSS class checks all operated on the encoded form of a value, which is wrong for any content outside Basic Latin.
- The password input's toggle button text was encoded twice.

Fixes content assigned to an `Html` option as a plain `string` being emitted as markup instead of being HTML-encoded.

This affected validation messages: `ModelError.ErrorMessage` is a `string`, and both `<govuk-error-summary-item for="…">` and `<govuk-error-message for="…">` passed it through an `Html` option. Where a message quotes what the user submitted — as ASP.NET Core's default type-conversion message does, for example when a non-numeric value is posted to an `int` property — the submitted value was written to the page unencoded.

### Breaking changes

A `string` assigned to an `Html` option is now HTML-encoded rather than emitted verbatim, because a `string` is text. To supply markup, wrap it with `TemplateString.FromEncoded(...)`:

```diff
-Html = "<span class=\"govuk-visually-hidden\">Emergency</span> Exit this page"
+Html = TemplateString.FromEncoded("<span class=\"govuk-visually-hidden\">Emergency</span> Exit this page")
```

Content that already came from Razor — anything assigned from a `TagHelperContent` or an `IHtmlContent` — is unaffected and still renders as markup.

The `Text` and `Html` properties on `CharacterCountOptionsBeforeInput`, `CharacterCountOptionsAfterInput`, `FileUploadOptionsBeforeInput`, `FileUploadOptionsAfterInput`, `DateInputOptionsBeforeInputs` and `DateInputOptionsAfterInputs` are now `TemplateString?` rather than `string?`, matching every other component's before- and after-input options.

## 4.4.0

Targets GOV.UK Frontend v6.4.0.

Added support for the interruption variant of the panel component, including the new `govuk-panel-actions`, `govuk-panel-action-button` and `govuk-panel-action-link` tag helpers.
Also adds short tag name support for the panel component.

The checkboxes, date input and radios components can now generate a fieldset without the `<govuk-checkboxes-fieldset>`, `<govuk-date-input-fieldset>` and `<govuk-radios-fieldset>` elements.
A `<govuk-checkboxes-fieldset-legend>`, `<govuk-date-input-fieldset-legend>` or `<govuk-radios-fieldset-legend>` element can now be placed directly inside the root element and the remaining child elements no longer have to be nested inside a fieldset element.
The root elements have also gained `fieldset-*`, `legend-*` and `legend-is-page-heading` attributes for the generated fieldset and legend, along with a `fieldset` attribute for when a fieldset is wanted but none of the other attributes or elements are used;
in that case the legend's content is deduced from the `for` attribute's `ModelMetadata`. The existing fieldset elements continue to work as before.

## 4.3.0

Targets GOV.UK Frontend v6.3.0.

Added tag helpers for the generic header component.

Added `GetJavascriptFileName()` and `GetStylesheetFileName()` methods to `PageTemplateHelper`.

## 4.2.1

Fixes including copied assets in static asset manifest generation.

## 4.2.0

Targets GOV.UK Frontend v6.2.0.

### `govuk-frontend` asset support

#### SASS support

A SASS module has been added that provides functions for creating image and font URLs with a versioned query parameter.
When this query parameter is included in a request URL, middleware will add long-lived cache headers to the response, allowing aggressive caching of assets.
See [the SASS sample](samples/Samples.Sass) for an example of how to use this.

#### MSBuild property changes

> [!NOTE]
> The following changes only apply if you have enabled `RestoreGovUkFrontendNpmPackage` in your project file.

The `RestoreGovUkFrontendNpmPackage`, `GovUkFrontendNpmPackageLocation` and `CopyGovUkFrontendAssetsToWebRoot` MSBuild properties have been deprecated.
In their place is `EnableGovUkFrontendSupport`, `GovUkFrontendNpmPackageDirectory`, `GovUkFrontendAssetsDirectory`, `GovUkFrontendJavaScriptDirectory` and
`GovUkFrontendStylesheetDirectory`, enabling more fine-grained control over assets.

#### Static file support

The `_GovUkPageTemplate.cshtml` layout view now supports [static files](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-10.0).

### Tag helpers

`<govuk-file-upload>` now supports attributes for customising the text used by the JavaScript-enhanced file upload component:
`choose-files-button-text`, `drop-instruction-text`, `entered-drop-zone-text`, `left-drop-zone-text`,
`multiple-files-chosen-text-one`, `multiple-files-chosen-text-other` and `no-file-chosen-text`.

### Fixes

Fixes default navigation ID and menu button text on service navigation components.

Fixes `Cache-Control` header value for hosted stylesheet, javascript and static assets.

Fixes default button content on `<govuk-password-input>`.

## 4.1.1

Adds `AddPageError` extension method to `HttpContext`.

## 4.1.0

Targets GOV.UK Frontend v6.1.0.

### Tag helpers

`<govuk-header>` can now have additional content specified.

The summary list tag helpers now support short tag name syntax.

## 4.0.1

### Fixes

Removes some redundant properties from `HeaderOptions`.

Amends attribute propagation on `<govuk-service-navigation-nav-item>` to copy extra attributes onto generated `<li>` instead of the inner link.
Attributes for the `<a>` element can still be specified by using `link-*` attributes.

## 4.0.0

Targets GOV.UK Frontend v6.0.0.

### `_GovUkPageTemplate` view changes

Additional sections and `ViewData` keys have been added.

### Breaking changes

#### Page template

If you are using the `_GovUkPageTemplate` view, you may need to update your views to use the new section names.
Specifically, if you where using the `Header` section, consider using the `GovUkHeader` and/or `GovUkServiceNavigation` sections instead.
Similarly, if you were using the `Footer` section, consider using the `GovUkFooter` section instead.

See `src/GovUk.Frontend.AspNetCore/Views/_GovUkPageTemplate.cshtml` for the complete view implementation.

#### Tag helpers

The deprecated `asp-for` attribute has been removed from the tag helpers that generate form elements; the `for` attribute should be used instead.

The content licence and copyright sections are no longer included in the footer by default with the `<govuk-footer>` tag helper.
Use empty `<govuk-footer-content-licence>` and `<govuk-footer-content-copyright>` elements to include these sections if needed.

The deprecated `gfa-error-prefix` attribute on `<title>` elements has been removed; the `error-prefix` attribute should be used instead.

The deprecated `is-current` attribute on `<govuk-pagination-item>` has been removed; the `current` attribute should be used instead.

#### `GovUkFrontEndOptions`

The deprecated `StaticAssetsContentPath`, `CompiledContentPath`, `PrependErrorSummary` properties on `GovUkFrontendOptions` have been removed.
The `Rebrand` option has also been removed.

### New features

Adds support for the password tag helper's show/hide customisation.

Infer `type` attribute on `<govuk-input>` from the model metadata when `type` is not specified.

### Fixes

Fix error summary when items are specified with an explicit `href` attribute.

## 3.5.0

Targets GOV.UK Frontend v5.14.0.

## 3.4.3

### Fixes

Fixes default back link content.

Fixes generated error summary to not swallow content.

## 3.4.2

### Fixes

Fix `id` attribute name on `<govuk-notification-banner>`.

Fix mangling non-conformant HTML blocks.

Fix missing whitespace after 'Error:' in error messages.

## 3.4.1

### New features

#### Password input
Tag helpers have been added to create a password input component.

#### `readonly` attribute
A `readonly` attribute has been added to the `<govuk-character-count>`, `<govuk-date-input>`, `<govuk-input>` and `<govuk-textarea>` tag helpers.

### Fixes

Fix `<govuk-date-input-hint>` when used inside a `<govuk-date-input-fieldset>`.

## 3.4.0

Targets GOV.UK Frontend v5.13.0.

### New features

#### `<govuk-date-input>`
The prefix used for generated error messages can now be specified by an `error-message-prefix` attribute on `<govuk-date-input>`
instead of using `[DateInput(ErrorMessagePrefix = "...")]` on the model property.

#### `<govuk-input>`
`<govuk-input-before-input>` and `<govuk-input-after-input>` tag helpers have been added that allow providing content to render before and after the generated
`<input>` element for text input components, respectively.

### Fixes

Fix adding additional classes to `<govuk-pagination>`.

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
