using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Builds radios and checkboxes items from the members of an enum.
/// </summary>
internal static class EnumItemsHelper
{
    internal sealed record EnumMember(string Name, string NumericValue, ulong Bits, string DisplayName, string? Description);

    /// <summary>
    /// The enum that a component's <c>for</c> expression is bound to.
    /// </summary>
    /// <param name="EnumMetadata">The metadata for the enum type (or nullable enum type).</param>
    /// <param name="IsFlags">
    /// Whether the expression is a single <see cref="FlagsAttribute"/> enum whose items each represent one bit,
    /// rather than an enum (or collection of an enum) whose items each represent one value.
    /// </param>
    internal sealed record EnumBinding(ModelMetadata EnumMetadata, bool IsFlags);

    /// <summary>
    /// Gets the enum that items can be generated from for a radios component, or <c>null</c> if the
    /// expression isn't bound to an enum or nullable enum.
    /// </summary>
    public static EnumBinding? GetRadiosEnumBinding(ModelExpression? @for) =>
        @for?.Metadata is { IsEnum: true, IsFlagsEnum: false } metadata ? new EnumBinding(metadata, IsFlags: false) : null;

    /// <summary>
    /// Gets the enum that items can be generated from for a checkboxes component, or <c>null</c> if the
    /// expression isn't bound to a collection of an enum or to a single <see cref="FlagsAttribute"/> enum.
    /// </summary>
    public static EnumBinding? GetCheckboxesEnumBinding(ModelExpression? @for) => @for?.Metadata switch
    {
        { IsFlagsEnum: true } metadata => new EnumBinding(metadata, IsFlags: true),
        { IsEnumerableType: true, ElementMetadata: { IsEnum: true, IsFlagsEnum: false } elementMetadata } =>
            new EnumBinding(elementMetadata, IsFlags: false),
        _ => null
    };

    public static IReadOnlyList<EnumMember> GetMembers(
        EnumBinding binding,
        IEnumerable<Enum>? excludeValues,
        IServiceProvider? services)
    {
        ArgumentNullException.ThrowIfNull(binding);

        var enumMetadata = binding.EnumMetadata;
        var enumType = enumMetadata.UnderlyingOrModelType;
        var excludedNames = GetExcludedNames(enumType, excludeValues);
        var localizer = GetDataAnnotationsLocalizer(enumType, services);

        // EnumGroupedDisplayNamesAndValues is populated by the data annotations metadata provider; it honours
        // [Display(Name, ResourceType, Order)] and any configured data annotations localizer.
        IEnumerable<KeyValuePair<string, string>> displayNamesAndValues =
            enumMetadata.EnumGroupedDisplayNamesAndValues?.Select(kvp => KeyValuePair.Create(kvp.Key.Name, kvp.Value)) ??
            Enum.GetNames(enumType).Select(name => KeyValuePair.Create(
                name,
                Convert.ToString(
                    Convert.ChangeType(Enum.Parse(enumType, name), Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture),
                    CultureInfo.InvariantCulture)!));

        var namesByValue = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (name, value) in enumMetadata.EnumNamesAndValues ?? displayNamesAndValues.ToDictionary())
        {
            namesByValue.TryAdd(value, name);
        }

