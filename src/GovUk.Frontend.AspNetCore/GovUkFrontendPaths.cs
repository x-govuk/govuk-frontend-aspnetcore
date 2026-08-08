using Microsoft.AspNetCore.Hosting;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// The URL paths that the files restored by the package's build targets are served from.
/// </summary>
/// <remarks>
/// The targets record the directories they copied into as project-relative paths on
/// <see cref="GovUkFrontendBuildInfoAttribute"/>. Turning those into URLs means stripping the web root,
/// since only what's underneath it gets served, and normalising the separators, since the directories
/// come from MSBuild properties and use whichever separator the project author wrote.
/// <para>
/// A <see langword="null"/> here means the build didn't restore that file, which is not the same as it
/// being at the default location: URL generation falls back to the default, because that's where a
/// project managing the file itself would be expected to put it, but the versioned asset middleware
/// treats it as nothing to match, because a file the build didn't produce can't be assumed to change
/// only when govuk-frontend does.
/// </para>
/// <para>
/// Each non-null value is either empty or rooted, so it can be used as a
/// <see cref="Microsoft.AspNetCore.Http.PathString"/>.
/// </para>
/// </remarks>
internal sealed class GovUkFrontendPaths
{
    /// <summary>Where the library looked before the directories became configurable.</summary>
    private const string DefaultAssets = "/assets";
    private const string DefaultCompiledContent = "";

    private GovUkFrontendPaths(string? assets, string? javaScript, string? stylesheet)
    {
        Assets = assets;
        JavaScript = javaScript;
        Stylesheet = stylesheet;
    }

    /// <summary>What a project gets when the build restored nothing it knows about.</summary>
    public static GovUkFrontendPaths None { get; } = new(null, null, null);

    /// <summary>The directory the assets were restored to, or <see langword="null"/> if they weren't.</summary>
    public string? Assets { get; }

    /// <summary>The directory <c>govuk-frontend.min.js</c> was restored to, or <see langword="null"/>.</summary>
    public string? JavaScript { get; }

    /// <summary>The directory <c>govuk-frontend.min.css</c> was restored to, or <see langword="null"/>.</summary>
    public string? Stylesheet { get; }

    /// <summary>The restored <c>govuk-frontend.min.js</c>, or <see langword="null"/> if it wasn't restored.</summary>
    public string? RestoredJavaScriptFile => JavaScript is null ? null : JavaScript + "/" + PageTemplateHelper.JavascriptFileName;

    /// <summary>The restored <c>govuk-frontend.min.css</c>, or <see langword="null"/> if it wasn't restored.</summary>
    public string? RestoredStylesheetFile => Stylesheet is null ? null : Stylesheet + "/" + PageTemplateHelper.StylesheetFileName;

    /// <summary>The directory to resolve asset URLs against, falling back to the default.</summary>
    public string AssetsUrlPath => Assets ?? DefaultAssets;

    /// <summary>The URL path of <c>govuk-frontend.min.js</c>, falling back to the default location.</summary>
    public string JavaScriptUrlPath => (JavaScript ?? DefaultCompiledContent) + "/" + PageTemplateHelper.JavascriptFileName;

    /// <summary>The URL path of <c>govuk-frontend.min.css</c>, falling back to the default location.</summary>
    public string StylesheetUrlPath => (Stylesheet ?? DefaultCompiledContent) + "/" + PageTemplateHelper.StylesheetFileName;

    /// <summary>The base path to generate script URLs against, falling back to the default location.</summary>
    public string JavaScriptUrlBase => JavaScript ?? DefaultCompiledContent;

    /// <summary>The base path to generate stylesheet URLs against, falling back to the default location.</summary>
    public string StylesheetUrlBase => Stylesheet ?? DefaultCompiledContent;

    public static GovUkFrontendPaths Create(IWebHostEnvironment? environment, GovUkFrontendBuildInfoAttribute? buildInfo)
    {
        // A web root outside the content root can't be expressed relative to it, and nothing the targets
        // copied could be underneath it anyway.
        if (environment is null ||
            buildInfo is null ||
            !environment.WebRootPath.StartsWith(environment.ContentRootPath, StringComparison.Ordinal))
        {
            return None;
        }

        var relativeWebRoot = NormalizeSeparators(Path.GetRelativePath(environment.ContentRootPath, environment.WebRootPath))
            .TrimEnd('/');

        return new GovUkFrontendPaths(
            GetPathUnderWebRoot(buildInfo.GovUkFrontendAssetsDirectory),
            GetPathUnderWebRoot(buildInfo.GovUkFrontendJavaScriptDirectory),
            GetPathUnderWebRoot(buildInfo.GovUkFrontendStylesheetDirectory));

        string? GetPathUnderWebRoot(string? directory)
        {
            if (directory is null)
            {
                return null;
            }

            var normalized = NormalizeSeparators(directory).TrimEnd('/');

            if (!normalized.StartsWith(relativeWebRoot, StringComparison.Ordinal))
            {
                return null;
            }

            var underWebRoot = normalized[relativeWebRoot.Length..];

            // Guards against a sibling directory whose name merely starts with the web root's, which would
            // otherwise produce a path that isn't rooted.
            return underWebRoot.Length == 0 || underWebRoot.StartsWith('/') ? underWebRoot : null;
        }
    }

    private static string NormalizeSeparators(string path) => path.Replace('\\', '/');
}
