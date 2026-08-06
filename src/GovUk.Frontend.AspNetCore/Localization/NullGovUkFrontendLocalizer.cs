namespace GovUk.Frontend.AspNetCore.Localization;

/// <summary>
/// An <see cref="IGovUkFrontendLocalizer"/> that provides no content, so the library's built-in
/// English content is always used.
/// </summary>
/// <remarks>
/// This is the implementation registered by
/// <see cref="GovUkFrontendExtensions.AddGovUkFrontend(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/>
/// when no other <see cref="IGovUkFrontendLocalizer"/> has been registered.
/// </remarks>
public sealed class NullGovUkFrontendLocalizer : IGovUkFrontendLocalizer
{
    private NullGovUkFrontendLocalizer()
    {
    }

    /// <summary>
    /// Gets the singleton <see cref="NullGovUkFrontendLocalizer"/> instance.
    /// </summary>
    public static NullGovUkFrontendLocalizer Instance { get; } = new();

    /// <inheritdoc/>
    public string? GetString(string name) => null;
}
