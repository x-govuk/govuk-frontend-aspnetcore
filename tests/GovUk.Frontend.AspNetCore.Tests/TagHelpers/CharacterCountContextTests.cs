using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CharacterCountContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CharacterCountContext();

        context.SetValue("Value", CharacterCountValueTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, new AttributeCollection(), "Error", CharacterCountErrorMessageTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-character-count-error-message> must be specified before <govuk-character-count-value>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CharacterCountContext();

        context.SetValue("Value", CharacterCountValueTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetHint(new AttributeCollection(), "Error", CharacterCountHintTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-character-count-hint> must be specified before <govuk-character-count-value>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CharacterCountContext();

        context.SetValue("Value", CharacterCountValueTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, new AttributeCollection(), "Error", CharacterCountLabelTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-character-count-label> must be specified before <govuk-character-count-value>.", ex.Message);
    }

    [Fact]
    public void SetValue_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CharacterCountContext();

        context.SetValue("Existing value", CharacterCountValueTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetValue("Value", CharacterCountValueTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-character-count-value> element is permitted within each <govuk-character-count>.", ex.Message);
    }
}
