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

    private bool _disposed;

    public PackageTestContext()
    {
        RepoRoot = GetAssemblyMetadata("RepoRoot");
        PackageFeed = GetAssemblyMetadata("PackageTestFeed");

        var packages = Directory.Exists(PackageFeed)
            ? Directory.GetFiles(PackageFeed, "GovUk.Frontend.AspNetCore.*.nupkg")
            : [];

        if (packages.Length != 1)
        {
            throw new InvalidOperationException(
                $"Expected exactly one GovUk.Frontend.AspNetCore package in '{PackageFeed}' but found {packages.Length}. " +
                "Build this project to pack the library under test.");
        }

        PackageVersion = Path.GetFileNameWithoutExtension(packages[0])["GovUk.Frontend.AspNetCore.".Length..];
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
    /// The package under test's extracted copy is removed when it's repacked, so a rebuild can't leave a
    /// stale one behind for a version number that happens to repeat.
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
