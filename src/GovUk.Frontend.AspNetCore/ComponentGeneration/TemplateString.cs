using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
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

    // Either an unencoded string, an already-encoded IHtmlContent, or a Composite of both. Composites
    // are kept unrendered so that composition doesn't have to pick an encoder, and so that a value
    // built from text stays recognizable as text.
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
        // Empty parts aren't dropped here the way Join drops them: the literals between holes are
        // content, and a literal that's only whitespace is still meaningful.
        var parts = builder.GetParts();

        _value = parts.Count switch
        {
            0 => string.Empty,
            1 => parts[0]._value,
            _ => new Composite([.. parts], Separator: string.Empty)
        };
    }

    private TemplateString(Composite composite)
    {
        _value = composite;
    }

    /// <summary>
    /// A sequence of values written in order, without committing to an encoder up front.
    /// </summary>
    private sealed record Composite(TemplateString[] Parts, string Separator);

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

        var parts = content.Where(item => item is not null && !item.IsEmpty()).ToArray();

        return parts.Length switch
        {
            0 => Empty,
            1 => parts[0]!,
            _ => new TemplateString(new Composite(parts!, separator))
        };
    }

    /// <summary>
    /// Concatenates the specified values, keeping each one's encoding.
    /// </summary>
    /// <param name="values">The values to concatenate.</param>
    /// <returns>A new <see cref="TemplateString"/> with the concatenated content.</returns>
    public static TemplateString Concat(params TemplateString?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return Join(string.Empty, values);
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

        return Empty;
    }

    /// <summary>
    /// A <see cref="TemplateString"/> with no content.
    /// </summary>
    public static TemplateString Empty { get; } = new(string.Empty);

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

        // Text stays text: concatenating two unencoded strings gives an unencoded string, so the
        // result still encodes at write time using whatever encoder the caller supplies.
        if (first._value is string str1 && second._value is string str2)
        {
            return new TemplateString(string.Concat(str1, str2));
        }

        return new TemplateString(new Composite([first, second], Separator: string.Empty));
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

    /// <summary>
    /// Gets whether this instance holds already-encoded HTML rather than plain text.
    /// </summary>
    /// <remarks>
    /// A value built by concatenating or joining others is HTML if any of its parts is.
    /// </remarks>
    public bool IsHtml => _value switch
    {
        Composite composite => Array.Exists(composite.Parts, part => part.IsHtml),
        IHtmlContent => true,
        _ => false
    };

    /// <summary>
    /// Gets the plain, unencoded text of this instance when it has an unambiguous text reading.
    /// </summary>
    /// <param name="text">
    /// When this method returns <see langword="true"/>, the unencoded text; otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when this instance was created from text, or from HTML containing no
    /// character an HTML encoder would escape — in which case its text and HTML forms are identical.
    /// Otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This never HTML-decodes: decoding isn't the inverse of encoding, so a decoded value wouldn't
    /// re-encode to the same markup. Use this for values that are identifiers, attribute tokens or
    /// parseable data; to write content out, write the <see cref="TemplateString"/> itself.
    /// </remarks>
    public bool TryGetText([NotNullWhen(true)] out string? text)
    {
        if (_value is null)
        {
            text = string.Empty;
            return true;
        }

        if (_value is string str)
        {
            text = str;
            return true;
        }

        if (_value is Composite composite)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < composite.Parts.Length; i++)
            {
                if (!composite.Parts[i].TryGetText(out var partText))
                {
                    text = null;
                    return false;
                }

                if (i > 0)
                {
                    builder.Append(composite.Separator);
                }

                builder.Append(partText);
            }

            text = builder.ToString();
            return true;
        }

        // Encoding is what tells the two readings apart, so ask it directly: content that encoding
        // leaves alone reads identically as text and as HTML. Testing against the default encoder is
        // the conservative choice, since it escapes the most.
        var rendered = Render(DefaultEncoder);

        if (string.Equals(DefaultEncoder.Encode(rendered), rendered, StringComparison.Ordinal))
        {
            text = rendered;
            return true;
        }

        text = null;
        return false;
    }

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

        if (_value is Composite composite)
        {
            for (var i = 0; i < composite.Parts.Length; i++)
            {
                if (i > 0 && composite.Separator.Length > 0)
                {
                    // The separator is text, so it's encoded like any other text.
                    encoder.Encode(writer, composite.Separator);
                }

                composite.Parts[i].WriteTo(writer, encoder);
            }

            return;
        }

        Debug.Assert(_value is IHtmlContent);
        ((IHtmlContent)_value).WriteTo(writer, encoder);
    }

    internal string Render(HtmlEncoder? encoder = null)
    {
        encoder ??= DefaultEncoder;

        if (_value is IHtmlContent and HtmlString htmlString)
        {
            return htmlString.Value ?? string.Empty;
        }

        using var writer = new StringWriter();
        WriteTo(writer, encoder);
        return writer.ToString();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || (obj is TemplateString other && Equals(other));

    /// <inheritdoc/>
    /// <remarks>
    /// Hashes the rendered form, so that it agrees with <see cref="Equals(TemplateString?)"/>.
    /// </remarks>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Render(DefaultEncoder));

    /// <inheritdoc/>
    /// <remarks>
    /// Two instances are equal when they render the same HTML, whether or not they hold the same kind
    /// of content.
    /// </remarks>
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

        // Fast path: both are strings - compare directly
        if (_value is string str1 && other._value is string str2)
        {
            return str1 == str2;
        }

        // Slow path: convert to HTML strings and compare
        return string.Equals(Render(DefaultEncoder), other.Render(DefaultEncoder), StringComparison.Ordinal);
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
        return Render(DefaultEncoder).Contains(other.Render(DefaultEncoder), StringComparison.Ordinal);
    }

    private string DebuggerToString() => Render(DefaultEncoder);
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

    internal static TemplateString WithEmptyFallback(this TemplateString? templateString, TemplateString? fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);

        return !templateString.IsEmpty() ? templateString : fallback;
    }

    /// <summary>
    /// Gets the text of <paramref name="templateString"/> for use as an identifier, key or attribute
    /// token, falling back to its rendered HTML when it has no unambiguous text reading.
    /// </summary>
    /// <remarks>
    /// The fallback only applies to values holding real markup, which none of these call sites expect;
    /// it keeps them behaving as they did when they rendered unconditionally.
    /// </remarks>
    internal static string ToText(this TemplateString? templateString) =>
        templateString is null ? string.Empty :
        templateString.TryGetText(out var text) ? text :
        templateString.Render();
}

