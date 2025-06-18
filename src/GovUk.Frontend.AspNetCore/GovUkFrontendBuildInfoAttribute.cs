using System.ComponentModel;

namespace GovUk.Frontend.AspNetCore;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GovUkFrontendBuildInfoAttribute : Attribute
{
    public GovUkFrontendBuildInfoAttribute(bool frontendNpmPackageRestored)
    {
        FrontendNpmPackageRestored = frontendNpmPackageRestored;
    }

    public bool FrontendNpmPackageRestored { get; }
}
