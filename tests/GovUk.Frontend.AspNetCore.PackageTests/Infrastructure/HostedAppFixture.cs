namespace GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

/// <summary>
/// Builds a fixture project once for a whole test class, so only the app launches are per test.
/// </summary>
public abstract class HostedAppFixture(PackageTestContext context) : IAsyncLifetime
{
    public PackageTestContext Context { get; } = context;

    public FixtureProject Project { get; private set; } = null!;

    /// <summary>A file that the targets didn't restore, for checking what the middleware leaves alone.</summary>
    public const string UnrelatedStaticFile = "not-govuk.txt";

    protected abstract FixtureProject CreateProject();

    public async ValueTask InitializeAsync()
    {
        Project = CreateProject();

        Directory.CreateDirectory(Project.PathTo("wwwroot"));
        await File.WriteAllTextAsync(Project.PathTo($"wwwroot/{UnrelatedStaticFile}"), "Not part of govuk-frontend.");

        (await Project.BuildAsync()).AssertSucceeded();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<FixtureApp> StartAsync(string targetFramework) => FixtureApp.StartAsync(Project, targetFramework);
}

/// <summary>A project that takes the package's defaults.</summary>
public sealed class DefaultConfigurationAppFixture(PackageTestContext context) : HostedAppFixture(context)
{
    protected override FixtureProject CreateProject() =>
        FixtureProject.CreateWebApp(Context, "HostingDefaults");
}

/// <summary>A project that moves everything the package restores into a subdirectory of the web root.</summary>
public sealed class CustomDirectoryAppFixture(PackageTestContext context) : HostedAppFixture(context)
{
    protected override FixtureProject CreateProject() =>
        FixtureProject.CreateWebApp(
            Context,
            "HostingCustomDirectories",
            new Dictionary<string, string>()
            {
                ["GovUkFrontendAssetsDirectory"] = "wwwroot/govuk/assets",
                ["GovUkFrontendJavaScriptDirectory"] = "wwwroot/govuk",
                ["GovUkFrontendStylesheetDirectory"] = "wwwroot/govuk"
            });
}
