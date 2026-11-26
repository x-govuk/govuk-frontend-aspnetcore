using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// An <see cref="IActionResult"/> that renders one of the GOV.UK error pages with a status code.
/// </summary>
/// <remarks>
/// <para>
/// This renders the same views as
/// <see cref="GovUkFrontendExtensions.UseGovUkFrontendErrorPages(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>,
/// with the same layout, content partials and localization, so an application can use them for responses the
/// middleware doesn't handle, such as a <c>403</c>.
/// </para>
/// <para>
/// Create one with the <c>GovUkPageNotFound()</c> and <c>GovUkProblemWithTheService()</c> extension methods on
/// <see cref="ControllerBase"/> and <see cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel"/>.
/// </para>
/// </remarks>
public sealed class GovUkErrorPageResult : IActionResult, IStatusCodeActionResult
{
    /// <summary>
    /// Creates a new <see cref="GovUkErrorPageResult"/>.
    /// </summary>
    /// <param name="viewName">The name of the view to render; one of <see cref="GovUkErrorPageModel.ViewNames"/>.</param>
    /// <param name="statusCode">The status code of the response.</param>
    public GovUkErrorPageResult(string viewName, int statusCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(viewName);

        ViewName = viewName;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the name of the view to render.
    /// </summary>
    public string ViewName { get; }

    /// <summary>
    /// Gets the status code of the response.
    /// </summary>
    public int StatusCode { get; }

    int? IStatusCodeActionResult.StatusCode => StatusCode;

    /// <inheritdoc/>
    public Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return ErrorPages.RenderAsync(context.HttpContext, ViewName, new GovUkErrorPageModel { StatusCode = StatusCode });
    }
}
