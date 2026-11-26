using System.IO.Compression;
using System.Xml.Linq;
using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// The metadata of every package this repo ships, and the lockstep versioning between them.
/// </summary>
public class PackagingTests(PackageTestContext context)
{
    public static TheoryData<string> PackageIds => [PackageTestContext.LibraryPackageId, .. PackageTestContext.TestingPackageIds];

    public static TheoryData<string> TestingPackageIds => [.. PackageTestContext.TestingPackageIds];

    [Theory]
    [MemberData(nameof(TestingPackageIds))]
    public void TestingPackage_HasTheLibrarysVersion(string packageId)
    {
        Assert.Equal(context.PackageVersion, context.TestingPackageVersions[packageId]);
    }

    [Theory]
    [InlineData(PackageTestContext.TestingPackageId, PackageTestContext.LibraryPackageId)]
    [InlineData(PackageTestContext.XunitPackageId, PackageTestContext.TestingPackageId)]
    public void TestingPackage_DependsOnExactlyTheSameVersionOfThisReposPackages(string packageId, string dependencyId)
    {
        // The helpers read the markup that one version of the library renders, so a consumer mustn't be able
        // to resolve a different one without NuGet warning about it.
        var nuspec = ReadNuspec(packageId);

        var dependencies = nuspec.Descendants()
            .Where(e => e.Name.LocalName == "dependency" && e.Attribute("id")?.Value == dependencyId)
            .ToArray();

        Assert.NotEmpty(dependencies);
        Assert.All(dependencies, d => Assert.Equal($"[{context.PackageVersion}]", d.Attribute("version")?.Value));
    }

    [Theory]
    [MemberData(nameof(TestingPackageIds))]
    public void TestingPackage_HasTheLibrarysSharedMetadata(string packageId)
    {
        var library = ReadNuspec(PackageTestContext.LibraryPackageId);
        var testing = ReadNuspec(packageId);

        foreach (var element in new[] { "authors", "copyright", "license", "projectUrl" })
        {
            var libraryValue = GetMetadata(library, element);

            Assert.NotNull(libraryValue);
            Assert.Equal(libraryValue, GetMetadata(testing, element));
        }
    }

    [Fact]
    public void XunitPackage_DependsOnTheAssertionsLibraryOnly()
    {
        // Not the runner: a test project brings its own xunit.v3 package, and the assertions are all that's needed.
        var dependencyIds = ReadNuspec(PackageTestContext.XunitPackageId).Descendants()
            .Where(e => e.Name.LocalName == "dependency")
            .Select(e => e.Attribute("id")!.Value)
            .Distinct()
            .Order(StringComparer.Ordinal);

        Assert.Equal([PackageTestContext.TestingPackageId, "xunit.v3.assert"], dependencyIds);
    }

    [Fact]
    public void XunitPackage_HasBuildTransitiveProps()
    {
        using var package = OpenPackage(PackageTestContext.XunitPackageId);

        Assert.NotNull(package.GetEntry($"buildTransitive/{PackageTestContext.XunitPackageId}.props"));
    }

    [Fact]
    public void PlaywrightPackage_DependsOnPlaywrightOnly()
    {
        // Not the library: browser tests may run against a deployed app, and nothing in the package needs ASP.NET Core.
        var dependencyIds = ReadNuspec(PackageTestContext.PlaywrightPackageId).Descendants()
            .Where(e => e.Name.LocalName == "dependency")
            .Select(e => e.Attribute("id")!.Value)
            .Distinct()
            .Order(StringComparer.Ordinal);

        Assert.Equal(["Microsoft.Playwright"], dependencyIds);
    }

    [Fact]
    public void PlaywrightPackage_HasBuildTransitiveProps()
    {
        using var package = OpenPackage(PackageTestContext.PlaywrightPackageId);

        Assert.NotNull(package.GetEntry($"buildTransitive/{PackageTestContext.PlaywrightPackageId}.props"));
    }

    [Theory]
    [MemberData(nameof(PackageIds))]
    public void Package_HasReadmeAndIcon(string packageId)
    {
        var nuspec = ReadNuspec(packageId);
        using var package = OpenPackage(packageId);

        var readme = GetMetadata(nuspec, "readme");
        var icon = GetMetadata(nuspec, "icon");

        Assert.NotNull(readme);
        Assert.NotNull(icon);
        Assert.NotNull(package.GetEntry(readme));
        Assert.NotNull(package.GetEntry(icon));
    }

    private ZipArchive OpenPackage(string packageId) =>
        ZipFile.OpenRead(context.GetPackagePath(packageId, GetVersion(packageId)));

    private string GetVersion(string packageId) =>
        context.TestingPackageVersions.TryGetValue(packageId, out var version) ? version : context.PackageVersion;

    private XDocument ReadNuspec(string packageId)
    {
        using var package = OpenPackage(packageId);

        var entry = Assert.Single(package.Entries, e => e.FullName.EndsWith(".nuspec", StringComparison.Ordinal));

        using var stream = entry.Open();

        return XDocument.Load(stream);
    }

    private static string? GetMetadata(XDocument nuspec, string name) =>
        nuspec.Descendants().FirstOrDefault(e => e.Name.LocalName == name)?.Value;
}
