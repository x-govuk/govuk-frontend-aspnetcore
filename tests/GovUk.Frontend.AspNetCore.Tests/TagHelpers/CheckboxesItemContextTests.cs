using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemContextTests
{
    [Fact]
    public void SetConditional_SetsConditionalOnContext()
    {
        // Arrange
        var context = new CheckboxesItemContext();

        // Act
        context.SetConditional([], content: new HtmlString("Conditional"));

        // Assert
        Assert.Equal("Conditional", context.Conditional?.Content?.ToString());
    }

    [Fact]
    public void SetConditional_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        context.SetConditional([], content: new HtmlString("Existing conditional"));

        // Act
        var ex = Record.Exception(() => context.SetConditional([], content: new HtmlString("Conditional")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-checkboxes-item-conditional> element is permitted within each <govuk-checkboxes-item>.", ex.Message);
    }

    [Fact]
    public void SetHint_SetsHintOnContext()
    {
        // Arrange
        var context = new CheckboxesItemContext();

        // Act
        context.SetHint([], content: new HtmlString("Hint"));

        // Assert
        Assert.Equal("Hint", context.Hint?.Content?.ToString());
    }

    [Fact]
    public void SetHint_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        context.SetConditional([], content: new HtmlString("Existing conditional"));

        // Act
        var ex = Record.Exception(() => context.SetHint([], content: new HtmlString("Hint")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("<govuk-checkboxes-item-hint> must be specified before <govuk-checkboxes-item-conditional>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesItemContext();
        context.SetHint([], content: new HtmlString("Existing hint"));

        // Act
        var ex = Record.Exception(() => context.SetHint([], content: new HtmlString("Hint")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-checkboxes-item-hint> element is permitted within each <govuk-checkboxes-item>.", ex.Message);
    }
}
