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

3. Enable `govuk-frontend` package restore on build by adding the following to your `.csproj` file:
   ```xml
   <PropertyGroup>
     <RestoreGovUkFrontendNpmPackage>true</RestoreGovUkFrontendNpmPackage>
   </PropertyGroup>
   ```
This will copy the contents of the `govuk-frontend` NPM package into your project.

> [!WARNING]
> When you enable `RestoreGovUkFrontendNpmPackage`, the automatic hosting of `govuk-frontend` files is disabled.
> By default, the static assets will be copied to `wwwroot/assets`.
> You must ensure that the required CSS and JavaScript is available.

> [!NOTE]
> Add `wwwroot/assets` and `govuk-frontend` to your `.gitignore` file to avoid committing the copied files to your repository.

4. Install the `DartSassBuilder` package:
   ```shell
   dotnet add package DartSassBuilder
   ```

5. Configure SASS compilation by adding the following to your `.csproj` file:
   ```xml
   <PropertyGroup>
     <EnableDefaultSassItems>false</EnableDefaultSassItems>
   </PropertyGroup>

   <ItemGroup>
     <SassFile Include="wwwroot/*.scss" Exclude="govuk-frontend/**/*.css" />
   </ItemGroup>
   ```

6. Create your SASS files in the `wwwroot` directory. From there you can import the govuk-frontend styles. For example, create a file named `main.scss` in the `wwwroot` directory with the following content:
   ```scss
   @import "govuk-frontend/govuk/all";

   /* your custom styles here */
   ```
