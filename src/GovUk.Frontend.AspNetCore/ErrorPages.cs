using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Renders the error page views and decides which requests get them.
/// </summary>
internal static class ErrorPages
{
    /// <summary>
    /// Whether <paramref name="services"/> can render a view, which needs MVC's view services as well as MVC Core.
    /// </summary>
    public static bool CanRenderViews(IServiceProvider services) =>
        services.GetService<IActionResultExecutor<ViewResult>>() is not null;

    /// <summary>
    /// Whether the request should get an HTML page: it must accept HTML, and mustn't be for an API controller.
    /// </summary>
    /// <remarks>
    /// API clients commonly send <c>*/*</c> and browsers always ask for <c>text/html</c> explicitly when navigating,
    /// so a wildcard is deliberately not enough.
    /// </remarks>
    public static bool WantsHtmlPage(HttpContext context, Endpoint? endpoint)
    {
        if (endpoint?.Metadata.GetMetadata<IApiBehaviorMetadata>() is not null)
        {
            return false;
        }

        RequestHeaders headers;

        try
        {
            headers = context.Request.GetTypedHeaders();
        }
        catch (FormatException)
        {
            return false;
        }

        return headers.Accept.Any(a =>
            a.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase) ||
            a.MediaType.Equals("application/xhtml+xml", StringComparison.OrdinalIgnoreCase));
    }

    public static Task RenderAsync(HttpContext context, string viewName, GovUkErrorPageModel model)
    {
        var metadataProvider = context.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var result = new ViewResult
        {
            ViewName = viewName,
            StatusCode = model.StatusCode,
            ViewData = new ViewDataDictionary<GovUkErrorPageModel>(metadataProvider, new ModelStateDictionary())
            {
                Model = model
            }
        };

        // A fresh RouteData means the view is looked up in the shared locations only, whatever the request matched.
        var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());

        return result.ExecuteResultAsync(actionContext);
    }

    public static async Task HandlePageNotFoundAsync(StatusCodeContext statusCodeContext)
    {
        var context = statusCodeContext.HttpContext;

        if (context.Response.StatusCode != StatusCodes.Status404NotFound || !WantsHtmlPage(context, context.GetEndpoint()))
        {
            return;
        }

        await RenderAsync(
            context,
            GovUkErrorPageModel.ViewNames.PageNotFound,
            new GovUkErrorPageModel { StatusCode = StatusCodes.Status404NotFound });
    }

    public static async Task HandleExceptionAsync(HttpContext context)
    {
        // The exception handler clears the endpoint before calling us; the feature keeps the original.
        var endpoint = context.Features.Get<IExceptionHandlerPathFeature>()?.Endpoint;

        if (!WantsHtmlPage(context, endpoint))
        {
            return;
        }

        await RenderAsync(
            context,
            GovUkErrorPageModel.ViewNames.ProblemWithTheService,
            new GovUkErrorPageModel { StatusCode = StatusCodes.Status500InternalServerError });
    }
}
