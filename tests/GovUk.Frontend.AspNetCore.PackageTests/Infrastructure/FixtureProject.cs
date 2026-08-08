using System.Text;

namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// A throwaway project that consumes the packed <c>GovUk.Frontend.AspNetCore</c>, generated into the
/// test run's sandbox and built with the real SDK.
/// </summary>
public sealed class FixtureProject
{
    /// <summary>
    /// The configuration the generated projects build in. The targets under test don't vary by
    /// configuration, so this is fixed rather than following the test project's.
    /// </summary>
    public const string Configuration = "Debug";

    /// <summary>Every fixture is built for all the frameworks the library targets.</summary>
    public static readonly string[] AllTargetFrameworks = ["net8.0", "net9.0", "net10.0"];

    private readonly PackageTestContext _context;

    private FixtureProject(PackageTestContext context, string name, string directory, IReadOnlyList<string> targetFrameworks)
    {
        _context = context;
        Name = name;
        Directory = directory;
        TargetFrameworks = targetFrameworks;
    }

    public string Name { get; }

    /// <summary>The generated project's directory; all the restore assertions are relative to it.</summary>
    public string Directory { get; }

    public IReadOnlyList<string> TargetFrameworks { get; }

    public string ProjectFileName => Name + ".csproj";

    /// <param name="properties">Properties to set in the project body, where a consumer would set them.</param>
    /// <param name="directoryBuildProperties">
    /// Properties to set in a <c>Directory.Build.props</c> instead, which MSBuild evaluates before the
    /// package's own props rather than after.
    /// </param>
    public static FixtureProject CreateWebApp(
        PackageTestContext context,
        string name,
        IReadOnlyDictionary<string, string>? properties = null,
        IReadOnlyDictionary<string, string>? directoryBuildProperties = null,
        IReadOnlyList<string>? targetFrameworks = null) =>
        Create(context, name, "Microsoft.NET.Sdk.Web", "WebApp", properties, directoryBuildProperties, targetFrameworks);

    /// <inheritdoc cref="CreateWebApp" />
    public static FixtureProject CreateClassLibrary(
        PackageTestContext context,
        string name,
        IReadOnlyDictionary<string, string>? properties = null,
        IReadOnlyDictionary<string, string>? directoryBuildProperties = null,
        IReadOnlyList<string>? targetFrameworks = null) =>
        Create(context, name, "Microsoft.NET.Sdk", "ClassLib", properties, directoryBuildProperties, targetFrameworks);

    private static FixtureProject Create(
        PackageTestContext context,
        string name,
        string sdk,
        string fixtureName,
        IReadOnlyDictionary<string, string>? properties,
        IReadOnlyDictionary<string, string>? directoryBuildProperties,
        IReadOnlyList<string>? targetFrameworks)
    {
        targetFrameworks ??= AllTargetFrameworks;

        var directory = Path.Combine(context.SandboxDirectory, "projects", name);
        System.IO.Directory.CreateDirectory(directory);

        var project = new FixtureProject(context, name, directory, targetFrameworks);

        project.WriteSandboxFiles(directoryBuildProperties);
        project.WriteProjectFile(sdk, properties);
        CopyDirectory(Path.Combine(context.FixturesDirectory, fixtureName), directory);

        return project;
    }

    /// <summary>
    /// Builds the project for every target framework in one invocation.
    /// </summary>
    /// <remarks>
    /// Single-process (<c>-m:1</c>) on purpose: the restore targets copy into the project directory, so
    /// every target framework's build writes the same destination files and parallel copies would race.
    /// </remarks>
    public Task<DotnetCliResult> BuildAsync(params string[] extraArguments) =>
        DotnetCli.RunAsync(
            [
                "build",
                ProjectFileName,
                "--configuration", Configuration,
                "-m:1",
                // Lingering build nodes hold handles on the sandbox and stop it being cleaned up.
                "-nodeReuse:false",
                "--nologo",
                "-v:m",
                .. extraArguments
            ],
            Directory);

    public Task<DotnetCliResult> RestoreAsync() =>
        DotnetCli.RunAsync(["restore", ProjectFileName, "--nologo", "-v:m"], Directory);

    /// <summary>
    /// Invokes a single target, for cases where a full build would mask what's being tested.
    /// </summary>
    public Task<DotnetCliResult> RunTargetAsync(string target, params string[] extraArguments) =>
        DotnetCli.RunAsync(
            [
                "msbuild",
                ProjectFileName,
                $"-t:{target}",
                $"-p:Configuration={Configuration}",
                // Force an inner build so the target sees the same state it would during a real build.
                $"-p:TargetFramework={TargetFrameworks[^1]}",
                "-m:1",
                "-nodeReuse:false",
                "-nologo",
                "-v:m",
                .. extraArguments
            ],
            Directory);

    public string PathTo(string relativePath) =>
        Path.Combine(Directory, relativePath.Replace('/', Path.DirectorySeparatorChar));

    public string OutputPathFor(string targetFramework) =>
        Path.Combine(Directory, "bin", Configuration, targetFramework);

