using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Specifies the maximum number of words allowed in a property.
/// </summary>
/// <remarks>
/// <para>Validation attribute to assert a string property does not exceed a maximum number of words.</para>
/// <para>This counts words the same way as a GDS character count component whose limit is set with the deprecated
/// <c>max-words</c> attribute. When the limit is set with <c>max-length</c> and a <c>count-type</c> of
/// <see cref="CharacterCountType.Words"/>, use <see cref="CharacterCountAttribute"/> instead.</para>
/// </remarks>
/// <param name="words">The maximum allowable number of words.</param>
public sealed class MaxWordsAttribute(int words) : ValidationAttribute
{

    /// <summary>
    /// Gets the maximum allowable number of words.
    /// </summary>
    public int Words { get; } = words;

    /// <summary>
    /// Formats the error message, with <paramref name="name"/> as <c>{0}</c> and <see cref="Words"/> as <c>{1}</c>.
    /// </summary>
    /// <param name="name">The name of the field in error.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Words);

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (Words < 0)
        {
            throw new InvalidOperationException("The maximum length must be a positive integer.");
        }

        if (value is null)
        {
            return true;
        }

        var validator = new MaxWordsValidator(Words);
        return validator.IsValid((string?)value);
    }
}
