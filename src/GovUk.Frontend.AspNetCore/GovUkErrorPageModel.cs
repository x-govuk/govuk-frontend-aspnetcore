namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// The model for the error page views rendered by
/// <see cref="GovUkFrontendExtensions.UseGovUkFrontendErrorPages(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>.
/// </summary>
public class GovUkErrorPageModel
{
    /// <summary>
    /// The names of the error page views.
    /// </summary>
    /// <remarks>
    /// An application can replace a page by adding a view with the same name to <c>Views/Shared</c> or
    /// <c>Pages/Shared</c>. Each default page also renders a partial view named after it with a <c>_</c> prefix and
    /// a <c>Content</c> suffix (e.g. <c>_GovUkPageNotFoundContent</c>), if one exists, for service-specific content
    /// such as contact details.
    /// </remarks>
#pragma warning disable CA1034 // Nested types should not be visible
    public static class ViewNames
#pragma warning restore CA1034
    {
        /// <summary>The &quot;Page not found&quot; page.</summary>
        public const string PageNotFound = "GovUkPageNotFound";

        /// <summary>The &quot;Sorry, there is a problem with the service&quot; page.</summary>
        public const string ProblemWithTheService = "GovUkProblemWithTheService";
    }

    /// <summary>
    /// Gets the status code of the response.
    /// </summary>
    public required int StatusCode { get; init; }
}
