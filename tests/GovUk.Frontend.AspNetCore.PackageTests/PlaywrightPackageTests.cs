using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// Consuming the Playwright package.
/// </summary>
public class PlaywrightPackageTests(PackageTestContext context)
{
    [Fact]
    public async Task Project_ReferencingPackage_FindsLocatorsOnIPageWithoutAUsing()
    {
        // The fixture uses page.ErrorSummary with no using for its namespace, so it only compiles if the package's
        // buildTransitive props add the global using.
        var project = FixtureProject.CreatePlaywrightTestLibrary(context, nameof(Project_ReferencingPackage_FindsLocatorsOnIPageWithoutAUsing));

        var result = await project.BuildAsync();

        result.AssertSucceeded();
    }
}
