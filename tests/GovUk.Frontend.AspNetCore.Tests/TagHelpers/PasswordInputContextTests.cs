using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PasswordInputContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotBeforeInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var beforeInputTagName = PasswordInputBeforeInputTagHelper.TagName;
        var errorMessageTagName = PasswordInputErrorMessageTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var afterInputTagName = PasswordInputAfterInputTagHelper.TagName;
        var errorMessageTagName = PasswordInputErrorMessageTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotBeforeInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var beforeInputTagName = PasswordInputBeforeInputTagHelper.TagName;
        var hintTagName = PasswordInputHintTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var afterInputTagName = PasswordInputAfterInputTagHelper.TagName;
        var hintTagName = PasswordInputHintTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotBeforeInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var beforeInputTagName = PasswordInputBeforeInputTagHelper.TagName;
        var labelTagName = PasswordInputLabelTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var afterInputTagName = PasswordInputAfterInputTagHelper.TagName;
        var labelTagName = PasswordInputLabelTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var afterInputTagName = PasswordInputAfterInputTagHelper.TagName;
        var beforeInputTagName = PasswordInputBeforeInputTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{beforeInputTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var beforeInputTagName = PasswordInputBeforeInputTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(PasswordInputBeforeInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{PasswordInputTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetAfterInput_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PasswordInputContext();
        var afterInputTagName = PasswordInputAfterInputTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetAfterInput(new TemplateString("Content"), afterInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(PasswordInputAfterInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{PasswordInputTagHelper.TagName}>.",
            ex.Message);
    }
}
