using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FeedbackTitleTagHelperTests : TagHelperTestBase<FeedbackTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var content = "Help us improve this service";
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

        var tagHelper = new FeedbackTitleTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(feedbackContext.Title);
        Assert.Equal(content, feedbackContext.Title.Value.Content.ToHtmlString());
        AssertContainsAttributes(attributes, feedbackContext.Title.Value.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var feedbackContext = new FeedbackContext();
        feedbackContext.SetTitle(new TemplateString("Existing"), [], TagName);

        var context = CreateTagHelperContext(contexts: feedbackContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FeedbackTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasBody_ThrowsInvalidOperationException()
    {
        // Arrange
        var bodyTagName = GetSiblingTagName(FeedbackBodyTagHelper.TagName, FeedbackBodyTagHelper.ShortTagName);

        var feedbackContext = new FeedbackContext();
        feedbackContext.SetBody(new TemplateString("Body"), [], bodyTagName);

        var context = CreateTagHelperContext(contexts: feedbackContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FeedbackTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{bodyTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var bodyTagName = GetOtherSpellingSiblingTagName(
            FeedbackBodyTagHelper.TagName,
            FeedbackBodyTagHelper.ShortTagName);

        var feedbackContext = new FeedbackContext();
        feedbackContext.SetBody(new TemplateString("Body"), [], bodyTagName);

        var context = CreateTagHelperContext(contexts: feedbackContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FeedbackTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{bodyTagName}>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
