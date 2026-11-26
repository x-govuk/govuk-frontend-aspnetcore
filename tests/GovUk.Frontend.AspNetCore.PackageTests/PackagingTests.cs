using System.IO.Compression;
using System.Xml.Linq;
using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// The metadata of every package this repo ships, and the lockstep versioning between them.
/// </summary>
public class PackagingTests(PackageTestContext context)
{
    public static TheoryData<string> PackageIds => [PackageTestContext.LibraryPackageId, PackageTestContext.TestingPackageId];

    [Fact]
    public void TestingPackage_HasTheLibrarysVersion()
    {
        Assert.Equal(context.PackageVersion, context.TestingPackageVersion);
    }

    [Fact]
    public void TestingPackage_DependsOnExactlyTheLibrarysVersion()
    {
        // The helpers read the markup that one version of the library renders, so a consumer mustn't be able
        // to resolve a different one without NuGet warning about it.
        var nuspec = ReadNuspec(PackageTestContext.TestingPackageId);

        var dependencies = nuspec.Descendants()
            .Where(e => e.Name.LocalName == "dependency" &&
                e.Attribute("id")?.Value == PackageTestContext.LibraryPackageId)
            .ToArray();

        Assert.NotEmpty(dependencies);
        Assert.All(dependencies, d => Assert.Equal($"[{context.PackageVersion}]", d.Attribute("version")?.Value));
    }

    [Theory]
    [InlineData("authors")]
    [InlineData("copyright")]
    [InlineData("license")]
    [InlineData("projectUrl")]
    public void TestingPackage_HasTheLibrarysSharedMetadata(string element)
    {
        var libraryValue = GetMetadata(ReadNuspec(PackageTestContext.LibraryPackageId), element);
        var testingValue = GetMetadata(ReadNuspec(PackageTestContext.TestingPackageId), element);

        Assert.NotNull(libraryValue);
        Assert.Equal(libraryValue, testingValue);
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
        packageId == PackageTestContext.TestingPackageId ? context.TestingPackageVersion : context.PackageVersion;

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
