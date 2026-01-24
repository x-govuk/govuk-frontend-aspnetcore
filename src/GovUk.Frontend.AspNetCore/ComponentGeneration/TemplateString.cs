using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Contains either an unencoded <see cref="string" /> or an <see cref="IHtmlContent"/>.
/// </summary>
[DebuggerDisplay("{DebuggerToString()}")]
public sealed class TemplateString : IEquatable<TemplateString>, IHtmlContent
{
    internal static HtmlEncoder DefaultEncoder { get; } = HtmlEncoder.Default;

    private readonly object? _value;

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from an unencoded <see cref="string"/>.
    /// </summary>
    /// <param name="value">The unencoded <see cref="string"/>.</param>
    public TemplateString(string? value)
    {
        _value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from an <see cref="IHtmlContent"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/>.</param>
    public TemplateString(IHtmlContent? content)
    {
        // TagHelperContent instances get re-used after the tag helper has been rendered;
        // we need to snapshot the current content to ensure we don't get the wrong value.
        if (content is TagHelperContent tagHelperContent)
        {
            content = new HtmlString(tagHelperContent.GetContent());
        }

        _value = (object?)content ?? string.Empty;
    }

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from an interpolated string.
    /// </summary>
    public TemplateString(TemplateStringInterpolatedStringHandler builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _value = new HtmlString(builder.GetFormattedText());
    }

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from an encoded <see cref="string"/>.
    /// </summary>
    /// <param name="value">The encoded <see cref="string"/>.</param>
    /// <returns>A new <see cref="TemplateString"/> with the contents of the specified <see cref="string"/>.</returns>
    public static TemplateString FromEncoded(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new TemplateString(new HtmlString(value));
    }

    /// <inheritdoc cref="Join(string, IEnumerable{TemplateString?})"/>
    public static TemplateString Join(string separator, params TemplateString?[] content) =>
        Join(separator, content.AsEnumerable());

    /// <summary>
    /// Joins multiple <see cref="TemplateString"/> instances with the specified separator.
    /// </summary>
    /// <param name="separator">The separator to use between each item.</param>
    /// <param name="content">The content items to join.</param>
    /// <returns>A new <see cref="TemplateString"/> with the joined content.</returns>
    public static TemplateString Join(string separator, IEnumerable<TemplateString?> content)
    {
        ArgumentNullException.ThrowIfNull(separator);
        ArgumentNullException.ThrowIfNull(content);

        var builder = new StringBuilder();
        using var writer = new StringWriter(builder);

        var first = true;
        foreach (var item in content)
        {
            if (item is null || item.IsEmpty())
            {
                continue;
            }

            if (!first)
            {
                builder.Append(separator);
            }

            item.WriteTo(writer, DefaultEncoder);
            first = false;
        }

        return FromEncoded(builder.ToString());
    }

    /// <summary>
    /// Returns the first non-empty <see cref="TemplateString"/> from the specified values.
    /// </summary>
    /// <param name="values">The array of <see cref="TemplateString"/> values to check.</param>
    /// <returns>The first non-empty <see cref="TemplateString"/> from the array, or <see cref="TemplateString.Empty"/> if all values are empty or null.</returns>
    public static TemplateString Coalesce(params TemplateString?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (var value in values)
        {
            if (value is not null && !value.IsEmpty())
            {
                return value;
            }
        }

        return TemplateString.Empty;
    }

    internal FluidValue ToFluidValue(HtmlEncoder encoder)
    {
        // Fast path for empty strings
        if (_value is "" or null)
        {
            return NilValue.Instance;
        }

        if (_value is string str)
        {
            return new StringValue(str, encode: true);
        }

        Debug.Assert(_value is IHtmlContent);
        var html = this.ToHtmlString(encoder);
        return new StringValue(html, encode: false);
    }

    /// <summary>
    /// A <see cref="TemplateString"/> with no content.
    /// </summary>
    public static TemplateString Empty { get; } = new(HtmlString.Empty);

    /// <summary>
    /// Concatenates two <see cref="TemplateString"/> instances.
    /// </summary>
#pragma warning disable CA2225
    public static TemplateString operator +(TemplateString? first, TemplateString? second)
#pragma warning restore CA2225
    {
        first ??= Empty;
        second ??= Empty;

        // Fast path for empty operands
        if (first._value is "" or null)
        {
            return second;
        }
        if (second._value is "" or null)
        {
            return first;
        }

        // Optimize concatenation for string + string case using StringBuilder to avoid intermediate allocations
        if (first._value is string str1 && second._value is string str2)
        {
            // Both are strings - encode both and concatenate using StringBuilder
            var encoded1 = DefaultEncoder.Encode(str1);
            var encoded2 = DefaultEncoder.Encode(str2);
            var sb = new StringBuilder(encoded1.Length + encoded2.Length);
            sb.Append(encoded1);
            sb.Append(encoded2);
            return new TemplateString(new HtmlString(sb.ToString()));
        }

        // At least one is IHtmlContent - concatenate their HTML representations
        using var writer = new StringWriter();
        first.WriteTo(writer, DefaultEncoder);
        second.WriteTo(writer, DefaultEncoder);
        return FromEncoded(writer.ToString());
    }

    /// <summary>
    /// Concatenates a <see cref="TemplateString"/> and a <see cref="string"/>.
    /// </summary>
#pragma warning disable CA2225
    public static TemplateString operator +(TemplateString? first, string? second)
#pragma warning restore CA2225
    {
        return first + new TemplateString(second);
    }

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from the specified unencoded <see cref="string"/>.
    /// </summary>
    /// <param name="value">The unencoded <see cref="string"/>.</param>
    /// <returns>A new <see cref="TemplateString"/> with the contents of the specified <see cref="string"/>.</returns>
#pragma warning disable CA2225
    public static implicit operator TemplateString(string? value)
    {
        return value is null ? Empty : new(value);
    }
#pragma warning restore CA2225

    /// <summary>
    /// Creates a <see cref="TemplateString"/> from <see cref="HtmlString"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/> to create the <see cref="TemplateString"/> from.</param>
    /// <returns>A new <see cref="TemplateString"/> wrapping the specified <see cref="HtmlString"/>.</returns>
#pragma warning disable CA2225
    public static implicit operator TemplateString(HtmlString? content)
    {
        return content is null ? Empty : new(content);
    }
#pragma warning restore CA2225

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator ==(TemplateString? first, TemplateString? second)
    {
        return (first is null && second is null) || (first is not null && second is not null && first.Equals(second));
    }

    public static bool operator !=(TemplateString? first, TemplateString? second)
    {
        return !(first == second);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <inheritdoc cref="IHtmlContent.WriteTo"/>
    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(encoder);

        // Fast path for empty
        if (_value is "" or null)
        {
            return;
        }

        if (_value is string str)
        {
            // Use encoder.Encode directly to writer to avoid string allocation
            encoder.Encode(writer, str);
            return;
        }

        Debug.Assert(_value is IHtmlContent);
        ((IHtmlContent)_value).WriteTo(writer, encoder);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || (obj is TemplateString other && Equals(other));

    /// <inheritdoc/>
    public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

    /// <inheritdoc/>
    public bool Equals(TemplateString? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // Fast path: if both have the same underlying value reference, they're equal
        if (ReferenceEquals(_value, other._value))
        {
            return true;
        }

        // Fast path: both empty
        if (_value is "" or null && other._value is "" or null)
        {
            return true;
        }

        // Fast path: one is empty, the other is not
        if (_value is "" or null || other._value is "" or null)
        {
            return false;
        }

        // Fast path: both are strings - compare directly
        if (_value is string str1 && other._value is string str2)
        {
            return str1 == str2;
        }

        // Slow path: convert to HTML strings and compare
        return string.Equals(this.ToHtmlString(DefaultEncoder), other.ToHtmlString(DefaultEncoder), StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether this <see cref="TemplateString"/> contains the specified <see cref="TemplateString"/>.
    /// </summary>
    /// <param name="other">The <see cref="TemplateString"/> to locate in this instance.</param>
    /// <returns><see langword="true"/> if the specified <see cref="TemplateString"/> is found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(TemplateString? other)
    {
        if (other is null)
        {
            return false;
        }

        // Fast path: both empty
        if (_value is "" or null && other._value is "" or null)
        {
            return true;
        }

        // Fast path: one is empty, the other is not
        if (_value is "" or null || other._value is "" or null)
        {
            return false;
        }

        // Fast path: both are strings - compare directly
        if (_value is string str1 && other._value is string str2)
        {
            return str1.Contains(str2, StringComparison.Ordinal);
        }

        // Slow path: convert to HTML strings and compare
        var thisHtml = this.ToHtmlString(DefaultEncoder);
        var otherHtml = other.ToHtmlString(DefaultEncoder);
        return thisHtml.Contains(otherHtml, StringComparison.Ordinal);
    }

    // TEMP
    internal IHtmlContent GetRawHtml()
    {
        if (_value is string str)
        {
            return new HtmlString(str);
        }

        Debug.Assert(_value is IHtmlContent);
        return (IHtmlContent)_value;
    }

    private string DebuggerToString() => this.ToHtmlString(DefaultEncoder);
}

/// <summary>
/// Extensions for <see cref="TemplateString"/>.
/// </summary>
public static class TemplateStringExtensions
{
    /// <summary>
    /// Creates a new <see cref="TemplateString"/> with the contents of <paramref name="templateString"/> and the
    /// specified <paramref name="classNames"/>.
    /// </summary>
    /// <param name="templateString">The initial set of CSS class names.</param>
    /// <param name="classNames">The additional CSS class names to append.</param>
    /// <returns>A new <see cref="TemplateString"/>.</returns>
    public static TemplateString AppendCssClasses(this TemplateString? templateString, params TemplateString?[] classNames)
    {
        ArgumentNullException.ThrowIfNull(classNames);

        // Fast path for no additional classes
        if (classNames.Length == 0)
        {
            return templateString ?? TemplateString.Empty;
        }

        return TemplateString.Join(
            " ",
            !templateString.IsEmpty() ? new[] { templateString }.Concat(classNames) : classNames);
    }

    /// <summary>
    /// Creates a <see cref="TemplateString"/> from <see cref="IHtmlContent"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/> to create the <see cref="TemplateString"/> from.</param>
    /// <returns>A new <see cref="TemplateString"/> wrapping the specified <see cref="IHtmlContent"/>.</returns>
    public static TemplateString ToTemplateString(this IHtmlContent? content) => new(content);
}

/// <summary>
/// Providers a handler for building HTML content with interpolation that is safely encoded.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
#pragma warning disable CA1815
#pragma warning disable CA1815
#pragma warning disable CA1001
public struct TemplateStringInterpolatedStringHandler
#pragma warning restore CA1001
#pragma warning restore CA1815
#pragma warning restore CA1815
{
    private static readonly HtmlEncoder _encoder = TemplateString.DefaultEncoder;

    private readonly StringWriter _writer;

    /// <summary>Initializes a new instance of the <see cref="TemplateStringInterpolatedStringHandler"/> struct.</summary>
    // ReSharper disable UnusedParameter.Local
    public TemplateStringInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _writer = new();
    }
    // ReSharper restore UnusedParameter.Local

    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value)
    {
        _writer.Write(_encoder.Encode(value));
    }

    /// <summary>Writes the specified value to the handler.</summary>
    /// <param name="value">The value to write.</param>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    public void AppendFormatted<T>(T value)
    {
        if (value is null)
        {
            return;
        }

        if (value is IHtmlContent htmlContent)
        {
            htmlContent.WriteTo(_writer, _encoder);
        }
        else
        {
            var str = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (str is not null)
            {
                _writer.Write(_encoder.Encode(str));
            }
        }
    }

    internal string GetFormattedText() => _writer.ToString();
}
