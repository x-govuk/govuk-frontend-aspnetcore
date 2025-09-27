using System.ComponentModel;

namespace GovUk.Frontend.AspNetCore;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[AttributeUsage(AttributeTargets.Assembly)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class GovUkFrontendBuildInfoAttribute(bool frontendNpmPackageRestored) : Attribute
{
    public bool FrontendNpmPackageRestored { get; } = frontendNpmPackageRestored;
}
