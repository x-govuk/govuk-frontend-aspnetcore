# Localization

The library ships with English content for everything it renders — `Error`, `There is a problem`,
`Back`, `Previous`, `Warning`, and so on. To render that content in another language, register an
`IGovUkFrontendLocalizer`.

## Getting started

Add a resource file to your project — say `GovUkFrontendStrings.resx` alongside an empty
`GovUkFrontendStrings` class — with a `.cy.resx` for each language you support, then:

```csharp
builder.Services.AddGovUkFrontend();
builder.Services.AddGovUkFrontendLocalization<GovUkFrontendStrings>();

// ...

app.UseRequestLocalization("en-GB", "cy");
```

Content is looked up by the names on
[`GovUkFrontendResourceNames`](../src/GovUk.Frontend.AspNetCore/Localization/GovUkFrontendResourceNames.cs),
using `CultureInfo.CurrentUICulture` — which `UseRequestLocalization` sets per request. **Resource
files only need the content you're changing**; any name without a resource falls back to the built-in
English, so an English app renders exactly as it did before.

`GovUkFrontendStrings.cy.resx`:

| Name                                | Value                    |
| ----------------------------------- | ------------------------ |
| `ErrorMessage.VisuallyHiddenText`   | `Gwall`                  |
| `ErrorSummary.TitleText`            | `Mae problem wedi codi`  |
| `Title.ErrorPrefix`                 | `Gwall:`                 |
| `BackLink.Text`                     | `Yn ôl`                  |

There are two other overloads: one taking a resource base name and location, and one taking a factory
if you want to supply an `IGovUkFrontendLocalizer` of your own.

```csharp
builder.Services.AddGovUkFrontendLocalization(sp => new MyLocalizer());
```

Either overload may be called before or after `AddGovUkFrontend()`.

## Resource names

Names follow the pattern `{Component}.{Parameter}[.{Variant}]`, where `{Parameter}` is the
govuk-frontend Nunjucks parameter name wherever there is one — so `ErrorMessage.VisuallyHiddenText`,
`Pagination.Previous.Text`, `ServiceNavigation.MenuButtonText`.

Three rules govern the values:

- **Values are HTML-encoded**, except for names ending in `Html`. There is currently one of those:
  `Footer.ContentLicence.Html`, whose default wraps a link, so it's localized as a whole sentence
  rather than as fragments around it.
- **`%{…}` placeholders must be kept verbatim.** `CharacterCount.TextareaDescriptionText.Characters`
  uses `%{count}` and `Pagination.Item.VisuallyHiddenText` uses `%{number}`. Numbers are substituted
  using the invariant culture, matching what the govuk-frontend JavaScript does on the same page.
- **`DateInput.ErrorMessage.*` values must contain exactly one `{0}`**, which is replaced with the
  name of the field in error. A malformed value throws from inside the model binder.

## Client-side content

Some content is rendered by the govuk-frontend JavaScript rather than by the server — the accordion's
show/hide labels, the character count's remaining-characters counter, the file upload's drop zone, and
the password input's announcements. The library passes these through as `data-i18n` attributes.

These names have **no built-in English**: when no content is supplied the attribute is omitted
entirely and the JavaScript uses its own default. Supply content for *every* name in a group,
otherwise you'll get a mix of languages.

`PasswordInput.ShowPasswordText` and `PasswordInput.ShowPasswordAriaLabelText` are used for the
server-rendered button *and* the `data-i18n` attributes, so they always change together.

### Known limitation: plural forms

The govuk-frontend JavaScript selects plural forms with `Intl.PluralRules`, which has up to six
categories. Welsh uses all six; this library currently only models `one` and `other`, matching the
shape of the underlying options. A Welsh service therefore can't fully localize the character count
counter or the file upload's multiple-files text through this route yet.

## Implementing `IGovUkFrontendLocalizer` directly

```csharp
public interface IGovUkFrontendLocalizer
{
    // Returns null to use the library's built-in English content.
    string? GetString(string name);
}
```

Returning `null` for a name you don't recognize is always safe, so new names added in later versions
won't break an existing implementation.

The service is resolved once and consumed by singletons, so implementations must be thread-safe and
must read `CultureInfo.CurrentUICulture` on each call rather than caching it. Registering a *scoped*
implementation fails dependency injection scope validation at startup.

## Overriding content per component

Localization only supplies defaults. Anything set explicitly — a tag helper attribute, or a property
on the options passed to `IComponentGenerator` — still wins. For example `<govuk-title
error-prefix="Problem:">` overrides `Title.ErrorPrefix` for that page.
