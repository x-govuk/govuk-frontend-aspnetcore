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
     <GovUkFrontendSupportPackageDirectory>lib\govuk-frontend-aspnetcore</GovUkFrontendSupportPackageDirectory>
     <GovUkFrontendStylesheetDirectory />
   </PropertyGroup>
   ```

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
     "Source": "Styles/app.scss",
     "Target": "wwwroot/app.css",
     "Arguments": "--style=compressed --quiet-deps",
     "IncludePaths": [
       "lib"
     ]
   }
   ```

7. Create an `app.scss` file in the `Styles` directory:
   ```scss
   @use "govuk-frontend" as *;

   /* your custom styles here */
   ```

> [!NOTE]
> If you're not hosting assets at `/assets`, you should override the `$govuk-assets-path` variable before e.g.
> ```scss
> @use "govuk-frontend" as * with (
>   $govuk-assets-path: "static/"
> );
> ```

8. Create a Razor Layout view in `Pages/Shared/_Layout.cshtml` or `Views/Shared/_Layout.cshtml` that imports the compiled stylesheet:
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
