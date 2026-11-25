# AGENTS.md

This file provides guidance to coding agents working with code in this repository.

## What this is

`GovUk.Frontend.AspNetCore` is a NuGet library that does two things for ASP.NET Core apps using the GOV.UK Design System: an MSBuild integration copies the `govuk-frontend` assets into the consuming project on build, and tag helpers (`<govuk-*>`) render the components. The targeted `govuk-frontend` version is set by `GovUkFrontendVersion` in `Directory.Build.props`.

## Commands

Tasks are defined in the `justfile` (tool versions in `mise.toml`). The library and tests target `net10.0` (`LibTfms` in `Directory.Build.props`); `global.json` pins the .NET 11 SDK, and the .NET 10 SDK is also needed for its runtime. The package tests build consumer apps for both `net10.0` and `net11.0`.

```sh
just install-tools          # dotnet tool restore (libman, needed by the first build)
just build
just format                 # dotnet format; CI runs `just format --verify-no-changes`
just unit-tests             # tests/GovUk.Frontend.AspNetCore.Tests
just integration-tests      # Playwright browser tests; run `just install-playwright` once first
just package-tests          # packs the library and builds throwaway projects against the .nupkg
just test                   # all three
just pack                   # outputs to ./packages
just docs                   # run the docs app; `just publish-docs` regenerates docs/
just build-samples          # packs, then builds samples/Samples.sln against the package
```

Tests use xUnit v3 on Microsoft.Testing.Platform (see `global.json`), so filters are MTP-style and arguments pass straight through `just`:

```sh
just unit-tests -f net10.0 --filter-class "*ButtonTagHelperTests"
just unit-tests -f net10.0 --filter-method "*DefaultComponentGeneratorTests.Button"
```

Release builds treat warnings as errors and the library uses `AnalysisMode=All`, so build in Release (`just build -c Release`) before considering a change done.

## Architecture

### Rendering: tag helpers → options → component generator

Rendering is split into two layers:

- **`ComponentGeneration/`** is a port of the govuk-frontend Nunjucks templates. Each component has an `*Options` class mirroring the Nunjucks macro parameters and a `DefaultComponentGenerator.<Component>.cs` partial implementing `IComponentGenerator.Generate<Component>Async`, returning a `GovUkComponent` (built from `HtmlTag`). Parameters that don't exist upstream are marked `[NonStandardParameter]`.
- **`TagHelpers/`** translate Razor markup into those options. A component with child elements (e.g. `<govuk-checkboxes>` with `<govuk-checkboxes-item>`, `<hint>`, `<error-message>`) uses a `*Context` object stored in `TagHelperContext.Items` that children populate; the parent tag helper then builds the options and calls the generator, finishing with `component.ApplyToTagHelper(output)`. Form-field tag helpers also bind to `for="..."` model expressions (labels, values and errors come from `ModelHelper`/`ModelMetadata`).

So a markup change belongs in the generator; a Razor API change belongs in the tag helper.

### Encoding (`TemplateString`)

Options use `TemplateString`, which holds either unencoded text or `IHtmlContent`, mirroring Nunjucks `text` vs `html` parameters. Double-encoding is guarded by `BannedSymbols.txt` (RS0030, an error): `AppendHtml(string)` and `HtmlContentExtensions.ToHtmlString` are banned in the library. Use `TemplateString.TryGetText`, `TemplateString.Render`, or append the `IHtmlContent` itself.

### Fixture tests against upstream

The build downloads `govuk-frontend@$(GovUkFrontendVersion)` into `lib/govuk-frontend-<version>/` via libman (target `InstallGovUkFrontendPackage`). The unit test project copies each upstream component's `fixtures.json` into its output, and `DefaultComponentGeneratorTests` runs every fixture through the generator (`[ComponentFixtureData("button", typeof(ButtonOptions), exclude: [...])]`), diffing HTML with AngleSharp. Fixtures that can't apply to the typed API are excluded by name with a comment saying why. Tag helper tests are separate and mostly check that markup produces the right options.

### Other library areas

- `ModelBinding/` — date input model binding (`DateInputModelBinder`, pluggable `DateInputModelConverter`s for `DateOnly`, `DateTime`, tuples, etc.).
- `Validation/` — `[CharacterCount]`/`[MaxWords]` attributes and their client-side adapters.
- `Localization/` — `IGovUkFrontendLocalizer` for built-in component strings.
- `Views/Shared/_GovUkPageTemplate.cshtml` — the page template layout; `PageTemplateHelper` and `HtmlHelperExtensions` supply the style/script imports.
- `GovUkFrontendExtensions` — `AddGovUkFrontend()` / `UseGovUkFrontend()` (the latter includes `VersionedAssetMiddleware`).

### Packaging and the consumer build

`Package/` is the package layout. `Package/buildTransitive/GovUk.Frontend.AspNetCore.targets` is what copies assets into consuming projects (controlled by `EnableGovUkFrontendSupport`, `RestoreGovUkFrontendAssets`, etc.). Its properties are derived in the `.targets` file, not a `.props`, because NuGet imports props before the consumer's project body is evaluated; keep that ordering when editing it. The library's own SCSS (`govuk-frontend.scss`) is compiled by AspNetCore.SassCompiler and packed as `content/govuk-frontend.min.css`. `PackageTests` is the place to test changes to this MSBuild logic. The version comes from git tags via MinVer (`v`-prefixed).

### Docs

`docs/components/*.md` and `docs/images/*` are **generated**. Don't edit them directly: edit the Liquid templates in `src/GovUk.Frontend.AspNetCore.Docs/Templates/components/` (examples in `Pages/Examples/`) and run `just publish-docs`, which runs the docs app, screenshots each example with Puppeteer, and writes the markdown. The tag helper API tables come from the XML doc comments on the tag helpers.

## Conventions

- User-visible changes get an entry under `## Unreleased` in `CHANGELOG.md`.
- `InternalsVisibleTo` covers the test and docs projects, so test helpers can use internals.
- `.cshtml`/`.razor` files are UTF-8 with BOM (`.editorconfig`).
