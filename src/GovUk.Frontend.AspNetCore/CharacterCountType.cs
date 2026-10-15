namespace GovUk.Frontend.AspNetCore;

/// <summary>
/// Represents how a GDS character count component counts the text entered.
/// </summary>
public enum CharacterCountType
{
    /// <summary>
    /// Counts the length of the text, as JavaScript's <c>String.length</c> does.
    /// </summary>
    /// <remarks>
    /// This counts UTF-16 code units so, for example, an emoji may count as more than one.
    /// </remarks>
    Length = 0,

    /// <summary>
    /// Counts the characters a user would perceive (grapheme clusters).
    /// </summary>
    /// <remarks>
    /// Requires a browser that supports <c>Intl.Segmenter</c>.
    /// </remarks>
    Characters = 1,

    /// <summary>
    /// Counts words.
    /// </summary>
    /// <remarks>
    /// Requires a browser that supports <c>Intl.Segmenter</c>, unless the limit is specified with the
    /// deprecated <c>max-words</c> attribute.
    /// </remarks>
    Words = 2
}
