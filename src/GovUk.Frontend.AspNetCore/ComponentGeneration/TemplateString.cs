using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Contains either an unencoded <see cref="string" /> or an <see cref="IHtmlContent"/>.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public readonly struct TemplateString : IEquatable<TemplateString>, IHtmlContent
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
    /// Gets an HTML string for the current <see cref="TemplateString"/>.
    /// </summary>
    /// <param name="encoder">The <see cref="HtmlEncoder"/> to encoded unencoded values with.</param>
    /// <param name="raw">Whether the raw, unencoded value should be returned for <see cref="string"/> values.</param>
    /// <returns>A string containing the HTML.</returns>
    public string ToHtmlString(HtmlEncoder? encoder = null, bool raw = false)
    {
        // Fast path for empty strings
        if (_value is "" or null)
        {
            return string.Empty;
        }

        if (_value is string str)
        {
            if (raw)
            {
                return str;
            }

            encoder ??= DefaultEncoder;
            return encoder.Encode(str);
        }

        Debug.Assert(_value is IHtmlContent);
        encoder ??= DefaultEncoder;
        return ((IHtmlContent)_value).ToHtmlString(encoder);
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
        var html = ((IHtmlContent)_value).ToHtmlString(encoder);
        return new StringValue(html, encode: false);
    }

    /// <summary>
    /// A <see cref="TemplateString"/> with no content.
    /// </summary>
    public static TemplateString Empty { get; } = new((string?)null);

    /// <summary>
    /// Concatenates two <see cref="TemplateString"/> instances.
    /// </summary>
#pragma warning disable CA2225
    public static TemplateString operator +(TemplateString? first, TemplateString? second)
#pragma warning restore CA2225
    {
        var firstValue = first ?? Empty;
        var secondValue = second ?? Empty;

        // Fast path for empty operands
        if (firstValue._value is "" or null)
        {
            return secondValue;
        }
        if (secondValue._value is "" or null)
        {
            return firstValue;
        }

        // Optimize concatenation for string + string case using StringBuilder to avoid intermediate allocations
        if (firstValue._value is string str1 && secondValue._value is string str2)
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
        var html1 = firstValue.ToHtmlString(DefaultEncoder);
        var html2 = secondValue.ToHtmlString(DefaultEncoder);
        var result = new StringBuilder(html1.Length + html2.Length);
        result.Append(html1);
        result.Append(html2);
        return new TemplateString(new HtmlString(result.ToString()));
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
    [return: NotNullIfNotNull(nameof(value))]
#pragma warning disable CA2225
    public static implicit operator TemplateString?(string? value)
    {
        return value is null ? null : new(value);
    }
#pragma warning restore CA2225

    /// <summary>
    /// Creates a <see cref="TemplateString"/> from <see cref="HtmlString"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/> to create the <see cref="TemplateString"/> from.</param>
    /// <returns>A new <see cref="TemplateString"/> wrapping the specified <see cref="HtmlString"/>.</returns>
    [return: NotNullIfNotNull(nameof(content))]
#pragma warning disable CA2225
    public static implicit operator TemplateString?(HtmlString? content)
    {
        return content is null ? null : new(content);
    }
#pragma warning restore CA2225

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator ==(TemplateString first, TemplateString second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(TemplateString first, TemplateString second)
    {
        return !first.Equals(second);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <inheritdoc/>
    public override string ToString() => ToHtmlString(DefaultEncoder);

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
        obj is TemplateString other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

    /// <inheritdoc/>
    public bool Equals(TemplateString other)
    {
        // Fast path: if both have the same underlying value reference, they're equal
        if (ReferenceEquals(_value, other._value))
        {
            return true;
        }

        // Fast path: both empty
        if ((_value is "" or null) && (other._value is "" or null))
        {
            return true;
        }

        // Fast path: one is empty, the other is not
        if ((_value is "" or null) || (other._value is "" or null))
        {
            return false;
        }

        // Fast path: both are strings - compare directly
        if (_value is string str1 && other._value is string str2)
        {
            return str1 == str2;
        }

        // Slow path: convert to HTML strings and compare
        return string.Equals(ToHtmlString(DefaultEncoder), other.ToHtmlString(DefaultEncoder), StringComparison.Ordinal);
    }
}

/// <summary>
/// Extensions for <see cref="TemplateString"/>.
/// </summary>
public static class TemplateStringExtensions
{
    // Estimated average class name length for capacity calculation
    private const int EstimatedClassNameLength = 20;

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> with the contents of <paramref name="templateString"/> and the
    /// specified <paramref name="classNames"/>.
    /// </summary>
    /// <param name="templateString">The initial set of CSS class names.</param>
    /// <param name="classNames">The additional CSS class names to append.</param>
    /// <returns>A new <see cref="TemplateString"/>.</returns>
    public static TemplateString AppendCssClasses(this TemplateString? templateString, params TemplateString[] classNames)
    {
        ArgumentNullException.ThrowIfNull(classNames);

        // Fast path for no additional classes
        if (classNames.Length == 0)
        {
            return templateString ?? TemplateString.Empty;
        }

        // Get the original value efficiently
        var originalHtml = templateString?.ToHtmlString(TemplateString.DefaultEncoder) ?? string.Empty;
        var original = originalHtml.AsSpan().Trim();

        // Calculate approximate capacity to minimize allocations
        int capacity = original.Length + classNames.Length * (EstimatedClassNameLength + 1); // +1 for space

        var sb = new StringBuilder(capacity);

        if (original.Length > 0)
        {
            sb.Append(original);
        }

        // Append classes with spaces
        for (int i = 0; i < classNames.Length; i++)
        {
            var classHtml = classNames[i].ToHtmlString(TemplateString.DefaultEncoder);

            // Skip empty class names
            if (classHtml.Length == 0)
            {
                continue;
            }

            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(classHtml);
        }

        return new TemplateString(new HtmlString(sb.ToString()));
    }

    /// <summary>
    /// Creates a <see cref="TemplateString"/> from <see cref="IHtmlContent"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/> to create the <see cref="TemplateString"/> from.</param>
    /// <returns>A new <see cref="TemplateString"/> wrapping the specified <see cref="IHtmlContent"/>.</returns>
    public static TemplateString ToTemplateString(this IHtmlContent? content) => new(content);
}
