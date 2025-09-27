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

        var item = new ErrorSummaryContextItem(
            href,
            errorMessageHtml,
            [],
            []);

        // Act
        context.AddItem(item);

        // Assert
        Assert.Collection(
            context.Items,
            item =>
            {
                Assert.Equal(errorMessageHtml, item.Html);
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
        context.SetDescription([], descriptionHtml);

        // Assert
        Assert.Equal(descriptionHtml, context.Description?.Html);
    }

    [Fact]
    public void SetDescription_AlreadyGotDescription_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetDescription([], html: "Existing description");

        // Act
        var ex = Record.Exception(() => context.SetDescription([], html: "Description"));

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
        context.SetTitle([], titleHtml);

        // Assert
        Assert.Equal(titleHtml, context.Title?.Html);
    }

    [Fact]
    public void SetTitle_AlreadyGotTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetTitle([], html: "Existing title");

        // Act
        var ex = Record.Exception(() => context.SetTitle([], html: "Title"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-error-summary-title> element is permitted within each <govuk-error-summary>.", ex.Message);
    }
}
