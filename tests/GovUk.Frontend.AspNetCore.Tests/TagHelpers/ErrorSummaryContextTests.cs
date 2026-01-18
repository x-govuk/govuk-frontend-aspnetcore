using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ErrorSummaryContextTests
{
    [Fact]
    public void AddItem_AddsItemToItems()
    {
        // Arrange
        var errorMessageHtml = "Error message";
        var href = "#TheField";

        var context = new ErrorSummaryContext();

        var item = new ErrorSummaryContextItem(new TemplateString(href), new TemplateString(errorMessageHtml),
            [],
            []);

        // Act
        context.AddItem(item);

        // Assert
        Assert.Collection(
            context.Items,
            item =>
            {
                Assert.Equal(new TemplateString(errorMessageHtml), item.Html);
                Assert.Equal(href, item.Href);
            });
    }

    [Fact]
    public void SetDescription_SetsDescriptionOnContext()
    {
        // Arrange
        var descriptionHtml = "Description";

        var context = new ErrorSummaryContext();

        // Act
        context.SetDescription([], new TemplateString(descriptionHtml));

        // Assert
        Assert.Equal(new TemplateString(descriptionHtml), context.Description?.Html);
    }

    [Fact]
    public void SetDescription_AlreadyGotDescription_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetDescription([], html: new TemplateString("Existing description"));

        // Act
        var ex = Record.Exception(() => context.SetDescription([], html: new TemplateString("Description")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-error-summary-description> element is permitted within each <govuk-error-summary>.", ex.Message);
    }

    [Fact]
    public void SetTitle_SetsTitleOnContext()
    {
        // Arrange
        var titleHtml = "Title";

        var context = new ErrorSummaryContext();

        // Act
        context.SetTitle([], new TemplateString(titleHtml));

        // Assert
        Assert.Equal(new TemplateString(titleHtml), context.Title?.Html);
    }

    [Fact]
    public void SetTitle_AlreadyGotTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetTitle([], html: new TemplateString("Existing title"));

        // Act
        var ex = Record.Exception(() => context.SetTitle([], html: new TemplateString("Title")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-error-summary-title> element is permitted within each <govuk-error-summary>.", ex.Message);
    }
}
