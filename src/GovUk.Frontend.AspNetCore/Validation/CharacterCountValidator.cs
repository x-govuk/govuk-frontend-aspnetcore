using System.Globalization;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Helper class to validate strings against the limit of a GDS character count component, counting the text the
/// same way as the component's JavaScript.
/// </summary>
/// <remarks>
/// <para>Use this when the component's limit is set with <c>max-length</c>. When the limit is set with the deprecated
/// <c>max-words</c> attribute, use <see cref="MaxWordsValidator"/>.</para>
/// <para>Line breaks are counted as one character, as they are in the browser, even though they're submitted as
/// two (<c>\r\n</c>).</para>
/// </remarks>
public class CharacterCountValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterCountValidator"/> class.
    /// </summary>
    /// <param name="maxLength">The maximum allowable number of characters, or of words when
    /// <paramref name="countType"/> is <see cref="CharacterCountType.Words"/>.</param>
    /// <param name="countType">How the text is counted. This must match the component's <c>count-type</c>.</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="maxLength"/> argument is not a positive integer
    /// or <paramref name="countType"/> is not a valid <see cref="CharacterCountType"/>.</exception>
    public CharacterCountValidator(int maxLength, CharacterCountType countType = CharacterCountType.Length)
    {
        if (maxLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength must be a positive integer.");
        }

        if (!Enum.IsDefined(countType))
        {
            throw new ArgumentOutOfRangeException(nameof(countType), $"{countType} is not a valid {nameof(CharacterCountType)}.");
        }

        MaxLength = maxLength;
        CountType = countType;
    }

    /// <summary>
    /// Gets the maximum allowable number of characters, or of words when <see cref="CountType"/> is
    /// <see cref="CharacterCountType.Words"/>.
    /// </summary>
    public int MaxLength { get; }

    /// <summary>
    /// Gets how the text is counted.
    /// </summary>
    public CharacterCountType CountType { get; }

    /// <summary>
    /// Counts the specified value as the component's JavaScript would.
    /// </summary>
    /// <param name="value">The value to count.</param>
    /// <returns>The number of characters, or of words when <see cref="CountType"/> is <see cref="CharacterCountType.Words"/>.</returns>
    public int GetCount(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return CountType switch
        {
            // A textarea's value in the browser has its line breaks normalized to \n, but they're submitted as \r\n.
            CharacterCountType.Length => value.Length - CountCrLfs(value),
            // \r\n is a single grapheme cluster, so needs no adjustment.
            CharacterCountType.Characters => new StringInfo(value).LengthInTextElements,
            _ => WordCounter.Count(value)
        };
    }

    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <remarks>
    /// A <see langword="null"/> value is considered valid.
    /// </remarks>
    /// <returns>A value indicating whether validation was successful.</returns>
    public bool IsValid(string? value)
    {
        if (value is null)
        {
            return true;
        }

        return GetCount(value) <= MaxLength;
    }

    private static int CountCrLfs(string value)
    {
        var count = 0;
        var index = 0;

        while ((index = value.IndexOf("\r\n", index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += 2;
        }

        return count;
    }
}
