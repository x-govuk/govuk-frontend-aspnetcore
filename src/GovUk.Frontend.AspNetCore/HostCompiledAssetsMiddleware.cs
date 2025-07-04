using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal partial class HostCompiledAssetsMiddleware
{
    internal const string StaticAssetVersionQueryParamName = "v";

    private readonly RequestDelegate _next;
    private readonly IFileProvider _fileProvider;
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;
    private readonly ConcurrentDictionary<(PathString PathBase, PathString Path), string> _compiledAssetCache;

    public HostCompiledAssetsMiddleware(
        RequestDelegate next,
        IFileProvider fileProvider,
        IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(fileProvider);
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        _next = next;
        _fileProvider = fileProvider;
        _optionsAccessor = optionsAccessor;
        _compiledAssetCache = new();
    }

    [GeneratedRegex(@"url\(/assets/(.*?)\)")]
    private static partial Regex GetCssAssetUrlReferencePattern();

    public async Task InvokeAsync(HttpContext context)
    {
        var version = PageTemplateHelper.GovUkFrontendVersion;

        var cssFileName = "govuk-frontend.min.css";
        var jsFileName = "govuk-frontend.min.js";

        if (context.Request.Path == PageTemplateHelper.DefaultCompiledContentPath + "/" + cssFileName)
        {
            var css = _compiledAssetCache.GetOrAdd(
                (context.Request.PathBase, context.Request.Path),
                _ =>
                {
                    var css = GetFileContents(cssFileName);

                    if (_optionsAccessor.Value.FrontendPackageHostingOptions.HasFlag(FrontendPackageHostingOptions.RemoveSourceMapReferences))
                    {
                        css = css.Replace("/*# sourceMappingURL=govuk-frontend.min.css.map */", "");
                    }

                    if (_optionsAccessor.Value.FrontendPackageHostingOptions.HasFlag(FrontendPackageHostingOptions.HostAssets))
                    {
                        // If we're hosting the static assets, ensure references in the CSS have the correct base URL.
                        // Also append a version query param so we can send a Cache-Control header with a long duration and 'immutable'.
                        css = GetCssAssetUrlReferencePattern().Replace(
                            css,
                            match =>
                            {
                                var relativePath = match.Groups[1].Value;
                                var withVersionQueryParam = QueryHelpers.AddQueryString(relativePath, StaticAssetVersionQueryParamName, version);
                                return $"url({context.Request.PathBase}{PageTemplateHelper.DefaultAssetsPath}/{withVersionQueryParam})";
                            });
                    }

                    return css;
                });

            context.Response.Headers.ContentType = new Microsoft.Extensions.Primitives.StringValues("text/css");

            var hasVersionQueryParam = context.Request.Query[StaticAssetVersionQueryParamName].Count != 0;
            if (hasVersionQueryParam)
            {
                context.Response.Headers.CacheControl = "Cache-Control: public, max-age=31536000, immutable";
            }

            await context.Response.WriteAsync(css);
            return;
        }

        if (context.Request.Path == PageTemplateHelper.DefaultCompiledContentPath + "/" + jsFileName)
        {
            var js = _compiledAssetCache.GetOrAdd(
                (context.Request.PathBase, context.Request.Path),
                _ =>
                {
                    var js = GetFileContents(jsFileName);

                    if (_optionsAccessor.Value.FrontendPackageHostingOptions.HasFlag(FrontendPackageHostingOptions.RemoveSourceMapReferences))
                    {
                        js = js.Replace("//# sourceMappingURL=govuk-frontend.min.js.map", "");
                    }

                    return js;
                });

            context.Response.Headers.ContentType = new Microsoft.Extensions.Primitives.StringValues("text/javascript");

            var hasVersionQueryParam = context.Request.Query[StaticAssetVersionQueryParamName].Count != 0;
            if (hasVersionQueryParam)
            {
                context.Response.Headers.CacheControl = "Cache-Control: max-age=31536000, immutable";
            }

            await context.Response.WriteAsync(js);
            return;
        }

        await _next(context);
    }

    private string GetFileContents(string fileName)
    {
        var fileInfo = _fileProvider.GetFileInfo(fileName);

        using var readStream = fileInfo.CreateReadStream();
        using var streamReader = new StreamReader(readStream);
        return streamReader.ReadToEnd();
    }
}
