using System.Reflection;
using System.Text.RegularExpressions;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// Everything the tests need to know about the package under test, plus the sandbox root that the
/// generated projects live in.
/// </summary>
/// <remarks>
/// One instance per test run; see <c>AssemblyInfo.cs</c>. The sandbox lives outside the repository so
/// the generated projects don't inherit the repo's <c>Directory.Build.props</c> or <c>nuget.config</c>.
/// </remarks>
public sealed class PackageTestContext : IDisposable
{
    private const string KeepSandboxVariableName = "GOVUK_PACKAGE_TESTS_KEEP_SANDBOX";

    public const string LibraryPackageId = "GovUk.Frontend.AspNetCore";
    public const string TestingPackageId = "GovUk.Frontend.AspNetCore.Testing";
    public const string XunitPackageId = "GovUk.Frontend.AspNetCore.Testing.Xunit.v3";
    public const string PlaywrightPackageId = "GovUk.Frontend.AspNetCore.Testing.Playwright";

    /// <summary>The packages that ship alongside the library, in lockstep with it.</summary>
    public static readonly string[] TestingPackageIds = [TestingPackageId, XunitPackageId, PlaywrightPackageId];

    private bool _disposed;

    public PackageTestContext()
    {
        RepoRoot = GetAssemblyMetadata("RepoRoot");
        PackageFeed = GetAssemblyMetadata("PackageTestFeed");

        PackageVersion = FindPackageVersion(PackageFeed, LibraryPackageId);
        TestingPackageVersions = TestingPackageIds.ToDictionary(id => id, id => FindPackageVersion(PackageFeed, id));
        GovUkFrontendVersion = ReadGovUkFrontendVersion(RepoRoot);
        FixturesDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures");

        // Alongside the feed rather than in the sandbox, so repeat runs don't re-download the package
        // under test's dependencies. It's wiped along with the rest of obj/.
        PackagesDirectory = Path.Combine(Path.GetDirectoryName(PackageFeed.TrimEnd(Path.DirectorySeparatorChar))!, "test-packages");
        Directory.CreateDirectory(PackagesDirectory);

        SandboxDirectory = Path.Combine(Path.GetTempPath(), "govuk-frontend-package-tests", Path.GetRandomFileName());
        Directory.CreateDirectory(SandboxDirectory);
    }

    /// <summary>The root of the repository, used to locate <c>global.json</c>.</summary>
    public string RepoRoot { get; }

    /// <summary>The folder feed containing the freshly packed package under test.</summary>
    public string PackageFeed { get; }

    /// <summary>The version of the freshly packed package under test.</summary>
    public string PackageVersion { get; }

    /// <summary>
    /// The versions of the freshly packed testing packages, by package ID, which should all match
    /// <see cref="PackageVersion"/>.
    /// </summary>
    public IReadOnlyDictionary<string, string> TestingPackageVersions { get; }

    /// <summary>The version of govuk-frontend the package ships, i.e. the value of the <c>v</c> query parameter.</summary>
    public string GovUkFrontendVersion { get; }

    /// <summary>Where the fixture app sources were copied to in this assembly's output.</summary>
    public string FixturesDirectory { get; }

    /// <summary>The temporary root that all generated projects live under.</summary>
    public string SandboxDirectory { get; }

    /// <summary>
    /// A restore packages folder shared by every generated project, so the package under test is only
    /// extracted once and the machine's global cache is never touched.
    /// </summary>
    /// <remarks>
    /// The package under test's extracted copy is removed when it's repacked; every pack of the same commit
    /// has the same version number, so a rebuild would otherwise leave a stale one behind.
    /// </remarks>
    public string PackagesDirectory { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (Environment.GetEnvironmentVariable(KeepSandboxVariableName) is "1" or "true")
        {
            return;
        }

        TryDeleteDirectory(SandboxDirectory);
    }

    /// <summary>Gets the path to the packed <c>.nupkg</c> for <paramref name="packageId"/> in the feed.</summary>
    public string GetPackagePath(string packageId, string version) =>
        Path.Combine(PackageFeed, $"{packageId}.{version}.nupkg");

    private static string FindPackageVersion(string feed, string packageId)
    {
        // A version starts with a digit, which is what tells GovUk.Frontend.AspNetCore.5.3.0.nupkg apart from
        // GovUk.Frontend.AspNetCore.Testing.5.3.0.nupkg (and that from GovUk.Frontend.AspNetCore.Testing.Xunit.v3.5.3.0.nupkg).
        var versions = Directory.Exists(feed)
            ? Directory.GetFiles(feed, $"{packageId}.*.nupkg")
                .Select(path => Path.GetFileNameWithoutExtension(path)[(packageId.Length + 1)..])
                .Where(version => char.IsAsciiDigit(version[0]))
                .ToArray()
            : [];

        if (versions.Length != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one {packageId} package in '{feed}' but found {versions.Length}. " +
                "Build this project to pack the packages under test.");
        }

        return versions[0];
    }

    private static string GetAssemblyMetadata(string key) =>
        typeof(PackageTestContext).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(a => a.Key == key)?.Value ??
        throw new InvalidOperationException($"No '{key}' assembly metadata found.");

    private static string ReadGovUkFrontendVersion(string repoRoot)
    {
        var propsPath = Path.Combine(repoRoot, "Directory.Build.props");
        var match = Regex.Match(File.ReadAllText(propsPath), @"<GovUkFrontendVersion>([^<]+)</GovUkFrontendVersion>");

        return match.Success
            ? match.Groups[1].Value
            : throw new InvalidOperationException($"No GovUkFrontendVersion found in '{propsPath}'.");
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
            // A leaked build server or app process can keep a handle open; leaving a temp folder behind
            // is not worth failing a run over.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
