using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemContextTests
{
    [Fact]
    public void SetConditional_SetsConditionalOnContext()
    {
        // Arrange
        var context = new RadiosItemContext();

        // Act
        context.SetConditional(new RadiosOptionsItemConditional { Html = new TemplateString("Conditional") }, tagName: "govuk-radios-item-conditional");

        // Assert
        Assert.Equal("Conditional", context.Conditional?.Options.Html?.ToHtmlString());
    }

    [Fact]
    public void SetConditional_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetConditional(new RadiosOptionsItemConditional { Html = new TemplateString("Existing conditional") }, tagName: "govuk-radios-item-conditional");

        // Act
        var ex = Record.Exception(() => context.SetConditional(new RadiosOptionsItemConditional { Html = new TemplateString("Conditional") }, tagName: "govuk-radios-item-conditional"));

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
        context.SetHint(new HintOptions { Html = new TemplateString("Hint") }, tagName: "govuk-radios-item-hint");

        // Assert
        Assert.Equal("Hint", context.Hint?.Options.Html?.ToHtmlString());
    }

    [Fact]
    public void SetHint_AlreadyGotConditional_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetConditional(new RadiosOptionsItemConditional { Html = new TemplateString("Existing conditional") }, tagName: "govuk-radios-item-conditional");

        // Act
        var ex = Record.Exception(() => context.SetHint(new HintOptions { Html = new TemplateString("Hint") }, tagName: "govuk-radios-item-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("<govuk-radios-item-hint> must be specified before <govuk-radios-item-conditional>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new RadiosItemContext();
        context.SetHint(new HintOptions { Html = new TemplateString("Existing hint") }, tagName: "govuk-radios-item-hint");

        // Act
        var ex = Record.Exception(() => context.SetHint(new HintOptions { Html = new TemplateString("Hint") }, tagName: "govuk-radios-item-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-radios-item-hint> element is permitted within each <govuk-radios-item>.", ex.Message);
    }
}
