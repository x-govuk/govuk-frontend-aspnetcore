using System.ComponentModel;

namespace GovUk.Frontend.AspNetCore;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GovUkFrontendBuildInfoAttribute(
    bool enableGovUkFrontendSupport,
    string? govUkFrontendNpmPackageDirectory,
    string? govUkFrontendAssetsDirectory,
    string? govUkFrontendJavaScriptDirectory,
    string? govUkFrontendStylesheetDirectory) :
    Attribute
{
    public bool EnableGovUkFrontendSupport { get; } = enableGovUkFrontendSupport;

    public string? GovUkFrontendNpmPackageDirectory { get; } = govUkFrontendNpmPackageDirectory;

    public string? GovUkFrontendAssetsDirectory { get; } = govUkFrontendAssetsDirectory;

    public string? GovUkFrontendJavaScriptDirectory { get; } = govUkFrontendJavaScriptDirectory;

    public string? GovUkFrontendStylesheetDirectory { get; } = govUkFrontendStylesheetDirectory;
}
