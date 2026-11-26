using AngleSharp.Html.Parser;

namespace PackageTestFixture;

public static class ErrorSummaryChecks
{
    // Compiles only if the package's global using brings the assertions into scope: there's no using for
    // GovUk.Frontend.AspNetCore.Testing.Xunit in this file, and ImplicitUsings doesn't add it.
    public static void Check()
    {
        var doc = new HtmlParser().ParseDocument("<p>Nothing here</p>");

        Xunit.Assert.NoErrorSummary(doc);
    }
}
