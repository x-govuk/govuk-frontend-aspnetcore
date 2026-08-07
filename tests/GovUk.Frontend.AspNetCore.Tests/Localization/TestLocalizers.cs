using GovUk.Frontend.AspNetCore.Localization;

namespace GovUk.Frontend.AspNetCore.Tests.Localization;

/// <summary>
/// An <see cref="IGovUkFrontendLocalizer"/> that returns content from a delegate.
/// </summary>
internal sealed class DelegateLocalizer(Func<string, string?> getString) : IGovUkFrontendLocalizer
{
    public List<string> RequestedNames { get; } = [];

    public string? GetString(string name)
    {
        RequestedNames.Add(name);
        return getString(name);
    }

    /// <summary>
    /// Creates a localizer that returns content for <paramref name="name"/> only.
    /// </summary>
    public static DelegateLocalizer ForName(string name, string value) =>
        new(n => n == name ? value : null);

    /// <summary>
    /// Creates a localizer that returns <paramref name="value"/> for every name.
    /// </summary>
    public static DelegateLocalizer ForAllNames(string value) => new(_ => value);
}
