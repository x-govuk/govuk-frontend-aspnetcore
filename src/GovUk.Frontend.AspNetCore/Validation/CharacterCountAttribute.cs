using System.ComponentModel.DataAnnotations;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Specifies the maximum number of characters or words allowed in a property, counted the same way as a GDS character
/// count component.
/// </summary>
/// <remarks>
/// <para>Use this when the component's limit is set with <c>max-length</c>, setting <see cref="CountType"/> to match
/// the component's <c>count-type</c>. When the limit is set with the deprecated <c>max-words</c> attribute, use
/// <see cref="MaxWordsAttribute"/>.</para>
/// </remarks>
/// <param name="maxLength">The maximum allowable number of characters, or of words when <see cref="CountType"/> is
/// <see cref="CharacterCountType.Words"/>.</param>
public sealed class CharacterCountAttribute(int maxLength) : ValidationAttribute
{
    /// <summary>
    /// Gets the maximum allowable number of characters, or of words when <see cref="CountType"/> is
    /// <see cref="CharacterCountType.Words"/>.
    /// </summary>
    public int MaxLength { get; } = maxLength;

    /// <summary>
    /// Gets or sets how the text is counted.
    /// </summary>
    /// <remarks>
    /// The default is <see cref="CharacterCountType.Length"/>.
    /// </remarks>
    public CharacterCountType CountType { get; set; } = CharacterCountType.Length;

    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        if (MaxLength <= 0)
        {
            throw new InvalidOperationException("The maximum length must be a positive integer.");
        }

        if (value is null)
        {
            return true;
        }

        var validator = new CharacterCountValidator(MaxLength, CountType);
        return validator.IsValid((string)value);
    }
}
