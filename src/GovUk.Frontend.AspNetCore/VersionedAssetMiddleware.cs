using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class VersionedAssetMiddleware : IMiddleware
{
    internal const string StaticAssetVersionQueryParamName = "v";

    private const string CacheControlHeaderValue = "public, max-age=31536000, immutable";

    private readonly PathString? _staticAssetsDirectory;
    private readonly PathString? _stylesheetPath;
    private readonly PathString? _javascriptPath;

    public VersionedAssetMiddleware(
        IWebHostEnvironment environment,
        IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(optionsAccessor);

        if (!environment.WebRootPath.StartsWith(environment.ContentRootPath, StringComparison.Ordinal))
        {
            return;
        }

        var relativeWebRoot = NormalizeSeparators(Path.GetRelativePath(environment.ContentRootPath, environment.WebRootPath));

        var buildInfo = optionsAccessor.Value.BuildInfo;

        Debug.Assert(buildInfo?.EnableGovUkFrontendSupport is true);

        if (GetPathUnderWebRoot(buildInfo?.GovUkFrontendAssetsDirectory) is { } assetDirectory)
        {
            _staticAssetsDirectory = assetDirectory;
        }

        if (GetPathUnderWebRoot(buildInfo?.GovUkFrontendJavaScriptDirectory) is { } jsDirectory)
        {
            _javascriptPath = jsDirectory + "/" + PageTemplateHelper.JavascriptFileName;
        }

        if (GetPathUnderWebRoot(buildInfo?.GovUkFrontendStylesheetDirectory) is { } cssDirectory)
        {
            _stylesheetPath = cssDirectory + "/" + PageTemplateHelper.StylesheetFileName;
        }

        // The directories come from MSBuild properties so they use whichever separator the project author
        // wrote (the defaults use '\'); PathString only accepts '/'.
        string? GetPathUnderWebRoot(string? directory)
        {
            if (directory is null)
            {
                return null;
            }

            var normalized = NormalizeSeparators(directory);

            return normalized.StartsWith(relativeWebRoot, StringComparison.Ordinal)
                ? normalized[relativeWebRoot.Length..].TrimEnd('/')
                : null;
        }
    }

    private static string NormalizeSeparators(string path) => path.Replace('\\', '/');

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var expectedVersion = GovUkFrontendInfo.Version;

        if (context.Request.Query[StaticAssetVersionQueryParamName] == expectedVersion)
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode != StatusCodes.Status200OK)
                {
                    return Task.CompletedTask;
                }

                var isStaticAssetRequest = _staticAssetsDirectory is { } staticAssetsDirectory &&
                    context.Request.Path.StartsWithSegments(staticAssetsDirectory, StringComparison.OrdinalIgnoreCase);

                var isJavascriptRequest = _javascriptPath is { } javascriptPath &&
                    context.Request.Path.Equals(javascriptPath, StringComparison.OrdinalIgnoreCase);

                var isStylesheetRequest = _stylesheetPath is { } stylesheetPath &&
                    context.Request.Path.Equals(stylesheetPath, StringComparison.OrdinalIgnoreCase);

                if (isStaticAssetRequest || isJavascriptRequest || isStylesheetRequest)
                {
                    context.Response.Headers.CacheControl = CacheControlHeaderValue;
                    context.Response.Headers.Remove("ETag");
                    context.Response.Headers.Remove("Last-Modified");
                }

                return Task.CompletedTask;
            });
        }

        await next(context);
    }
}
