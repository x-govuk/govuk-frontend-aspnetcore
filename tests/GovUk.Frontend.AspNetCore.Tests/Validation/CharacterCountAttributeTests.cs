using System.ComponentModel.DataAnnotations;
using GovUk.Frontend.AspNetCore.Validation;

namespace GovUk.Frontend.AspNetCore.Tests.Validation;

public class CharacterCountAttributeTests
{
    [Theory]
    [InlineData("abc", true)]
    [InlineData("a\r\nb", true)]
    [InlineData("abcd", false)]
    public void Length_ValidatesAsExpected(string input, bool expectedValid) =>
        AssertValidation(new LengthModel { TheString = input }, expectedValid, "TheString must be 3 characters or fewer");

    [Theory]
    [InlineData("ééé", true)]
    [InlineData("abcd", false)]
    public void Characters_ValidatesAsExpected(string input, bool expectedValid) =>
        AssertValidation(new CharactersModel { TheString = input }, expectedValid, "TheString must be 3 characters or fewer");

    [Theory]
    [InlineData("first, second - third!", true)]
    [InlineData("first second-third-fourth", false)]
    public void Words_ValidatesAsExpected(string input, bool expectedValid) =>
        AssertValidation(new WordsModel { TheString = input }, expectedValid, "TheString must be 3 words or fewer");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void NullOrEmptyInput_PassesValidation(string? input) =>
        AssertValidation(new LengthModel { TheString = input }, expectedValid: true, expectedErrorMessage: null);

    [Fact]
    public void NullInputWithRequired_ReportsRequiredError() =>
        AssertValidation(new RequiredModel { TheString = null }, expectedValid: false, "Enter TheString");

    private static void AssertValidation(object model, bool expectedValid, string? expectedErrorMessage)
    {
        // Arrange
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        // Assert
        Assert.Equal(expectedValid, isValid);

        if (expectedValid)
        {
            Assert.Empty(validationResults);
        }
        else
        {
            Assert.Collection(validationResults, result => Assert.Equal(expectedErrorMessage, result.ErrorMessage));
        }
    }

    public class LengthModel
    {
        [CharacterCount(3, ErrorMessage = "TheString must be 3 characters or fewer")]
        public string? TheString { get; set; }
    }

    public class CharactersModel
    {
        [CharacterCount(3, CountType = CharacterCountType.Characters, ErrorMessage = "TheString must be 3 characters or fewer")]
        public string? TheString { get; set; }
    }

    public class WordsModel
    {
        [CharacterCount(3, CountType = CharacterCountType.Words, ErrorMessage = "TheString must be 3 words or fewer")]
        public string? TheString { get; set; }
    }

    public class RequiredModel
    {
        [Required(ErrorMessage = "Enter TheString")]
        [CharacterCount(3, ErrorMessage = "TheString must be 3 characters or fewer")]
        public string? TheString { get; set; }
    }
}
