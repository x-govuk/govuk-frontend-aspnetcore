using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextAreaContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotBeforeInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
        var errorMessageTagName = TextAreaErrorMessageTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var valueTagName = TextAreaValueTagHelper.TagName;
        var errorMessageTagName = TextAreaErrorMessageTagHelper.TagName;
        context.SetValue(new TemplateString("Value"), valueTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new TemplateString("Error"), errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{valueTagName}>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        var errorMessageTagName = TextAreaErrorMessageTagHelper.TagName;
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
        var context = new TextAreaContext();
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
        var hintTagName = TextAreaHintTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var valueTagName = TextAreaValueTagHelper.TagName;
        var hintTagName = TextAreaHintTagHelper.TagName;
        context.SetValue(new TemplateString("Value"), valueTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint([], new TemplateString("Error"), hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{valueTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        var hintTagName = TextAreaHintTagHelper.TagName;
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
        var context = new TextAreaContext();
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
        var labelTagName = TextAreaLabelTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{beforeInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var valueTagName = TextAreaValueTagHelper.TagName;
        var labelTagName = TextAreaLabelTagHelper.TagName;
        context.SetValue(new TemplateString("Value"), valueTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{valueTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        var labelTagName = TextAreaLabelTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new TemplateString("Error"), labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var valueTagName = TextAreaValueTagHelper.TagName;
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
        context.SetValue(new TemplateString("Value"), valueTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{beforeInputTagName}> must be specified before <{valueTagName}>.", ex.Message);
    }

    [Fact]
    public void SetBeforeInput_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
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
        var context = new TextAreaContext();
        var beforeInputTagName = TextAreaBeforeInputTagHelper.TagName;
        context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetBeforeInput(new TemplateString("Content"), beforeInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextAreaBeforeInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextAreaTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetAfterInput_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetAfterInput(new TemplateString("Content"), afterInputTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {TestUtils.GetAllTagNameElementsMessage(TextAreaAfterInputTagHelper.AllTagNames, "or")} element" +
                $" is permitted within each <{TextAreaTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetValue_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var valueTagName = TextAreaValueTagHelper.TagName;
        context.SetValue(new TemplateString("Existing value"), valueTagName);

        // Act
        var ex = Record.Exception(() => context.SetValue(new TemplateString("Value"), valueTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{valueTagName}> element is permitted within each <{TextAreaTagHelper.TagName}>.",
            ex.Message);
    }

    [Fact]
    public void SetValue_AlreadyGotAfterInput_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextAreaContext();
        var afterInputTagName = TextAreaAfterInputTagHelper.TagName;
        var valueTagName = TextAreaValueTagHelper.TagName;
        context.SetAfterInput(new TemplateString("Content"), afterInputTagName);

        // Act
        var ex = Record.Exception(() => context.SetValue(new TemplateString("Value"), valueTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{valueTagName}> must be specified before <{afterInputTagName}>.", ex.Message);
    }
}