    public string AssemblyPathFor(string targetFramework) =>
        Path.Combine(OutputPathFor(targetFramework), Name + ".dll");

    public bool FileExists(string relativePath) => File.Exists(PathTo(relativePath));

    public void AssertFileExists(string relativePath)
    {
        if (!FileExists(relativePath))
        {
            Assert.Fail($"Expected '{relativePath}' to have been restored into the project.{DescribeRestoredFiles()}");
        }
    }

    public void AssertFileDoesNotExist(string relativePath)
    {
        if (FileExists(relativePath))
        {
            Assert.Fail($"Expected '{relativePath}' not to have been restored into the project.{DescribeRestoredFiles()}");
        }
    }

    public void AssertDirectoryIsEmptyOrMissing(string relativePath)
    {
        var path = PathTo(relativePath);

        if (System.IO.Directory.Exists(path) &&
            System.IO.Directory.EnumerateFileSystemEntries(path).Any())
        {
            Assert.Fail($"Expected '{relativePath}' to be empty or missing.{DescribeRestoredFiles()}");
        }
    }

    /// <summary>
    /// The files the targets copied into the project directory, relative to it, excluding build output
    /// and the fixture's own sources.
    /// </summary>
    public IReadOnlyList<string> GetRestoredFiles() =>
        System.IO.Directory.EnumerateFiles(Directory, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(Directory, f).Replace(Path.DirectorySeparatorChar, '/'))
            .Where(f => !f.StartsWith("bin/", StringComparison.Ordinal) && !f.StartsWith("obj/", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal)
            .ToArray();

    private string DescribeRestoredFiles()
    {
        var files = GetRestoredFiles();

        // Asset folders run to hundreds of files; a truncated list is enough to see what happened.
        var shown = files.Take(60).ToArray();
        var suffix = files.Count > shown.Length ? $"{Environment.NewLine}  ... and {files.Count - shown.Length} more" : "";

        return $"{Environment.NewLine}Files in the project directory:{Environment.NewLine}  " +
            string.Join($"{Environment.NewLine}  ", shown) + suffix;
    }

    private static string RenderProperties(IReadOnlyDictionary<string, string>? properties, string indent)
    {
        var rendered = new StringBuilder();

        foreach (var (key, value) in properties ?? new Dictionary<string, string>())
        {
            rendered.AppendLine($"{indent}<{key}>{value}</{key}>");
        }

        return rendered.ToString();
    }

    private void WriteProjectFile(string sdk, IReadOnlyDictionary<string, string>? properties)
    {
        var scenarioProperties = RenderProperties(properties, "    ");

        // The scenario's properties go in the project body, where a consumer would put them; passing them
        // as global properties on the command line evaluates differently.
        File.WriteAllText(
            Path.Combine(Directory, ProjectFileName),
            $"""
            <Project Sdk="{sdk}">

              <PropertyGroup>
                <TargetFrameworks>{string.Join(';', TargetFrameworks)}</TargetFrameworks>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
                <NoDefaultLaunchSettingsFile>true</NoDefaultLaunchSettingsFile>
                <RootNamespace>PackageTestFixture</RootNamespace>
            {scenarioProperties}  </PropertyGroup>

              <ItemGroup>
                <PackageReference Include="GovUk.Frontend.AspNetCore" Version="{_context.PackageVersion}" />
              </ItemGroup>

            </Project>

            """);
    }

    private void WriteSandboxFiles(IReadOnlyDictionary<string, string>? directoryBuildProperties)
    {
        // Stops MSBuild's upward search, so the sandbox can't inherit the repo's build customisation, and
        // keeps restore out of the machine's global packages folder.
        File.WriteAllText(
            Path.Combine(Directory, "Directory.Build.props"),
            $"""
            <Project>
              <PropertyGroup>
                <RestorePackagesPath>{_context.PackagesDirectory}</RestorePackagesPath>
            {RenderProperties(directoryBuildProperties, "    ")}  </PropertyGroup>
            </Project>

            """);

        File.WriteAllText(Path.Combine(Directory, "Directory.Build.targets"), "<Project>\n</Project>\n");

        // Clearing the inherited sources keeps whatever the machine has configured out of the picture,
        // and the source mapping guarantees the package under test comes from the local feed rather than
        // a released version off nuget.org that happens to have the same id.
        File.WriteAllText(
            Path.Combine(Directory, "NuGet.config"),
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <packageSources>
                <clear />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
                <add key="package-under-test" value="{_context.PackageFeed}" />
              </packageSources>
              <packageSourceMapping>
                <packageSource key="nuget.org">
                  <package pattern="*" />
                </packageSource>
                <packageSource key="package-under-test">
                  <package pattern="GovUk.Frontend.AspNetCore" />
                </packageSource>
              </packageSourceMapping>
            </configuration>

            """);

        File.Copy(Path.Combine(_context.RepoRoot, "global.json"), Path.Combine(Directory, "global.json"), overwrite: true);
    }

    private static void CopyDirectory(string source, string destination)
    {
        foreach (var file in System.IO.Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var target = Path.Combine(destination, Path.GetRelativePath(source, file));
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(file, target, overwrite: true);
        }
    }
}