        var members = new List<EnumMember>();
        var seenNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (displayName, value) in displayNamesAndValues)
        {
            if (!namesByValue.TryGetValue(value, out var name) || excludedNames.Contains(name) || !seenNames.Add(name))
            {
                continue;
            }

            var bits = ToBits(Enum.Parse(enumType, name));

            // For a flags enum each item is one bit; the zero member and combinations of bits don't get items.
            if (binding.IsFlags && !IsSingleBit(bits))
            {
                continue;
            }

            var description = GetDescription(enumType.GetField(name, BindingFlags.Public | BindingFlags.Static), localizer);

            members.Add(new EnumMember(name, value, bits, displayName, description));
        }

        return members;
    }

    /// <summary>
    /// Whether <paramref name="value"/> refers to <paramref name="member"/>, by name (as model binding does,
    /// ignoring case) or by its numeric value.
    /// </summary>
    public static bool Matches(EnumMember member, string? value) =>
        value is not null &&
        (string.Equals(member.Name, value.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(member.NumericValue, value.Trim(), StringComparison.Ordinal));

    /// <summary>
    /// Creates a function that determines whether a member's item should be checked given the current values for
    /// the expression (see <see cref="GetCurrentValues"/>).
    /// </summary>
    public static Func<EnumMember, bool> CreateIsChecked(EnumBinding binding, IReadOnlyCollection<string> currentValues)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ArgumentNullException.ThrowIfNull(currentValues);

        if (!binding.IsFlags)
        {
            return member => currentValues.Any(v => Matches(member, v));
        }

        // Each value may itself be a combination, e.g. "Read, Write" from the model's ToString() or "3".
        var enumType = binding.EnumMetadata.UnderlyingOrModelType;
        ulong combined = 0;

        foreach (var value in currentValues)
        {
            if (Enum.TryParse(enumType, value, ignoreCase: true, out var parsed))
            {
                combined |= ToBits(parsed);
            }
        }

        return member => (combined & member.Bits) == member.Bits;
    }

    /// <summary>
    /// Converts an enum value to its bits, regardless of the enum's underlying type.
    /// </summary>
    private static ulong ToBits(object enumValue) =>
        Type.GetTypeCode(Enum.GetUnderlyingType(enumValue.GetType())) switch
        {
            TypeCode.Byte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 =>
                Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture),
            _ => unchecked((ulong)Convert.ToInt64(enumValue, CultureInfo.InvariantCulture))
        };

    private static bool IsSingleBit(ulong bits) => bits != 0 && (bits & (bits - 1)) == 0;

    private static IStringLocalizer? GetDataAnnotationsLocalizer(Type enumType, IServiceProvider? services)
    {
        // Mirrors MVC's data annotations metadata provider, which localizes an enum's [Display(Name)]s using a
        // localizer created for the enum type (when localization is configured).
        if (services?.GetService<IStringLocalizerFactory>() is not { } factory ||
            services.GetService<IOptions<MvcDataAnnotationsLocalizationOptions>>()?.Value.DataAnnotationLocalizerProvider is not { } localizerProvider)
        {
            return null;
        }

        return localizerProvider(enumType, factory);
    }

    private static string? GetDescription(FieldInfo? field, IStringLocalizer? localizer)
    {
        var display = field?.GetCustomAttribute<DisplayAttribute>(inherit: false);

        if (display is null)
        {
            return null;
        }

        // As MVC does for [Display(Name)]: a ResourceType takes precedence over the localizer.
        if (display.ResourceType is null && localizer is not null && !string.IsNullOrEmpty(display.Description))
        {
            return localizer[display.Description];
        }

        return display.GetDescription();
    }

    /// <summary>
    /// Gets the string values for the expression from <c>ModelState</c> if there's an attempted value,
    /// otherwise from the model.
    /// </summary>
    public static IReadOnlyCollection<string> GetCurrentValues(ViewContext viewContext, string fullName, object? model)
    {
        ArgumentNullException.ThrowIfNull(viewContext);
        ArgumentNullException.ThrowIfNull(fullName);

        if (viewContext.ViewData.ModelState.TryGetValue(fullName, out var entry) && entry.RawValue is { } rawValue)
        {
            return ToStrings(rawValue);
        }

        return model is null ? [] : ToStrings(model);

        static IReadOnlyCollection<string> ToStrings(object value) => value switch
        {
            string s => [s],
            IEnumerable e => e.Cast<object?>().Select(v => v?.ToString()).OfType<string>().ToArray(),
            _ => value.ToString() is { } s ? [s] : []
        };
    }

    /// <summary>
    /// Combines items generated from an enum's members with items declared in markup.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A declared item whose value matches a member replaces that member's generated item, in the member's position;
    /// <paramref name="fillDefaults"/> fills in its label and hint where they're missing.
    /// </para>
    /// <para>
    /// Any other declared item (a divider, or an item that isn't a member) goes immediately before the next declared
    /// item that matches a member, or after all the generated items if there's no such item.
    /// </para>
    /// </remarks>
    public static IReadOnlyCollection<TItem> Merge<TItem>(
        IReadOnlyList<EnumMember> members,
        IEnumerable<TItem> declaredItems,
        Func<TItem, string?> getValue,
        Func<TItem, bool> isDivider,
        Func<EnumMember, TItem> createItem,
        Func<TItem, EnumMember, TItem> fillDefaults)
    {
        ArgumentNullException.ThrowIfNull(members);
        ArgumentNullException.ThrowIfNull(declaredItems);

        var result = new List<TItem>();
        var pending = new List<TItem>();
        var nextMember = 0;

        foreach (var item in declaredItems)
        {
            var memberIndex = isDivider(item) ? -1 : IndexOfMember(getValue(item));

            if (memberIndex < nextMember)
            {
                // Not a member (or a member that's already been placed); place it with the next member.
                pending.Add(item);
                continue;
            }

            for (; nextMember < memberIndex; nextMember++)
            {
                result.Add(createItem(members[nextMember]));
            }

            result.AddRange(pending);
            pending.Clear();
            result.Add(fillDefaults(item, members[memberIndex]));
            nextMember = memberIndex + 1;
        }

        for (; nextMember < members.Count; nextMember++)
        {
            result.Add(createItem(members[nextMember]));
        }

        result.AddRange(pending);

        return result;

        int IndexOfMember(string? value)
        {
            for (var i = 0; i < members.Count; i++)
            {
                if (Matches(members[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
    }

    private static HashSet<string> GetExcludedNames(Type enumType, IEnumerable<Enum>? excludeValues)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (var value in excludeValues ?? [])
        {
            if (value.GetType() != enumType)
            {
                throw new ArgumentException(
                    $"Excluded values must be members of '{enumType.FullName}'; '{value}' is a '{value.GetType().FullName}'.",
                    nameof(excludeValues));
            }

            if (Enum.GetName(enumType, value) is { } name)
            {
                names.Add(name);
            }
        }

        return names;
    }
}
