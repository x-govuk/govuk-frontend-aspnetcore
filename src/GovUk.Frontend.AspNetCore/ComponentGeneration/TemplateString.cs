using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Contains either an unencoded <see cref="System.String" /> or an <see cref="IHtmlContent"/>.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public sealed class TemplateString : IEquatable<TemplateString>, IHtmlContent
{
    private readonly object? _value;

    /// <summary>
    /// Creates a new <see cref="TemplateString"/> from an unencoded <see cref="String"/>.
    /// </summary>
    /// <param name="value">The unencoded <see cref="String"/>.</param>
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
    /// <param name="raw">Whether the raw, unencoded value should be returned for <see cref="String"/> values.</param>
    /// <returns>A string containing the HTML.</returns>
    public string ToHtmlString(HtmlEncoder encoder, bool raw = false)
    {
        ArgumentNullException.ThrowIfNull(encoder);

        if (_value is string str)
        {
            return raw ? str : encoder.Encode(str);
        }

        Debug.Assert(_value is IHtmlContent);
        return ((IHtmlContent)_value).ToHtmlString(encoder);
    }

    internal FluidValue ToFluidValue(HtmlEncoder encoder)
    {
        if (_value is null)
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
    /// Creates a new <see cref="TemplateString"/> from the specified unencoded <see cref="String"/>.
    /// </summary>
    /// <param name="value">The unencoded <see cref="String"/>.</param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static implicit operator TemplateString?(string? value) => value is null ? null : new(value);

    /// <summary>
    /// Creates a <see cref="TemplateString"/> from <see cref="HtmlString"/>.
    /// </summary>
    /// <param name="content">The <see cref="IHtmlContent"/> to create the <see cref="TemplateString"/> from.</param>
    /// <returns>A new <see cref="TemplateString"/> wrapping the specified <see cref="HtmlString"/>.</returns>
    [return: NotNullIfNotNull(nameof(content))]
    public static implicit operator TemplateString?(HtmlString? content) => content is null ? null : new(content);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static bool operator ==(TemplateString? first, TemplateString? second) =>
        first is null && second is null || (first is not null && second is not null && first.Equals(second));

    public static bool operator !=(TemplateString? first, TemplateString? second) => !(first == second);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <inheritdoc/>
    public override string ToString() => ToHtmlString(HtmlEncoder.Default);

    /// <inheritdoc cref="IHtmlContent.WriteTo"/>
    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(encoder);

        if (_value is string str)
        {
            writer.Write(encoder.Encode(str));
            return;
        }

        Debug.Assert(_value is IHtmlContent);
        ((IHtmlContent)_value).WriteTo(writer, encoder);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj) || obj is TemplateString other && Equals(other);

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

        return Equals(ToString(), other.ToString());
    }
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
    /// <param name="encoder">The <see cref="HtmlEncoder"/> to encode values with.</param>
    /// <param name="classNames">The additional CSS class names to append.</param>
    /// <returns>A new <see cref="TemplateString"/>.</returns>
    public static TemplateString AppendCssClasses(this TemplateString? templateString, HtmlEncoder encoder, params TemplateString[] classNames)
    {
        ArgumentNullException.ThrowIfNull(encoder);

        var original = templateString?.ToHtmlString(encoder).Trim() ?? "";

        if (classNames.Length == 0)
        {
            return original;
        }

        var sb = new StringBuilder();

        if (original.Length > 0)
        {
            sb.Append(original);
            sb.Append(' ');
        }

        foreach (var className in classNames)
        {
            sb.Append(className.ToHtmlString(encoder));
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
