namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Options to control the files from the govuk-frontend NPM package to host.
/// </summary>
[Flags]
public enum FrontendPackageHostingOptions
{
    /// <summary>
    /// Don't serve any files.
    /// </summary>
    None = 0,

    /// <summary>
    /// Serve asset files (images, fonts etc.).
    /// </summary>
    /// <remarks>
    /// Assets will be available under the <c>/assets</c> path.
    /// </remarks>
    HostAssets = 1 << 0,

    /// <summary>
    /// Serve the compiled CSS and JavaScript files.
    /// </summary>
    HostCompiledFiles = 1 << 1,

    /// <summary>
    /// Whether source map references should be removed from the compiled CSS and JavaScript files.
    /// </summary>
    RemoveSourceMapReferences = 1 << 2
}
