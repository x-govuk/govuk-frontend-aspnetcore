using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

namespace GovUk.Frontend.AspNetCore.PackageTests;

/// <summary>
/// What the package's targets copy into a consuming project, and the build info they generate from it.
/// </summary>
public class RestoreTests(PackageTestContext context)
{
    private const string DeprecationWarning =
        "The CopyGovUkFrontendAssetsToWebRoot property is deprecated; use RestoreGovUkFrontendAssets instead.";

    [Fact]
    public async Task WebSdkProject_WithNoConfiguration_RestoresAssetsJavascriptAndStylesheet()
    {
        var project = FixtureProject.CreateWebApp(context, nameof(WebSdkProject_WithNoConfiguration_RestoresAssetsJavascriptAndStylesheet));

        var result = await project.BuildAsync();

        result.AssertSucceeded();
        result.AssertHasNoWarning(DeprecationWarning);

        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
        project.AssertFileExists("wwwroot/assets/images/govuk-crest.svg");
        project.AssertFileExists("wwwroot/assets/manifest.json");
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk-frontend.min.css");

        // The fonts are a separate subdirectory of assets, so they prove the copy is recursive.
        Assert.NotEmpty(Directory.GetFiles(project.PathTo("wwwroot/assets/fonts"), "*.woff2"));

        // The stylesheet is the one this repo compiles, which appends the version to the asset URLs it
        // references, not the one that ships inside the npm package, which doesn't.
        Assert.Contains(
            $"?v={context.GovUkFrontendVersion}",
            await File.ReadAllTextAsync(project.PathTo("wwwroot/govuk-frontend.min.css")),
            StringComparison.Ordinal);

        // Both of these are opt-in.
        project.AssertDirectoryIsEmptyOrMissing("lib");

        AssertBuildInfoForEveryTargetFramework(
            project,
            assetsDirectory: "wwwroot/assets",
            javaScriptDirectory: "wwwroot",
            stylesheetDirectory: "wwwroot");
    }

    [Fact]
    public async Task NonWebSdkProject_WithNoConfiguration_RestoresNothing()
    {
        var project = FixtureProject.CreateClassLibrary(context, nameof(NonWebSdkProject_WithNoConfiguration_RestoresNothing));

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        // EnableGovUkFrontendSupport only defaults to true for Web SDK projects.
        project.AssertDirectoryIsEmptyOrMissing("wwwroot");
        project.AssertDirectoryIsEmptyOrMissing("lib");

        AssertNoBuildInfoForAnyTargetFramework(project);
    }

    [Fact]
    public async Task NonWebSdkProject_WithSupportEnabled_RestoresAssetsJavascriptAndStylesheet()
    {
        var project = FixtureProject.CreateClassLibrary(
            context,
            nameof(NonWebSdkProject_WithSupportEnabled_RestoresAssetsJavascriptAndStylesheet),
            new Dictionary<string, string>() { ["EnableGovUkFrontendSupport"] = "true" });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk-frontend.min.css");
    }

