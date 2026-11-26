using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// Consuming the xUnit assertions package.
/// </summary>
public class XunitPackageTests(PackageTestContext context)
{
    [Fact]
    public async Task Project_ReferencingPackage_FindsAssertionsOnAssertWithoutAUsing()
    {
        // The fixture calls Assert.NoErrorSummary(...) with no using for its namespace, so it only compiles if the
        // package's buildTransitive props add the global using.
        var project = FixtureProject.CreateXunitTestLibrary(context, nameof(Project_ReferencingPackage_FindsAssertionsOnAssertWithoutAUsing));

        var result = await project.BuildAsync();

        result.AssertSucceeded();
    }
}
