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

        var item = new ErrorSummaryContextItem(
            href,
            new TemplateString(errorMessageHtml),
            [],
            []);

        // Act
        context.AddItem(item, ErrorSummaryItemTagHelper.TagName);

        // Assert
        Assert.Collection(
            context.Items,
            item =>
            {
                Assert.Equal(errorMessageHtml, item.Html.ToHtmlString());
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
        context.SetDescription([], new TemplateString(descriptionHtml), ErrorSummaryDescriptionTagHelper.TagName);

        // Assert
        Assert.Equal(descriptionHtml, context.Description?.Html.ToHtmlString());
    }

    [Fact]
    public void SetDescription_AlreadyGotDescription_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetDescription([], html: new TemplateString("Existing description"), ErrorSummaryDescriptionTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetDescription([], html: new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "Only one <govuk-error-summary-description> or <description> element is permitted within each <govuk-error-summary>.",
            ex.Message);
    }

    [Fact]
    public void SetTitle_SetsTitleOnContext()
    {
        // Arrange
        var titleHtml = "Title";

        var context = new ErrorSummaryContext();

        // Act
        context.SetTitle([], new TemplateString(titleHtml), ErrorSummaryTitleTagHelper.TagName);

        // Assert
        Assert.Equal(titleHtml, context.Title?.Html.ToHtmlString());
    }

    [Fact]
    public void SetTitle_AlreadyGotTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetTitle([], html: new TemplateString("Existing title"), ErrorSummaryTitleTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.SetTitle([], html: new TemplateString("Title"), ErrorSummaryTitleTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "Only one <govuk-error-summary-title> or <error-summary-title> element is permitted within each <govuk-error-summary>.",
            ex.Message);
    }

    [Fact]
    public void SetTitle_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.AddItem(
            new ErrorSummaryContextItem(Href: null, Html: new TemplateString("An error"), Attributes: [], ItemAttributes: []),
            ErrorSummaryItemTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetTitle([], html: new TemplateString("Title"), ErrorSummaryTitleTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<govuk-error-summary-title> cannot be used alongside <error-summary-item>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

    [Fact]
    public void SetDescription_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetTitle([], html: new TemplateString("Title"), ErrorSummaryTitleTagHelper.ShortTagName);

        // Act
        var ex = Record.Exception(
            () => context.SetDescription([], html: new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<govuk-error-summary-description> cannot be used alongside <error-summary-title>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

    [Fact]
    public void AddItem_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new ErrorSummaryContext();
        context.SetDescription([], html: new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName);

        // Act
        var ex = Record.Exception(() => context.AddItem(
            new ErrorSummaryContextItem(Href: null, Html: new TemplateString("An error"), Attributes: [], ItemAttributes: []),
            ErrorSummaryItemTagHelper.ShortTagName));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<error-summary-item> cannot be used alongside <govuk-error-summary-description>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

}
