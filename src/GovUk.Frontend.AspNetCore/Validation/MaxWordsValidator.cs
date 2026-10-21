using System.Text.RegularExpressions;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Helper class to validate strings are no larger than a specified number of words.
/// </summary>
/// <remarks>
/// This counts words the same way as a GDS character count component whose limit is set with the deprecated
/// <c>max-words</c> attribute: as runs of characters separated by whitespace. When the limit is set with
/// <c>max-length</c> and a <c>count-type</c> of <see cref="CharacterCountType.Words"/>, use
/// <see cref="CharacterCountValidator"/>.
/// </remarks>
public class MaxWordsValidator
{
    // Must match the /\S+/g in https://github.com/alphagov/govuk-frontend/blob/v6.6.0/packages/govuk-frontend/src/govuk/components/character-count/character-count.mjs.
    // JavaScript's \s is a different set of characters to .NET's, so it's spelled out.
    private static readonly Regex _pattern = new(@"[^\t\n\v\f\r\p{Z}\uFEFF]+", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxWordsValidator"/> class.
    /// </summary>
    /// <param name="maxWords">The maximum allowable number of words</param>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="maxWords"/> argument is not a positive integer.</exception>
    public MaxWordsValidator(int maxWords)
    {
        if (maxWords <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWords), "maxWords must be a positive integer.");
        }

        MaxWords = maxWords;
    }

    /// <summary>
    /// Gets the maximum allowable number of words.
    /// </summary>
    public int MaxWords { get; }

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

        var wordCount = _pattern.Count(value);
        return wordCount <= MaxWords;
    }
}
