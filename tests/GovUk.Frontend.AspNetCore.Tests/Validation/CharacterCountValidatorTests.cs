using GovUk.Frontend.AspNetCore.Validation;

namespace GovUk.Frontend.AspNetCore.Tests.Validation;

public class CharacterCountValidatorTests
{
    // Expected counts are from the browser: value.length, and the number of segments (or word-like segments) from
    // Intl.Segmenter with granularity "grapheme" (or "word"), and the number of matches of /\S+/g.
    public static TheoryData<string, int, int, int, int> Counts { get; } = new()
    {
        { "", 0, 0, 0, 0 },
        { "hello world", 11, 11, 2, 2 },
        { "  leading and trailing  ", 24, 24, 3, 3 },
        { "well-known", 10, 10, 2, 1 },
        { "don't stop", 10, 10, 2, 2 },
        { "don’t", 5, 5, 1, 1 },
        { "3.14 1,000 1;2", 14, 14, 3, 3 },
        { "a:b", 3, 3, 1, 1 },
        { "___ a_b _x", 10, 10, 3, 3 },
        { "foo.bar e.g. U.S.A.", 19, 19, 3, 3 },
        { "カタカナ テスト", 8, 8, 2, 2 },
        { "emoji 😀 👍🏽 ok", 16, 12, 2, 4 },
        { "צה\"ל", 4, 4, 1, 1 },
        { "a­b", 3, 3, 1, 1 },
        { "a​b", 3, 3, 2, 1 },
        { "abc123 123abc", 13, 13, 2, 2 },
        { "a b", 3, 3, 1, 2 },
        { "...", 3, 3, 0, 1 },
        { "x · y a·b", 9, 9, 3, 4 },
        { "café naïve", 10, 10, 2, 2 },
        { "été", 5, 3, 1, 1 },
        { "́a", 2, 2, 1, 1 },
        { "1.2.3", 5, 5, 1, 1 },
        { "a.1", 3, 3, 2, 1 },
        { "1.a", 3, 3, 2, 1 },
        { "Hello, world! How are you?", 26, 26, 5, 5 },
        { "first\nsecond\nthird", 18, 18, 3, 3 },
        { "🇬🇧🇫🇷", 8, 2, 0, 1 },
        { "👨‍👩‍👧", 8, 1, 0, 1 },
        { "tab\tseparated", 13, 13, 2, 2 },
        { "a--b", 4, 4, 2, 1 },
        { "a - b", 5, 5, 2, 3 },
        { "x\u0085y", 3, 3, 2, 1 },
        { "x﻿y", 3, 3, 1, 2 },
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ctor_InvalidMaxLength_ThrowsArgumentOutOfRangeException(int maxLength)
    {
        // Arrange

        // Act
        var ex = Record.Exception(() => new CharacterCountValidator(maxLength));

        // Assert
        var argException = Assert.IsType<ArgumentOutOfRangeException>(ex);
        Assert.Equal("maxLength", argException.ParamName);
    }

    [Fact]
    public void Ctor_InvalidCountType_ThrowsArgumentOutOfRangeException()
    {
        // Arrange

        // Act
        var ex = Record.Exception(() => new CharacterCountValidator(10, (CharacterCountType)42));

        // Assert
        var argException = Assert.IsType<ArgumentOutOfRangeException>(ex);
        Assert.Equal("countType", argException.ParamName);
    }

    [Theory]
    [MemberData(nameof(Counts))]
    public void GetCount_Length_ReturnsSameCountAsBrowser(string value, int expectedLength, int expectedCharacters, int expectedWords, int expectedMaxWords)
    {
        // Arrange
        _ = (expectedCharacters, expectedWords, expectedMaxWords);
        var validator = new CharacterCountValidator(10, CharacterCountType.Length);

        // Act
        var result = validator.GetCount(value);

        // Assert
        Assert.Equal(expectedLength, result);
    }

    [Theory]
    [MemberData(nameof(Counts))]
    public void GetCount_Characters_ReturnsSameCountAsBrowser(string value, int expectedLength, int expectedCharacters, int expectedWords, int expectedMaxWords)
    {
        // Arrange
        _ = (expectedLength, expectedWords, expectedMaxWords);
        var validator = new CharacterCountValidator(10, CharacterCountType.Characters);

        // Act
        var result = validator.GetCount(value);

        // Assert
        Assert.Equal(expectedCharacters, result);
    }

    [Theory]
    [MemberData(nameof(Counts))]
    public void GetCount_Words_ReturnsSameCountAsBrowser(string value, int expectedLength, int expectedCharacters, int expectedWords, int expectedMaxWords)
    {
        // Arrange
        _ = (expectedLength, expectedCharacters, expectedMaxWords);
        var validator = new CharacterCountValidator(10, CharacterCountType.Words);

        // Act
        var result = validator.GetCount(value);

        // Assert
        Assert.Equal(expectedWords, result);
    }

    [Theory]
    [InlineData(CharacterCountType.Length, 18)]
    [InlineData(CharacterCountType.Characters, 18)]
    [InlineData(CharacterCountType.Words, 3)]
    public void GetCount_SubmittedLineBreaks_CountsLineBreaksAsTheBrowserDoes(CharacterCountType countType, int expectedCount)
    {
        // Arrange
        var validator = new CharacterCountValidator(10, countType);

        // Act
        var result = validator.GetCount("first\r\nsecond\r\nthird");

        // Assert
        Assert.Equal(expectedCount, result);
    }

    [Theory]
    [InlineData("日本語のテキスト", 3)]
    [InlineData("ภาษาไทยง่ายนิดเดียว", 5)]
    public void GetCount_WordsInLanguageWithoutSpaces_DoesNotReturnMoreThanBrowser(string value, int browserCount)
    {
        // Arrange
        var validator = new CharacterCountValidator(10, CharacterCountType.Words);

        // Act
        var result = validator.GetCount(value);

        // Assert
        Assert.InRange(result, 1, browserCount);
    }

    [Theory]
    [InlineData(CharacterCountType.Length)]
    [InlineData(CharacterCountType.Characters)]
    [InlineData(CharacterCountType.Words)]
    public void IsValid_NullInput_ReturnsTrue(CharacterCountType countType)
    {
        // Arrange
        var validator = new CharacterCountValidator(3, countType);

        // Act
        var result = validator.IsValid(value: null);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(CharacterCountType.Length, "abc", true)]
    [InlineData(CharacterCountType.Length, "abcd", false)]
    [InlineData(CharacterCountType.Length, "a\r\nb", true)]
    [InlineData(CharacterCountType.Length, "ée", true)]
    [InlineData(CharacterCountType.Length, "éé", false)]
    [InlineData(CharacterCountType.Characters, "ééé", true)]
    [InlineData(CharacterCountType.Characters, "abcd", false)]
    [InlineData(CharacterCountType.Words, "first second third", true)]
    [InlineData(CharacterCountType.Words, "first, second - third!", true)]
    [InlineData(CharacterCountType.Words, "first second third fourth", false)]
    [InlineData(CharacterCountType.Words, "first second-third", true)]
    [InlineData(CharacterCountType.Words, "first second-third-fourth", false)]
    public void IsValid_ReturnsExpectedResult(CharacterCountType countType, string value, bool expectedResult)
    {
        // Arrange
        var validator = new CharacterCountValidator(3, countType);

        // Act
        var result = validator.IsValid(value);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
