using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationContextTests
{
    /// <summary>
    /// The tag names for each child element, in both spellings; the children all have
    /// &lt;govuk-pagination&gt; for their parent, so both bind in the same place.
    /// </summary>
    public static TheoryData<string, string, string, string> TagNames { get; } = new()
    {
        {
            PaginationItemTagHelper.TagName,
            PaginationEllipsisItemTagHelper.TagName,
            PaginationPreviousTagHelper.TagName,
            PaginationNextTagHelper.TagName
        },
        {
            PaginationItemTagHelper.ShortTagName,
            PaginationEllipsisItemTagHelper.ShortTagName,
            PaginationPreviousTagHelper.ShortTagName,
            PaginationNextTagHelper.ShortTagName
        }
    };

    [Theory]
    [MemberData(nameof(TagNames))]
    public void AddItem_AlreadyGotNext_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), nextTagName);

        // Act
        var ex = Record.Exception(
            () => context.AddItem(new PaginationOptionsItem() { Ellipsis = true }, ellipsisTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{ellipsisTagName}> must be specified before <{nextTagName}>.", ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void AddItem_WithCurrentItemAndAlreadyGotCurrentItem_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            itemTagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("2"), Current = true },
            itemTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one current {itemTagName} is permitted.", ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void AddItem_ValidRequest_AddsItemToContext(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        var item = new PaginationOptionsItem() { Ellipsis = true };

        // Act
        context.AddItem(item, ellipsisTagName);

        // Assert
        Assert.Collection(context.Items, i => Assert.Same(item, i));
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetNext_AlreadyGotNext_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), nextTagName);

        // Act
        var ex = Record.Exception(() => context.SetNext(new PaginationOptionsNext(), nextTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PaginationNextTagHelper.TagName}> or <{PaginationNextTagHelper.ShortTagName}> " +
                $"element is permitted within each <{PaginationTagHelper.TagName}>.",
            ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetNext_ValidRequest_SetsNextOnContext(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        var next = new PaginationOptionsNext();

        // Act
        context.SetNext(next, nextTagName);

        // Assert
        Assert.Same(next, context.Next);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetPrevious_AlreadyGotNext_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.SetNext(new PaginationOptionsNext(), nextTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious(), previousTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{previousTagName}> must be specified before <{nextTagName}>.", ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetPrevious_AlreadyGotPrevious_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.SetPrevious(new PaginationOptionsPrevious(), previousTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious(), previousTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PaginationPreviousTagHelper.TagName}> or <{PaginationPreviousTagHelper.ShortTagName}> " +
                $"element is permitted within each <{PaginationTagHelper.TagName}>.",
            ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetPrevious_AlreadyGotItems_ThrowsInvalidOperationException(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        context.AddItem(
            new PaginationOptionsItem() { Number = new HtmlString("1"), Current = true },
            itemTagName);

        // Act
        var ex = Record.Exception(() => context.SetPrevious(new PaginationOptionsPrevious(), previousTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{previousTagName}> must be specified before <{itemTagName}>.", ex.Message);
    }

    [Theory]
    [MemberData(nameof(TagNames))]
    public void SetPrevious_ValidRequest_SetsPreviousOnContext(
        string itemTagName, string ellipsisTagName, string previousTagName, string nextTagName)
    {
        // Arrange
        var context = new PaginationContext();
        var previous = new PaginationOptionsPrevious();

        // Act
        context.SetPrevious(previous, previousTagName);

        // Assert
        Assert.Same(previous, context.Previous);
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
            $"<{PaginationItemTagHelper.ShortTagName}> cannot be used alongside " +
                $"<{PaginationPreviousTagHelper.TagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
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
            $"<{PaginationNextTagHelper.TagName}> cannot be used alongside " +
                $"<{PaginationItemTagHelper.ShortTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
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
            $"<{PaginationPreviousTagHelper.TagName}> cannot be used alongside " +
                $"<{PaginationNextTagHelper.ShortTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