/// <summary>
/// Providers a handler for building HTML content with interpolation that is safely encoded.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[InterpolatedStringHandler]
#pragma warning disable CA1815
public struct TemplateStringInterpolatedStringHandler
#pragma warning restore CA1815
{
    // Parts are collected rather than rendered, so the result is written with the caller's encoder
    // and a value interpolated from literals and text is still recognizable as text.
    private readonly List<TemplateString> _parts;

    /// <summary>Initializes a new instance of the <see cref="TemplateStringInterpolatedStringHandler"/> struct.</summary>
    // ReSharper disable UnusedParameter.Local
    public TemplateStringInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        _parts = new List<TemplateString>(formattedCount * 2 + 1);
    }
    // ReSharper restore UnusedParameter.Local

    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value)
    {
        _parts.Add(new TemplateString(value));
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

        if (value is TemplateString templateString)
        {
            // Added as-is; wrapping it would relabel a text hole as HTML.
            _parts.Add(templateString);
        }
        else if (value is IHtmlContent htmlContent)
        {
            _parts.Add(new TemplateString(htmlContent));
        }
        else
        {
            var str = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (str is not null)
            {
                _parts.Add(new TemplateString(str));
            }
        }
    }

    internal readonly IReadOnlyList<TemplateString> GetParts() => _parts;
}
