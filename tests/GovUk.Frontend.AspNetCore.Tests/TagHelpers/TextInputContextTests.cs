using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextInputContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotBeforeInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        var errorMessageTagName = TextInputErrorMessageTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var errorMessageTagName = TextInputErrorMessageTagHelper.TagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{prefixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotSuffix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        var errorMessageTagName = TextInputErrorMessageTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        var errorMessageTagName = TextInputErrorMessageTagHelper.TagName;
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
        var context = new TextInputContext();
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        var hintTagName = TextInputHintTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var hintTagName = TextInputHintTagHelper.TagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{prefixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotSuffix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        var hintTagName = TextInputErrorMessageTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        var hintTagName = TextInputHintTagHelper.TagName;
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
        var context = new TextInputContext();
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        var labelTagName = TextInputLabelTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var labelTagName = TextInputLabelTagHelper.TagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{prefixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotSuffix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        var labelTagName = TextInputLabelTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        var labelTagName = TextInputLabelTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{beforeInputTagName}> must be specified before <{prefixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotSuffix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{beforeInputTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
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
        var context = new TextInputContext();
        var beforeInputTagName = TextInputBeforeInputTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextInputBeforeInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextInputTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetPrefix_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrefix(new InputOptionsPrefix(), prefixTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextInputPrefixTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextInputTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetPrefix_AlreadyGotSuffix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrefix(new InputOptionsPrefix(), prefixTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{prefixTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetPrefix_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        var prefixTagName = TextInputSuffixTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrefix(new InputOptionsPrefix(), afterInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{afterInputTagName}> must be specified before <{prefixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetSuffix_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetSuffix(new InputOptionsSuffix(), suffixTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextInputSuffixTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextInputTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetSuffix_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputPrefixTagHelper.TagName;
        var suffixTagName = TextInputSuffixTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetSuffix(new InputOptionsSuffix(), afterInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{afterInputTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetAfterInput_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var afterInputTagName = TextInputAfterInputTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetAfterInput(new TemplateString("Content"), afterInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextInputAfterInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextInputTagHelper.TagName}>.",
            ex.Message);
    }
}
