# Sample with SASS integration

This sample demonstrates how to set up a project with SASS that can reference the `govuk-frontend` scss source files.

To replicate this setup in your own project, follow these steps:

1. Install the `GovUk.Frontend.AspNetCore` package:
   ```shell
   dotnet add package GovUk.Frontend.AspNetCore
   ```

2. Add services to your application:
    ```cs
    builder.Services.AddGovUkFrontend();
    ```

3. Add middleware:
    ```cs
    app.UseGovUkFrontend();
    ```

4. Restore `govuk-frontend` assets on build by adding the following to your `.csproj` file:
   ```xml
   <PropertyGroup>
     <EnableGovUkFrontendSupport>true</EnableGovUkFrontendSupport>
     <GovUkFrontendNpmPackageDirectory>lib\govuk-frontend</GovUkFrontendNpmPackageDirectory>
     <GovUkFrontendStylesheetDirectory />
   </PropertyGroup>
   ```

> [!NOTE]
> Add `wwwroot/assets`, `wwwroot/govuk-frontend.min.js` and `lib/govuk-frontend` to your `.gitignore` file to avoid committing the copied files to your repository.

4. Install the `AspNetCore.SassCompiler` package:
   ```shell
   dotnet add package AspNetCore.SassCompiler
   ```

5. Watch SCSS files for changes:
   ```cs
   #if DEBUG
   builder.Services.AddSassCompiler();
   #endif

6. Add a `sasscompiler.json` file:
   ```json
   {
     "Source": "Styles",
     "Destination": "wwwroot/css",
     "Arguments": "--style=compressed --quiet-deps",
     "IncludePaths": [
       "lib"
     ]
   }
   ```

7. Create your SASS files in the `Styles` directory. From there you can import the govuk-frontend styles. For example, create a `site.scss` file with the following content:
   ```scss
   @use "govuk-frontend/index";

   /* your custom styles here */
   ```

8. Create a Razor Layout view in `Pages/Shared/_Layout.cshtml` or `Views/Shared/_Layout.cshtml` that imports the compiled stylesheet:
   ```razor
   @{
     Layout = "_GovUkPageTemplate";
   }

   @section Head {
     <link rel="stylesheet" asp-href-include="~/css/site.css" asp-append-version="true">
   }

   @RenderBody()
    ```
