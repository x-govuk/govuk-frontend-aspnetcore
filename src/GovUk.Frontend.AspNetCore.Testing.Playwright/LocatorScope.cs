using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// What a lookup searches within: a whole page or a locator. <see cref="IPage"/> and <see cref="ILocator"/> have the
/// same lookups with different option types, so the extension members on each go through this.
/// </summary>
internal sealed class LocatorScope
{
    private readonly IPage? _page;
    private readonly ILocator? _locator;

    private LocatorScope(IPage? page, ILocator? locator)
    {
        _page = page;
        _locator = locator;
    }

    /// <summary>
    /// Gets the page, for creating the inner locators of <see cref="LocatorFilterOptions.Has"/>, which are relative
    /// to the element being filtered.
    /// </summary>
    public IPage Page => _page ?? _locator!.Page;

    public static LocatorScope For(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);
        return new(page, null);
    }

    public static LocatorScope For(ILocator locator)
    {
        ArgumentNullException.ThrowIfNull(locator);
        return new(null, locator);
    }

    public ILocator Locator(string selector) => _page?.Locator(selector) ?? _locator!.Locator(selector);

    /// <summary>
    /// Gets the form controls whose label is exactly <paramref name="label"/>, ignoring surrounding whitespace.
    /// </summary>
    public ILocator GetByLabel(string label) =>
        _page?.GetByLabel(label, new() { Exact = true }) ?? _locator!.GetByLabel(label, new() { Exact = true });

    /// <summary>
    /// Gets the elements with <paramref name="role"/> whose accessible name is exactly <paramref name="name"/>,
    /// ignoring surrounding whitespace.
    /// </summary>
    public ILocator GetByRole(AriaRole role, string name) =>
        _page?.GetByRole(role, new() { Name = name, Exact = true }) ??
        _locator!.GetByRole(role, new() { Name = name, Exact = true });
}

internal static class Selectors
{
    /// <summary>
    /// Selects the nearest form group around an element.
    /// </summary>
    public const string ClosestFormGroup =
        "xpath=ancestor::*[contains(concat(' ', normalize-space(@class), ' '), ' govuk-form-group ')][1]";

    /// <summary>
    /// Formats <paramref name="value"/> as a quoted CSS attribute value, e.g. for <c>[name="..."]</c>.
    /// </summary>
    public static string AttributeValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
    }

    /// <summary>
    /// Matches text that is exactly <paramref name="text"/>, allowing for differences in whitespace.
    /// </summary>
    public static Regex ExactText(string text) => new($"^\\s*{Words(text)}\\s*$");

    /// <summary>
    /// Matches text that starts with the words in <paramref name="text"/>, e.g. a link's visible text followed by
    /// visually hidden text.
    /// </summary>
    public static Regex StartsWithWords(string text) => new($"^\\s*{Words(text)}(?:\\s|$)");

    /// <summary>
    /// Matches the words in <paramref name="text"/>, separated by any whitespace. <see cref="Regex.Escape"/>'s
    /// escapes are all valid in the JavaScript regular expressions Playwright converts these to.
    /// </summary>
    internal static string Words(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return string.Join(
            "\\s+",
            text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape));
    }
}
