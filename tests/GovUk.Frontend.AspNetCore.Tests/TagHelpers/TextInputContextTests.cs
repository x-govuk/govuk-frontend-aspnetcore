using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextInputContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var errorMessageTagName = TextInputTagHelper.ErrorMessageTagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, new AttributeCollection(), "Error", errorMessageTagName));

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
        var errorMessageTagName = TextInputTagHelper.ErrorMessageTagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, new AttributeCollection(), "Error", errorMessageTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{errorMessageTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var hintTagName = TextInputTagHelper.HintTagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint(new AttributeCollection(), "Error", hintTagName));

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
        var hintTagName = TextInputTagHelper.ErrorMessageTagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetHint(new AttributeCollection(), "Error", hintTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{hintTagName}> must be specified before <{suffixTagName}>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotPrefix_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TextInputContext();
        var prefixTagName = TextInputPrefixTagHelper.TagName;
        var labelTagName = TextInputTagHelper.LabelTagName;
        context.SetPrefix(new InputOptionsPrefix(), prefixTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, new AttributeCollection(), "Error", labelTagName));

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
        var labelTagName = TextInputTagHelper.LabelTagName;
        context.SetSuffix(new InputOptionsSuffix(), suffixTagName);

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, new AttributeCollection(), "Error", labelTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{labelTagName}> must be specified before <{suffixTagName}>.", ex.Message);
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
        Assert.Equal($"Only one <govuk-input-prefix> element is permitted within each <govuk-input>.", ex.Message);
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
        Assert.Equal($"Only one <govuk-input-suffix> element is permitted within each <govuk-input>.", ex.Message);
    }
}
