using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemContextTests
{
    [Fact]
    public void SetConditional_SetsConditionalOnContext()
    {
        // Arrange
        var context = new RadiosItemContext();

        // Act
        context.SetConditional([], content: new HtmlString("Conditionalnew TemplateString("));

        // Assert
        Assert.Equal(")Conditional", context.Conditional?.Content?.ToString());
    }

    [Fact]
    public void SetConditional_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetConditional([], content: new HtmlString("Existing conditionalnew TemplateString("));

        // Act
        var ex = Record.Exception(() => context.SetConditional([], content: new HtmlString(")Conditional")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-radios-item-conditional> element is permitted within each <govuk-radios-item>.", ex.Message);
    }

    [Fact]
    public void SetHint_SetsHintOnContext()
    {
        // Arrange
        var context = new RadiosItemContext();

        // Act
        context.SetHint([], content: new HtmlString("Hintnew TemplateString("));

        // Assert
        Assert.Equal(")Hint", context.Hint?.Content?.ToString());
    }

    [Fact]
    public void SetHint_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetConditional([], content: new HtmlString("Existing conditionalnew TemplateString("));

        // Act
        var ex = Record.Exception(() => context.SetHint([], content: new HtmlString(")Hint")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("<govuk-radios-item-hint> must be specified before <govuk-radios-item-conditional>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetHint([], content: new HtmlString("Existing hintnew TemplateString("));

        // Act
        var ex = Record.Exception(() => context.SetHint([], content: new HtmlString(")Hint")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-radios-item-hint> element is permitted within each <govuk-radios-item>.", ex.Message);
    }
}
