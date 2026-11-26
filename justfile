default:
  @just --list

# Install local tools
install-tools:
  @dotnet tool restore

# Install Playwright
install-playwright:
  @dotnet build tests/GovUk.Frontend.AspNetCore.IntegrationTests -c Release -v quiet --nologo
  @dotnet pwsh ./tests/GovUk.Frontend.AspNetCore.IntegrationTests/bin/Release/net10.0/playwright.ps1 install chromium

# Restore dependencies
restore *ARGS:
  @dotnet restore {{ARGS}}

# Build the solution
build *ARGS:
  @dotnet build {{ARGS}}

# Build the samples
[working-directory: 'samples']
build-samples *ARGS: (_clear-samples-package-cache) (pack '-c Release')
  @dotnet build {{ARGS}}

[private]
[working-directory: 'samples']
_clear-samples-package-cache:
  @dotnet pwsh -f Remove-CachedPackages.ps1

# Run the tests
test *ARGS: (unit-tests ARGS) (testing-tests ARGS) (testing-playwright-tests ARGS) (integration-tests ARGS) (package-tests ARGS)

# Run the unit tests
unit-tests *ARGS:
  @dotnet test --project tests/GovUk.Frontend.AspNetCore.Tests/GovUk.Frontend.AspNetCore.Tests.csproj {{ARGS}}

# Run the tests for the testing packages
testing-tests *ARGS:
  @dotnet test --project tests/GovUk.Frontend.AspNetCore.Testing.Tests/GovUk.Frontend.AspNetCore.Testing.Tests.csproj {{ARGS}}

# Run the tests for the Playwright testing package; run `just install-playwright` once first
testing-playwright-tests *ARGS:
  @dotnet test --project tests/GovUk.Frontend.AspNetCore.Testing.Playwright.Tests/GovUk.Frontend.AspNetCore.Testing.Playwright.Tests.csproj {{ARGS}}

# Run the integration tests
integration-tests *ARGS:
  @dotnet test --project tests/GovUk.Frontend.AspNetCore.IntegrationTests/GovUk.Frontend.AspNetCore.IntegrationTests.csproj {{ARGS}}

# Run the package tests; these pack the library and build throwaway projects against it
package-tests *ARGS:
  @dotnet test --project tests/GovUk.Frontend.AspNetCore.PackageTests/GovUk.Frontend.AspNetCore.PackageTests.csproj {{ARGS}}

# Format the C# code
format *ARGS:
  @dotnet format {{ARGS}}

# Package the library and the testing packages
pack *ARGS:
  @rm -rf packages
  @dotnet pack src/GovUk.Frontend.AspNetCore/ {{ARGS}}
  @dotnet pack src/GovUk.Frontend.AspNetCore.Testing/ {{ARGS}}
  @dotnet pack src/GovUk.Frontend.AspNetCore.Testing.Xunit.v3/ {{ARGS}}
  @dotnet pack src/GovUk.Frontend.AspNetCore.Testing.Playwright/ {{ARGS}}

# Run the docs app
docs *ARGS:
  @dotnet run --project src/GovUk.Frontend.AspNetCore.Docs/GovUk.Frontend.AspNetCore.Docs.csproj {{ARGS}}

# Publish the docs
publish-docs *ARGS:
  @dotnet run --project src/GovUk.Frontend.AspNetCore.Docs/GovUk.Frontend.AspNetCore.Docs.csproj {{ARGS}} -- publish
