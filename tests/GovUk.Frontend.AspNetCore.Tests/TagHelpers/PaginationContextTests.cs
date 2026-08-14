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
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Ellipsis = true },
            PaginationEllipsisItemTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-ellipsis> must be specified before <govuk-pagination-next>.", ex.Message);
    }

    [Fact]
    public void AddItem_AlreadyGotNextAddedWithShortTagName_ThrowsInvalidOperationExceptionNamingTheShortTagNames()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Ellipsis = true },
            PaginationEllipsisItemTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<ellipsis> must be specified before <next>.", ex.Message);
    }

    [Fact]
    public void AddItem_WithCurrentItemAndAlreadyGotCurrentItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            PaginationItemTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("2"), Current = true },
            PaginationItemTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one current govuk-pagination-item is permitted.", ex.Message);
    }

    [Fact]
    public void AddItem_WithCurrentItemAddedWithShortTagName_ThrowsInvalidOperationExceptionNamingTheShortTagName()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            PaginationItemTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("2"), Current = true },
            PaginationItemTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one current pagination-item is permitted.", ex.Message);
    }

    [Fact]
    public void AddItem_ValidRequest_AddsItemToContext()
    {
        // Arrange
        var context = new PaginationContext();
        var item = new PaginationOptionsItem() { Ellipsis = true };

        // Act
        context.AddItem(item, PaginationEllipsisItemTagHelper.TagName);

        // Assert
        Assert.Collection(context.Items, i => Assert.Same(item, i));
    }

    [Fact]
    public void AddItem_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1") },
            PaginationItemTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<pagination-item> cannot be used alongside <govuk-pagination-previous>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

    [Fact]
    public void SetNext_AlreadyGotNext_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.TagName);

        // Act
        var ex = Record.Exception(
            () => context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "Only one <govuk-pagination-next> or <next> element is permitted within each <govuk-pagination>.",
            ex.Message);
    }

    [Fact]
    public void SetNext_ValidRequest_SetsNextOnContext()
    {
        // Arrange
        var context = new PaginationContext();
        var next = new PaginationOptionsNext();

        // Act
        context.SetNext(next, PaginationNextTagHelper.TagName);

        // Assert
        Assert.Same(next, context.Next);
    }

    [Fact]
    public void SetNext_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1") },
            PaginationItemTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<govuk-pagination-next> cannot be used alongside <pagination-item>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotNext_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.TagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-previous> must be specified before <govuk-pagination-next>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotNextAddedWithShortTagName_ThrowsInvalidOperationExceptionNamingTheShortTagNames()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<previous> must be specified before <next>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotPrevious_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "Only one <govuk-pagination-previous> or <previous> element is permitted within each <govuk-pagination>.",
            ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            PaginationItemTagHelper.TagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-pagination-previous> must be specified before <govuk-pagination-item>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_AlreadyGotItemsAddedWithShortTagName_ThrowsInvalidOperationExceptionNamingTheShortTagNames()
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            PaginationItemTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<previous> must be specified before <pagination-item>.", ex.Message);
    }

    [Fact]
    public void SetPrevious_ValidRequest_SetsPreviousOnContext()
    {
        // Arrange
        var context = new PaginationContext();
        var previous = new PaginationOptionsPrevious();

        // Act
        context.SetPrevious(previous, PaginationPreviousTagHelper.TagName);

        // Assert
        Assert.Same(previous, context.Previous);
    }

    [Fact]
    public void SetPrevious_SiblingUsesOtherTagNameSpelling_ThrowsBeforeTheOrderingChecks()
    {
        // Arrange
        // The previous element is also out of order here; reordering does not fix the spelling, so
        // the mismatch is what gets reported
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), PaginationNextTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetPrevious(new PaginationOptionsPrevious(), PaginationPreviousTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<govuk-pagination-previous> cannot be used alongside <next>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
