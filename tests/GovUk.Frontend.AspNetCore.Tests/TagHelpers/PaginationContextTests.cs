using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationContextTests
{
    [Fact]
    public void AddItem_AlreadyGotNext_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext());

        // Act
        var ex = Record.Exception(() => context.AddItem(new PaginationOptionsItem() { Ellipsis = true }));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-ellipsis> must be specified before <govuk-pagination-next>.", ex.Message);
    }

    [Fact]
    public void AddItem_WithCurrentItemAndAlreadyGotCurrentItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(new PaginationOptionsItem()
        {
            Number = new HtmlString("1"),
            Current = true
        });

        // Act
        var ex = Record.Exception(() => context.AddItem(new PaginationOptionsItem()
        {
            Number = new HtmlString("2"),
            Current = true
        }));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one current govuk-pagination-item is permitted.", ex.Message);
    }

    [Fact]
    public void AddItem_ValidRequest_AddsItemToContext()
    {
        // Arrange
        var context = new PaginationContext();
        var item = new PaginationOptionsItem() { Ellipsis = true };

        // Act
        context.AddItem(item);

        // Assert
        Assert.Collection(context.Items, i => Assert.Same(item, i));
    }

    [Fact]
    public void SetNext_AlreadyGotNext_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext());

        // Act
        var ex = Record.Exception(() => context.SetNext(new PaginationOptionsNext()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-pagination-next> element is permitted within each <govuk-pagination>.", ex.Message);
    }

    [Fact]
    public void SetNext_ValidRequest_SetsNextOnContext()
    {
        // Arrange
        var context = new PaginationContext();
        var next = new PaginationOptionsNext();

        // Act
        context.SetNext(next);

        // Assert
        Assert.Same(next, context.Next);
    }

    [Fact]
    public void SetPrevious_AlreadyGotNext_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext());

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-previous> must be specified before <govuk-pagination-next>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotPrevious_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetPrevious(new PaginationOptionsPrevious());

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-pagination-previous> element is permitted within each <govuk-pagination>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(new PaginationOptionsItem()
        {
            Number = new HtmlString("1"),
            Current = true
        });

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-previous> must be specified before <govuk-pagination-item>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_ValidRequest_SetsPreviousOnContext()
    {
        // Arrange
        var context = new PaginationContext();
        var previous = new PaginationOptionsPrevious();

        // Act
        context.SetPrevious(previous);

        // Assert
        Assert.Same(previous, context.Previous);
    }
}