    [Fact]
    public async Task WithAllRestoreOptionsEnabled_RestoresNpmPackageAndSupportFiles()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithAllRestoreOptionsEnabled_RestoresNpmPackageAndSupportFiles),
            new Dictionary<string, string>()
            {
                ["RestoreGovUkFrontendAssets"] = "true",
                ["RestoreGovUkFrontendJavascript"] = "true",
                ["RestoreGovUkFrontendStylesheet"] = "true",
                ["RestoreGovUkFrontendNpmPackage"] = "true",
                ["RestoreGovUkFrontendSupportPackage"] = "true"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk-frontend.min.css");

        // The whole npm package, not just the built bundles.
        project.AssertFileExists("lib/govuk-frontend/govuk-frontend.min.js");
        project.AssertFileExists("lib/govuk-frontend/assets/images/favicon.ico");
        project.AssertFileExists("lib/govuk-frontend/index.scss");

        // The Sass support module, which is generated at pack time rather than coming from npm.
        project.AssertFileExists("lib/govuk-frontend-aspnetcore/index.scss");
        project.AssertFileExists("lib/govuk-frontend-aspnetcore/_constants.scss");

        Assert.Contains(
            context.GovUkFrontendVersion,
            await File.ReadAllTextAsync(project.PathTo("lib/govuk-frontend-aspnetcore/_constants.scss")));
    }

    [Fact]
    public async Task WithCustomDirectories_RestoresToThoseDirectories()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithCustomDirectories_RestoresToThoseDirectories),
            new Dictionary<string, string>()
            {
                ["GovUkFrontendAssetsDirectory"] = "wwwroot/govuk/assets",
                ["GovUkFrontendJavaScriptDirectory"] = "wwwroot/govuk",
                ["GovUkFrontendStylesheetDirectory"] = "wwwroot/govuk",
                ["RestoreGovUkFrontendNpmPackage"] = "true",
                ["GovUkFrontendNpmPackageDirectory"] = "vendor/npm",
                ["RestoreGovUkFrontendSupportPackage"] = "true",
                ["GovUkFrontendSupportPackageDirectory"] = "vendor/support"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertFileExists("wwwroot/govuk/assets/images/favicon.ico");
        project.AssertFileExists("wwwroot/govuk/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk/govuk-frontend.min.css");
        project.AssertFileExists("vendor/npm/govuk-frontend.min.js");
        project.AssertFileExists("vendor/support/index.scss");

        project.AssertFileDoesNotExist("wwwroot/govuk-frontend.min.js");
        project.AssertFileDoesNotExist("wwwroot/govuk-frontend.min.css");
        project.AssertDirectoryIsEmptyOrMissing("wwwroot/assets");
        project.AssertDirectoryIsEmptyOrMissing("lib");

        AssertBuildInfoForEveryTargetFramework(
            project,
            assetsDirectory: "wwwroot/govuk/assets",
            javaScriptDirectory: "wwwroot/govuk",
            stylesheetDirectory: "wwwroot/govuk");
    }

    /// <summary>
    /// A directory written the way a Windows author would write it has to survive being embedded in the
    /// generated attribute, where an unescaped backslash is an escape sequence rather than a separator.
    /// </summary>
    /// <remarks>
    /// MSBuild is not consistent about whether it normalises the separators before they reach the
    /// generated source — on Unix the same build emits them verbatim for one target framework and
    /// normalised for the next — so the value is compared with the separators normalised, the way
    /// <c>VersionedAssetMiddleware</c> compares it. What has to hold either way is that a backslash which
    /// does survive is escaped, since an unescaped one is a C# escape sequence rather than a separator.
    /// </remarks>
    [Fact]
    public async Task WithWindowsStyleDirectorySeparators_RestoresToThoseDirectoriesAndEscapesTheGeneratedBuildInfo()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithWindowsStyleDirectorySeparators_RestoresToThoseDirectoriesAndEscapesTheGeneratedBuildInfo),
            new Dictionary<string, string>()
            {
                ["GovUkFrontendAssetsDirectory"] = @"wwwroot\govuk\assets",
                ["GovUkFrontendJavaScriptDirectory"] = @"wwwroot\govuk",
                ["GovUkFrontendStylesheetDirectory"] = @"wwwroot\govuk"
            });

        var result = await project.BuildAsync();

        // A literal parameter would have produced source that doesn't compile, or a string containing a
        // \g escape.
        result.AssertSucceeded();

        project.AssertFileExists("wwwroot/govuk/assets/images/favicon.ico");
        project.AssertFileExists("wwwroot/govuk/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk/govuk-frontend.min.css");

        foreach (var targetFramework in project.TargetFrameworks)
        {
            var buildInfo = GeneratedBuildInfo.Read(project, targetFramework);

            Assert.Equal("wwwroot/govuk/assets", Normalize(buildInfo.AssetsDirectory));
            Assert.Equal("wwwroot/govuk", Normalize(buildInfo.JavaScriptDirectory));
            Assert.Equal("wwwroot/govuk", Normalize(buildInfo.StylesheetDirectory));

            if (buildInfo.AssetsDirectory!.Contains('\\', StringComparison.Ordinal))
            {
                Assert.Contains(@"""wwwroot\\govuk\\assets""", buildInfo.RawArguments, StringComparison.Ordinal);
            }
        }

        static string? Normalize(string? directory) => directory?.Replace('\\', '/');
    }

    [Fact]
    public async Task WithAllRestoreOptionsDisabled_RestoresNothingAndReportsNoDirectories()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithAllRestoreOptionsDisabled_RestoresNothingAndReportsNoDirectories),
            new Dictionary<string, string>()
            {
                ["RestoreGovUkFrontendAssets"] = "false",
                ["RestoreGovUkFrontendJavascript"] = "false",
                ["RestoreGovUkFrontendStylesheet"] = "false",
                ["RestoreGovUkFrontendNpmPackage"] = "false",
                ["RestoreGovUkFrontendSupportPackage"] = "false"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertDirectoryIsEmptyOrMissing("wwwroot");
        project.AssertDirectoryIsEmptyOrMissing("lib");

        // Support is still on, so the middleware is still added; it just has nothing to mark as immutable.
        AssertBuildInfoForEveryTargetFramework(
            project,
            assetsDirectory: null,
            javaScriptDirectory: null,
            stylesheetDirectory: null);
    }

    [Fact]
    public async Task WithSupportDisabled_RestoresNothingEvenWhenTheRestoreOptionsAreEnabled()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithSupportDisabled_RestoresNothingEvenWhenTheRestoreOptionsAreEnabled),
            new Dictionary<string, string>()
            {
                ["EnableGovUkFrontendSupport"] = "false",
                ["RestoreGovUkFrontendAssets"] = "true",
                ["RestoreGovUkFrontendJavascript"] = "true",
                ["RestoreGovUkFrontendStylesheet"] = "true",
                ["RestoreGovUkFrontendNpmPackage"] = "true",
                ["RestoreGovUkFrontendSupportPackage"] = "true"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertDirectoryIsEmptyOrMissing("wwwroot");
        project.AssertDirectoryIsEmptyOrMissing("lib");

        AssertNoBuildInfoForAnyTargetFramework(project);
    }

    [Fact]
    public async Task WithDeprecatedCopyAssetsPropertySetToTrue_RestoresAssetsAndWarns()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithDeprecatedCopyAssetsPropertySetToTrue_RestoresAssetsAndWarns),
            new Dictionary<string, string>() { ["CopyGovUkFrontendAssetsToWebRoot"] = "true" });

        var result = await project.BuildAsync();

        result.AssertSucceeded();
        result.AssertHasWarning(DeprecationWarning);

        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
    }

    /// <summary>
    /// The project body is where a consumer naturally sets a property, so the backward-compatibility
    /// shim has to work from there and not just from a <c>Directory.Build.props</c>.
    /// </summary>
    [Fact]
    public async Task WithDeprecatedCopyAssetsPropertySetToFalseInProjectBody_DoesNotRestoreAssets()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithDeprecatedCopyAssetsPropertySetToFalseInProjectBody_DoesNotRestoreAssets),
            new Dictionary<string, string>() { ["CopyGovUkFrontendAssetsToWebRoot"] = "false" });

        var result = await project.BuildAsync();

        result.AssertSucceeded();
        result.AssertHasWarning(DeprecationWarning);

        project.AssertDirectoryIsEmptyOrMissing("wwwroot/assets");

        // The old property only ever controlled the assets.
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk-frontend.min.css");
    }

    /// <summary>
    /// The new property wins when both are set, rather than the shim overriding it.
    /// </summary>
    [Fact]
    public async Task WithDeprecatedCopyAssetsPropertyContradictingTheNewOne_TheNewOneWins()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithDeprecatedCopyAssetsPropertyContradictingTheNewOne_TheNewOneWins),
            new Dictionary<string, string>()
            {
                ["CopyGovUkFrontendAssetsToWebRoot"] = "false",
                ["RestoreGovUkFrontendAssets"] = "true"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();
        result.AssertHasWarning(DeprecationWarning);

        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
    }

    [Fact]
    public async Task WithDeprecatedCopyAssetsPropertySetToFalseInDirectoryBuildProps_DoesNotRestoreAssets()
    {
        var project = FixtureProject.CreateWebApp(
            context,
            nameof(WithDeprecatedCopyAssetsPropertySetToFalseInDirectoryBuildProps_DoesNotRestoreAssets),
            directoryBuildProperties: new Dictionary<string, string>() { ["CopyGovUkFrontendAssetsToWebRoot"] = "false" });

        var result = await project.BuildAsync();

        result.AssertSucceeded();
        result.AssertHasWarning(DeprecationWarning);

        project.AssertDirectoryIsEmptyOrMissing("wwwroot/assets");

        // The old property only ever controlled the assets.
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/govuk-frontend.min.css");
    }

    /// <summary>
    /// The other two derivations: setting a support package directory implies
    /// <c>RestoreGovUkFrontendSupportPackage</c>, and either opt-in implies
    /// <c>EnableGovUkFrontendSupport</c>. Both have to work from the project body.
    /// </summary>
    [Fact]
    public async Task WithDerivedPropertiesSetInProjectBody_TheDerivationHappens()
    {
        var project = FixtureProject.CreateClassLibrary(
            context,
            nameof(WithDerivedPropertiesSetInProjectBody_TheDerivationHappens),
            new Dictionary<string, string>()
            {
                // Implies EnableGovUkFrontendSupport.
                ["RestoreGovUkFrontendNpmPackage"] = "true",
                // Implies RestoreGovUkFrontendSupportPackage.
                ["GovUkFrontendSupportPackageDirectory"] = "vendor/support"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertFileExists("lib/govuk-frontend/govuk-frontend.min.js");
        project.AssertFileExists("vendor/support/index.scss");

        AssertBuildInfoForEveryTargetFramework(
            project,
            assetsDirectory: "wwwroot/assets",
            javaScriptDirectory: "wwwroot",
            stylesheetDirectory: "wwwroot");
    }

    [Fact]
    public async Task WithDerivedPropertiesSetInDirectoryBuildProps_TheDerivationHappens()
    {
        var project = FixtureProject.CreateClassLibrary(
            context,
            nameof(WithDerivedPropertiesSetInDirectoryBuildProps_TheDerivationHappens),
            directoryBuildProperties: new Dictionary<string, string>()
            {
                ["RestoreGovUkFrontendNpmPackage"] = "true",
                ["GovUkFrontendSupportPackageDirectory"] = "vendor/support"
            });

        var result = await project.BuildAsync();

        result.AssertSucceeded();

        project.AssertFileExists("lib/govuk-frontend/govuk-frontend.min.js");
        project.AssertFileExists("vendor/support/index.scss");
    }

    /// <summary>
    /// The IDE runs design-time builds on project load and after every edit; copying files during those
    /// means the IDE churns the project directory in the background.
    /// </summary>
    [Fact]
    public async Task DesignTimeBuild_DoesNotCopyAnyFiles()
    {
        var project = FixtureProject.CreateWebApp(context, nameof(DesignTimeBuild_DoesNotCopyAnyFiles));

        (await project.RestoreAsync()).AssertSucceeded();

        (await project.RunTargetAsync("RestoreGovUkFrontend", "-p:DesignTimeBuild=true")).AssertSucceeded();

        project.AssertDirectoryIsEmptyOrMissing("wwwroot");

        // Running the same target without the flag proves the target was reachable in the first place,
        // so the assertion above can't pass just because nothing ran.
        (await project.RunTargetAsync("RestoreGovUkFrontend")).AssertSucceeded();

        project.AssertFileExists("wwwroot/govuk-frontend.min.css");
    }

    [Fact]
    public async Task Rebuild_AfterRestoredFilesAreDeleted_RestoresThemAgain()
    {
        var project = FixtureProject.CreateWebApp(context, nameof(Rebuild_AfterRestoredFilesAreDeleted_RestoresThemAgain));

        (await project.BuildAsync()).AssertSucceeded();

        File.Delete(project.PathTo("wwwroot/govuk-frontend.min.css"));
        File.Delete(project.PathTo("wwwroot/govuk-frontend.min.js"));
        File.Delete(project.PathTo("wwwroot/assets/images/favicon.ico"));

        (await project.BuildAsync()).AssertSucceeded();

        project.AssertFileExists("wwwroot/govuk-frontend.min.css");
        project.AssertFileExists("wwwroot/govuk-frontend.min.js");
        project.AssertFileExists("wwwroot/assets/images/favicon.ico");
    }

    private static void AssertBuildInfoForEveryTargetFramework(
        FixtureProject project,
        string? assetsDirectory,
        string? javaScriptDirectory,
        string? stylesheetDirectory)
    {
        foreach (var targetFramework in project.TargetFrameworks)
        {
            var buildInfo = GeneratedBuildInfo.Read(project, targetFramework);

            Assert.Equal(assetsDirectory, buildInfo.AssetsDirectory);
            Assert.Equal(javaScriptDirectory, buildInfo.JavaScriptDirectory);
            Assert.Equal(stylesheetDirectory, buildInfo.StylesheetDirectory);
        }
    }

    /// <summary>
    /// With support disabled there is nothing for the library to act on, so the targets emit no attribute
    /// at all rather than one saying so.
    /// </summary>
    private static void AssertNoBuildInfoForAnyTargetFramework(FixtureProject project)
    {
        foreach (var targetFramework in project.TargetFrameworks)
        {
            Assert.Null(GeneratedBuildInfo.ReadOrDefault(project, targetFramework));
        }
    }
}
