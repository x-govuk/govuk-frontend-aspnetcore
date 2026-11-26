using Microsoft.Playwright;

namespace PackageTestFixture;

public static class ErrorSummaryLocators
{
    // Compiles only if the package's global using brings the extension members into scope: there's no using for
    // GovUk.Frontend.AspNetCore.Testing.Playwright in this file, and ImplicitUsings doesn't add it.
    public static ILocator GetErrors(IPage page) => page.ErrorSummary.Items;
}
