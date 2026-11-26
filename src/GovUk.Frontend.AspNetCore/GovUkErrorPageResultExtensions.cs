using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Extension methods for returning the GOV.UK error pages from MVC controllers and Razor Pages.
/// </summary>
public static class GovUkErrorPageResultExtensions
{
    /// <summary>
    /// Creates a <see cref="GovUkErrorPageResult"/> that renders the &quot;Page not found&quot; page.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="statusCode">The status code of the response. The default is <c>404</c>.</param>
    /// <returns>The created <see cref="GovUkErrorPageResult"/>.</returns>
    public static GovUkErrorPageResult GovUkPageNotFound(
        this ControllerBase controller,
        int statusCode = StatusCodes.Status404NotFound)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return new GovUkErrorPageResult(GovUkErrorPageModel.ViewNames.PageNotFound, statusCode);
    }

    /// <summary>
    /// Creates a <see cref="GovUkErrorPageResult"/> that renders the &quot;Sorry, there is a problem with the
    /// service&quot; page.
    /// </summary>
    /// <param name="controller">The controller.</param>
    /// <param name="statusCode">The status code of the response. The default is <c>500</c>.</param>
    /// <returns>The created <see cref="GovUkErrorPageResult"/>.</returns>
    public static GovUkErrorPageResult GovUkProblemWithTheService(
        this ControllerBase controller,
        int statusCode = StatusCodes.Status500InternalServerError)
    {
        ArgumentNullException.ThrowIfNull(controller);
        return new GovUkErrorPageResult(GovUkErrorPageModel.ViewNames.ProblemWithTheService, statusCode);
    }

    /// <summary>
    /// Creates a <see cref="GovUkErrorPageResult"/> that renders the &quot;Page not found&quot; page.
    /// </summary>
    /// <param name="pageModel">The page model.</param>
    /// <param name="statusCode">The status code of the response. The default is <c>404</c>.</param>
    /// <returns>The created <see cref="GovUkErrorPageResult"/>.</returns>
    public static GovUkErrorPageResult GovUkPageNotFound(
        this PageModel pageModel,
        int statusCode = StatusCodes.Status404NotFound)
    {
        ArgumentNullException.ThrowIfNull(pageModel);
        return new GovUkErrorPageResult(GovUkErrorPageModel.ViewNames.PageNotFound, statusCode);
    }

    /// <summary>
    /// Creates a <see cref="GovUkErrorPageResult"/> that renders the &quot;Sorry, there is a problem with the
    /// service&quot; page.
    /// </summary>
    /// <param name="pageModel">The page model.</param>
    /// <param name="statusCode">The status code of the response. The default is <c>500</c>.</param>
    /// <returns>The created <see cref="GovUkErrorPageResult"/>.</returns>
    public static GovUkErrorPageResult GovUkProblemWithTheService(
        this PageModel pageModel,
        int statusCode = StatusCodes.Status500InternalServerError)
    {
        ArgumentNullException.ThrowIfNull(pageModel);
        return new GovUkErrorPageResult(GovUkErrorPageModel.ViewNames.ProblemWithTheService, statusCode);
    }
}
