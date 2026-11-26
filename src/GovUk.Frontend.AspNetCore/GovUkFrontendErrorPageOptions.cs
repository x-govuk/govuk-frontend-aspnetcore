namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Options for the error pages added by
/// <see cref="GovUkFrontendExtensions.UseGovUkFrontendErrorPages(Microsoft.AspNetCore.Builder.IApplicationBuilder)"/>.
/// </summary>
/// <remarks>
/// Configure these with <c>services.Configure&lt;GovUkFrontendErrorPageOptions&gt;(...)</c>. They are read when the
/// middleware is added, so changes after startup have no effect.
/// </remarks>
public class GovUkFrontendErrorPageOptions
{
    /// <summary>
    /// Gets or sets whether a &quot;Page not found&quot; page is rendered for <c>404</c> responses that have no body.
    /// </summary>
    /// <remarks>
    /// The default is <see langword="true"/>.
    /// </remarks>
    public bool HandlePageNotFound { get; set; } = true;

    /// <summary>
    /// Gets or sets whether a &quot;Sorry, there is a problem with the service&quot; page is rendered for
    /// unhandled exceptions.
    /// </summary>
    /// <remarks>
    /// <para>The default is <see langword="true"/>.</para>
    /// <para>
    /// The exception handler hides the developer exception page, so you may want to set this to
    /// <see langword="false"/> in development.
    /// </para>
    /// </remarks>
    public bool HandleExceptions { get; set; } = true;

    /// <summary>
    /// Gets or sets the layout used by the error pages.
    /// </summary>
    /// <remarks>
    /// When this is <see langword="null"/>, the pages use the layout set by the application's
    /// <c>Views/_ViewStart.cshtml</c>, or <c>_GovUkPageTemplate</c> when that doesn't set one.
    /// </remarks>
    public string? Layout { get; set; }
}
