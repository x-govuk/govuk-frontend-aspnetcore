using System.ComponentModel.DataAnnotations;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Specifies the maximum number of words allowed in a property.
/// </summary>
/// <remarks>
/// Validation attribute to assert a string property does not exceed a maximum number of words.
/// </remarks>
/// <param name="words">The maximum allowable number of words.</param>
public sealed class MaxWordsAttribute(int words) : ValidationAttribute
{

    /// <summary>
    /// Gets the maximum allowable number of words.
    /// </summary>
    public int Words { get; } = words;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (Words < 0)
        {
            throw new InvalidOperationException("The maximum length must be a positive integer.");
        }

        var validator = new MaxWordsValidator(Words);
        return validator.IsValid((string?)value);
    }
}
