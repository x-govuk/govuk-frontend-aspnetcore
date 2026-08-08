using Microsoft.AspNetCore.Http;

namespace GovUk.Frontend.AspNetCore;

internal class VersionedAssetMiddleware : IMiddleware
{
    internal const string StaticAssetVersionQueryParamName = "v";

    private const string CacheControlHeaderValue = "public, max-age=31536000, immutable";

    private readonly PathString? _staticAssetsDirectory;
    private readonly PathString? _stylesheetPath;
    private readonly PathString? _javascriptPath;

    // Only the files the build actually restored are marked immutable. A file the project manages itself
    // can sit at the same URL and change without the govuk-frontend version changing, so it must not pick
    // up a year-long cache just because the request happens to carry a matching version.
    public VersionedAssetMiddleware(GovUkFrontendPaths paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        _staticAssetsDirectory = paths.Assets is { } assets ? new PathString(assets) : null;
        _javascriptPath = paths.RestoredJavaScriptFile is { } javaScript ? new PathString(javaScript) : null;
        _stylesheetPath = paths.RestoredStylesheetFile is { } stylesheet ? new PathString(stylesheet) : null;
    }

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
