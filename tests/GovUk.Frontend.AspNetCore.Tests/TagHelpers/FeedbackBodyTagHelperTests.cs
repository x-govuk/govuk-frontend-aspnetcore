using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FeedbackBodyTagHelperTests : TagHelperTestBase<FeedbackBodyTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsBodyOnContext()
    {
        // Arrange
        var content = "Tell us about your experience";
        var attributes = CreateDummyDataAttributes();

        var feedbackContext = new FeedbackContext();

        var context = CreateTagHelperContext(attributes: attributes, contexts: feedbackContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FeedbackBodyTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(feedbackContext.Body);
        Assert.Equal(content, feedbackContext.Body.Value.Content.ToHtmlString());
        AssertContainsAttributes(attributes, feedbackContext.Body.Value.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasBody_ThrowsInvalidOperationException()
    {
        // Arrange
        var feedbackContext = new FeedbackContext();
        feedbackContext.SetBody(new TemplateString("Existing"), [], TagName);

        var context = CreateTagHelperContext(contexts: feedbackContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FeedbackBodyTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var titleTagName = GetOtherSpellingSiblingTagName(
            FeedbackTitleTagHelper.TagName,
            FeedbackTitleTagHelper.ShortTagName);

        var feedbackContext = new FeedbackContext();
        feedbackContext.SetTitle(new TemplateString("Title"), [], titleTagName);

        var context = CreateTagHelperContext(contexts: feedbackContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FeedbackBodyTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{titleTagName}>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
