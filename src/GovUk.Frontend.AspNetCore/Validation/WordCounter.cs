using System.Globalization;
using System.Text;

namespace GovUk.Frontend.AspNetCore.Validation;

/// <summary>
/// Counts words the way <c>Intl.Segmenter</c> does with <c>granularity: "word"</c>, counting the segments it
/// reports as <c>isWordLike</c>.
/// </summary>
/// <remarks>
/// <para>This implements the word boundary rules from
/// <see href="https://www.unicode.org/reports/tr29/#Word_Boundary_Rules">Unicode Standard Annex #29</see>,
/// which browsers' segmenters are based on. Words are runs of letters and numbers; whitespace, punctuation and
/// symbols (including emoji) aren't words. Punctuation within a word, such as the apostrophe in "don't" or the
/// decimal point in "3.14", doesn't split it, but a hyphen does.</para>
/// <para>Where browsers differ, this counts as the browser that finds the fewest words, so text a browser showed as
/// within the limit passes validation:</para>
/// <list type="bullet">
/// <item>Recent versions of Chromium split words at a full stop or colon between letters, so "e.g." is two words
/// there but one here, as it is in other browsers.</item>
/// <item>Browsers find the words in languages written without spaces between them, such as Chinese, Japanese or
/// Thai, using a dictionary. That isn't replicated here; a run of such characters is counted as a single word.</item>
/// </list>
/// </remarks>
internal static class WordCounter
{
    private enum WordBreakClass
    {
        Other,
        ALetter,
        HebrewLetter,
        Numeric,
        Katakana,
        ExtendNumLet,
        MidLetter,
        MidNum,
        MidNumLet,
        SingleQuote,
        DoubleQuote,
        Extend,
        Dictionary
    }

    public static int Count(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // Extend and Format characters belong to the character before them (WB4), so they're dropped here,
        // unless there's nothing before them to attach to.
        var classes = new List<WordBreakClass>(value.Length);
        foreach (var rune in value.EnumerateRunes())
        {
            var wordBreakClass = GetWordBreakClass(rune);

            if (wordBreakClass == WordBreakClass.Extend)
            {
                if (classes.Count == 0)
                {
                    classes.Add(WordBreakClass.Other);
                }

                continue;
            }

            classes.Add(wordBreakClass);
        }

        var count = 0;
        var i = 0;

        while (i < classes.Count)
        {
            var current = classes[i];

            if (current == WordBreakClass.Dictionary)
            {
                count++;

                while (i < classes.Count && classes[i] == WordBreakClass.Dictionary)
                {
                    i++;
                }

                continue;
            }

            if (!IsWordCharacter(current))
            {
                i++;
                continue;
            }

            count++;
            i++;

            while (i < classes.Count)
            {
                var next = classes[i];

                if (Joins(current, next))
                {
                    current = next;
                    i++;
                    continue;
                }

                if (i + 1 < classes.Count && JoinsAcross(current, next, classes[i + 1]))
                {
                    current = classes[i + 1];
                    i += 2;
                    continue;
                }

                break;
            }
        }

        return count;
    }

    private static bool IsAHLetter(WordBreakClass c) => c is WordBreakClass.ALetter or WordBreakClass.HebrewLetter;

    private static bool IsWordCharacter(WordBreakClass c) =>
        IsAHLetter(c) || c is WordBreakClass.Numeric or WordBreakClass.Katakana or WordBreakClass.ExtendNumLet;

    // WB5, WB8, WB9, WB10, WB13, WB13a, WB13b
    private static bool Joins(WordBreakClass previous, WordBreakClass next) =>
        (IsAHLetter(previous) || previous == WordBreakClass.Numeric) && (IsAHLetter(next) || next == WordBreakClass.Numeric) ||
        previous == WordBreakClass.Katakana && next == WordBreakClass.Katakana ||
        IsWordCharacter(previous) && next == WordBreakClass.ExtendNumLet ||
        previous == WordBreakClass.ExtendNumLet && IsWordCharacter(next);

