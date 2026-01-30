using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectContextTests
{
    [Fact]
    public void SetErrorMessage_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new SelectContext(aspFor: null);

        context.AddItem(new SelectOptionsItem()
        {
            Text = "Option"
        });

        // Act
        var ex = Record.Exception(() => context.SetErrorMessage(null, [], new HtmlString("Error"), SelectErrorMessageTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-select-error-message> must be specified before <govuk-select-item>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new SelectContext(aspFor: null);

        context.AddItem(new SelectOptionsItem()
        {
            Text = "Option"
        });

        // Act
        var ex = Record.Exception(() => context.SetHint([], new HtmlString("Error"), SelectHintTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-select-hint> must be specified before <govuk-select-item>.", ex.Message);
    }

    [Fact]
    public void SetLabel_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new SelectContext(aspFor: null);

        context.AddItem(new SelectOptionsItem()
        {
            Text = "Option"
        });

        // Act
        var ex = Record.Exception(() => context.SetLabel(false, [], new HtmlString("Error"), SelectLabelTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-select-label> must be specified before <govuk-select-item>.", ex.Message);
    }
}
