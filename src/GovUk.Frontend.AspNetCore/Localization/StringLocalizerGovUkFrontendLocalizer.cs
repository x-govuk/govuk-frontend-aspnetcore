using Microsoft.Extensions.Localization;

namespace GovUk.Frontend.AspNetCore.Localization;

/// <summary>
/// An <see cref="IGovUkFrontendLocalizer"/> backed by an <see cref="IStringLocalizer"/>.
/// </summary>
internal sealed class StringLocalizerGovUkFrontendLocalizer : IGovUkFrontendLocalizer
{
    private readonly IStringLocalizer _stringLocalizer;

    public StringLocalizerGovUkFrontendLocalizer(IStringLocalizer stringLocalizer)
    {
        ArgumentNullException.ThrowIfNull(stringLocalizer);

        _stringLocalizer = stringLocalizer;
    }

    public string? GetString(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        // IStringLocalizer returns the name itself when the resource is missing, so ResourceNotFound
        // is the only reliable way to tell "not translated" from "translated to something".
        var localizedString = _stringLocalizer[name];

        return localizedString.ResourceNotFound || string.IsNullOrEmpty(localizedString.Value)
            ? null
            : localizedString.Value;
    }
}