    // WB6, WB7, WB7b, WB7c, WB11, WB12
    private static bool JoinsAcross(WordBreakClass previous, WordBreakClass middle, WordBreakClass next)
    {
        var isMidNumLetQ = middle is WordBreakClass.MidNumLet or WordBreakClass.SingleQuote;

        return
            IsAHLetter(previous) && IsAHLetter(next) && (middle == WordBreakClass.MidLetter || isMidNumLetQ) ||
            previous == WordBreakClass.HebrewLetter && next == WordBreakClass.HebrewLetter && middle == WordBreakClass.DoubleQuote ||
            previous == WordBreakClass.Numeric && next == WordBreakClass.Numeric && (middle == WordBreakClass.MidNum || isMidNumLetQ);
    }

    private static WordBreakClass GetWordBreakClass(Rune rune)
    {
        var value = rune.Value;

        switch (value)
        {
            case '\'':
                return WordBreakClass.SingleQuote;
            case '"':
                return WordBreakClass.DoubleQuote;
            case '.' or '‘' or '’' or '․' or '﹒' or '＇' or '．':
                return WordBreakClass.MidNumLet;
            case ':' or '·' or '·' or '՟' or '״' or '‧' or '︓' or '﹕' or '：':
                return WordBreakClass.MidLetter;
            case ',' or ';' or ';' or '։' or '،' or '؍' or '٬' or '߸' or '⁄'
                or '︐' or '︔' or '﹐' or '﹔' or '，' or '；':
                return WordBreakClass.MidNum;
            case ' ':
                return WordBreakClass.ExtendNumLet;
            case '‍':
                return WordBreakClass.Extend;
            case '​':
                return WordBreakClass.Other;
        }

        if (value is (>= 0x05D0 and <= 0x05EA) or (>= 0x05EF and <= 0x05F2) or 0xFB1D or (>= 0xFB1F and <= 0xFB28)
            or (>= 0xFB2A and <= 0xFB4F))
        {
            return WordBreakClass.HebrewLetter;
        }

        if (value is (>= 0x30A0 and <= 0x30FF) or (>= 0x31F0 and <= 0x31FF) or (>= 0x32D0 and <= 0x32FE)
            or (>= 0x3300 and <= 0x3357) or (>= 0xFF66 and <= 0xFF9F) or (>= 0x3031 and <= 0x3035) or 0x309B or 0x309C)
        {
            return WordBreakClass.Katakana;
        }

        if (IsDictionaryScript(value))
        {
            return WordBreakClass.Dictionary;
        }

        return Rune.GetUnicodeCategory(rune) switch
        {
            UnicodeCategory.UppercaseLetter or
            UnicodeCategory.LowercaseLetter or
            UnicodeCategory.TitlecaseLetter or
            UnicodeCategory.ModifierLetter or
            UnicodeCategory.OtherLetter or
            UnicodeCategory.LetterNumber => WordBreakClass.ALetter,
            UnicodeCategory.DecimalDigitNumber => WordBreakClass.Numeric,
            UnicodeCategory.ConnectorPunctuation => WordBreakClass.ExtendNumLet,
            UnicodeCategory.NonSpacingMark or
            UnicodeCategory.SpacingCombiningMark or
            UnicodeCategory.EnclosingMark or
            UnicodeCategory.Format => WordBreakClass.Extend,
            _ => WordBreakClass.Other
        };
    }

    // Scripts that browsers segment into words with a dictionary.
    private static bool IsDictionaryScript(int value) =>
        value is
            (>= 0x0E00 and <= 0x0EFF) or // Thai, Lao
            (>= 0x1000 and <= 0x109F) or // Myanmar
            (>= 0x1780 and <= 0x17FF) or // Khmer
            (>= 0x19E0 and <= 0x19FF) or // Khmer symbols
            (>= 0xAA60 and <= 0xAA7F) or // Myanmar extended-A
            (>= 0xA9E0 and <= 0xA9FF) or // Myanmar extended-B
            (>= 0x3040 and <= 0x309A) or (>= 0x309D and <= 0x309F) or // Hiragana
            (>= 0x3400 and <= 0x4DBF) or // CJK unified ideographs extension A
            (>= 0x4E00 and <= 0x9FFF) or // CJK unified ideographs
            (>= 0xF900 and <= 0xFAFF) or // CJK compatibility ideographs
            (>= 0x20000 and <= 0x3FFFF); // CJK unified ideographs extensions B onwards
}
