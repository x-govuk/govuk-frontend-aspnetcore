# Sample with SASS integration

This sample demonstrates how to set up a project with SASS that can reference the `govuk-frontend` scss source files.

To replicate this setup in your own project, follow these steps:

1. Follow the main [installation guide](../../../README.md#installation).

2. Restore `govuk-frontend` assets on build by adding the following to your `.csproj` file:
   ```xml
   <PropertyGroup>
     <EnableGovUkFrontendSupport>true</EnableGovUkFrontendSupport>
     <GovUkFrontendNpmPackageDirectory>lib\govuk-frontend</GovUkFrontendNpmPackageDirectory>
     <GovUkFrontendSupportPackageDirectory>lib\govuk-frontend-aspnetcore</GovUkFrontendSupportPackageDirectory>
     <GovUkFrontendStylesheetDirectory />
   </PropertyGroup>
   ```

3. Install the `AspNetCore.SassCompiler` package:
   ```shell
   dotnet add package AspNetCore.SassCompiler
   ```

4. Watch SCSS files for changes:
   ```cs
   if (builder.Environment.IsDevelopment())
   {
       builder.Services.AddSassCompiler();
   }

5. Create an `app.scss` file in the `Styles` directory:
   ```scss
   @use "sass:meta";
   @use "govuk-frontend-aspnetcore";
   @use "govuk-frontend" as * with (
     $govuk-font-url-function: meta.get-function("versioned-font-url", $module: "govuk-frontend-aspnetcore"),
     $govuk-image-url-function: meta.get-function("versioned-image-url", $module: "govuk-frontend-aspnetcore")
   );

   /* your custom styles here */
   ```

> [!NOTE]
> If you're not hosting assets at `/assets`, you should override the `$govuk-fonts-path` and `$govuk-images-path` variables e.g.
> ```scss
> @use "govuk-frontend-aspnetcore" as * with (
>   $govuk-fonts-path: "/static/fonts",
>   $govuk-images-path: "/static/images"
> );
> ```

6. Add a `sasscompiler.json` file:
   ```json
   {
     "Source": "Styles/app.scss",
     "Target": "wwwroot/app.css",
     "Arguments": "--style=compressed --quiet-deps",
     "IncludePaths": [
       "lib"
     ]
   }
   ```

7. Create a Razor Layout view in `Pages/Shared/_Layout.cshtml` or `Views/Shared/_Layout.cshtml` that imports the compiled stylesheet:
   ```razor
   @{
     Layout = "_GovUkPageTemplate";
   }

   @section Head {
     <link rel="stylesheet" asp-href-include="~/app.css" asp-append-version="true">
   }

   @RenderBody()
    ```

> [!NOTE]
> Ensure `wwwroot/app.css`, `wwwroot/assets`, `wwwroot/govuk-frontend.min.js` and `lib/govuk-frontend*` are covered in your `.gitignore` file.
