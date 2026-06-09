using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class VersionedAssetMiddleware : IMiddleware
{
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

        var relativeWebRoot = Path.GetRelativePath(environment.ContentRootPath, environment.WebRootPath);

        var buildInfo = optionsAccessor.Value.BuildInfo;

        Debug.Assert(buildInfo?.EnableGovUkFrontendSupport is true);

        if (buildInfo?.GovUkFrontendAssetsDirectory is { } assetDirectory &&
            assetDirectory.StartsWith(relativeWebRoot, StringComparison.Ordinal))
        {
            _staticAssetsDirectory = assetDirectory[relativeWebRoot.Length..];
        }

        if (buildInfo?.GovUkFrontendJavaScriptDirectory is { } jsDirectory &&
            jsDirectory.StartsWith(relativeWebRoot, StringComparison.Ordinal))
        {
            _javascriptPath = jsDirectory[relativeWebRoot.Length..].TrimEnd('/') + "/" + PageTemplateHelper.JavascriptFileName;
        }

        if (buildInfo?.GovUkFrontendStylesheetDirectory is { } cssDirectory &&
            cssDirectory.StartsWith(relativeWebRoot, StringComparison.Ordinal))
        {
            _stylesheetPath = cssDirectory[relativeWebRoot.Length..].TrimEnd('/') + "/" + PageTemplateHelper.StylesheetFileName;
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var expectedVersion = GovUkFrontendInfo.Version;

        if (context.Request.Query[HostCompiledAssetsMiddleware.StaticAssetVersionQueryParamName] == expectedVersion)
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
