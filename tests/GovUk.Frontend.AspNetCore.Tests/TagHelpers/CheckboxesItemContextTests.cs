using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemContextTests
{
    [Fact]
    public void SetConditional_SetsConditionalOnContext()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        var conditionalOptions = new CheckboxesOptionsItemConditional { Html = new TemplateString("Conditional") };

        // Act
        context.SetConditional(conditionalOptions, "govuk-checkboxes-item-conditional");

        // Assert
        Assert.Equal("Conditional", context.Conditional?.Options.Html?.ToHtmlString());
    }

    [Fact]
    public void SetConditional_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        var existingConditionalOptions = new CheckboxesOptionsItemConditional { Html = new TemplateString("Existing conditional") };
        context.SetConditional(existingConditionalOptions, "govuk-checkboxes-item-conditional");

        // Act
        var conditionalOptions = new CheckboxesOptionsItemConditional { Html = new TemplateString("Conditional") };
        var ex = Record.Exception(() => context.SetConditional(conditionalOptions, "govuk-checkboxes-item-conditional"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-checkboxes-item-conditional> element is permitted within each <govuk-checkboxes-item>.", ex.Message);
    }

    [Fact]
    public void SetHint_SetsHintOnContext()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        var hintOptions = new HintOptions { Html = new TemplateString("Hint") };

        // Act
        context.SetHint(hintOptions, "govuk-checkboxes-item-hint");

        // Assert
        Assert.Equal("Hint", context.Hint?.Options.Html?.ToHtmlString());
    }

    [Fact]
    public void SetHint_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        var existingConditionalOptions = new CheckboxesOptionsItemConditional { Html = new TemplateString("Existing conditional") };
        context.SetConditional(existingConditionalOptions, "govuk-checkboxes-item-conditional");

        // Act
        var hintOptions = new HintOptions { Html = new TemplateString("Hint") };
        var ex = Record.Exception(() => context.SetHint(hintOptions, "govuk-checkboxes-item-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("<govuk-checkboxes-item-hint> must be specified before <govuk-checkboxes-item-conditional>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        var existingHintOptions = new HintOptions { Html = new TemplateString("Existing hint") };
        context.SetHint(existingHintOptions, "govuk-checkboxes-item-hint");

        // Act
        var hintOptions = new HintOptions { Html = new TemplateString("Hint") };
        var ex = Record.Exception(() => context.SetHint(hintOptions, "govuk-checkboxes-item-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-checkboxes-item-hint> element is permitted within each <govuk-checkboxes-item>.", ex.Message);
    }
}
