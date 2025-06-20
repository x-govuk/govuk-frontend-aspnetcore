using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore;

internal class GovUkFrontendStartupFilter : IStartupFilter
{
    private readonly IOptions<GovUkFrontendOptions> _optionsAccessor;

    public GovUkFrontendStartupFilter(IOptions<GovUkFrontendOptions> optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(optionsAccessor);
        _optionsAccessor = optionsAccessor;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        ArgumentNullException.ThrowIfNull(next);

        return app =>
        {
            if (_optionsAccessor.Value.CompiledContentPath is PathString compiledContentPath)
            {
                var fileProvider = new ManifestEmbeddedFileProvider(
                    typeof(GovUkFrontendStartupFilter).Assembly,
                    root: "Content/Compiled");

                app.UseMiddleware<RewriteCompiledAssetsMiddleware>(fileProvider);

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = fileProvider,
                    RequestPath = compiledContentPath
                });
            }

            if (_optionsAccessor.Value.StaticAssetsContentPath is PathString assetsContentPath)
            {
                var fileProvider = new ManifestEmbeddedFileProvider(
                    typeof(GovUkFrontendStartupFilter).Assembly,
                    root: "Content/Assets");

                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = fileProvider,
                    RequestPath = assetsContentPath,
                    OnPrepareResponse = ctx =>
                    {
                        var hasVersionQueryParam =
                            ctx.Context.Request.Query[RewriteCompiledAssetsMiddleware.StaticAssetVersionQueryParamName].Count != 0;

                        if (hasVersionQueryParam)
                        {
                            ctx.Context.Response.Headers.CacheControl = "Cache-Control: max-age=31536000, immutable";
                        }
                    }
                });
            }

            next(app);
#pragma warning restore CS0618 // Type or member is obsolete
        };
    }
}
