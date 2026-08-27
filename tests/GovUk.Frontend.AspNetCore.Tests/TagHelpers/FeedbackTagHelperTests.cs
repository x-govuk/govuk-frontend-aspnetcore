using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FeedbackTagHelperTests : TagHelperTestBase<FeedbackTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Help us improve this service";
        var bodyContent = "Tell us about your experience";
        var headingLevel = 3;
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var titleAttributes = CreateDummyDataAttributes();
        var bodyAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var feedbackContext = context.GetContextItem<FeedbackContext>();

                feedbackContext.SetTitle(
                    new TemplateString(titleContent),
                    new AttributeCollection(titleAttributes),
                    FeedbackTitleTagHelper.TagName);

                feedbackContext.SetBody(
                    new TemplateString(bodyContent),
                    new AttributeCollection(bodyAttributes),
                    FeedbackBodyTagHelper.TagName);

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<FeedbackOptions>(nameof(IComponentGenerator.GenerateFeedbackAsync));

        var tagHelper = new FeedbackTagHelper(componentGenerator)
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(headingLevel, actualOptions.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml?.ToHtmlString());
        AssertContainsAttributes(titleAttributes, actualOptions.TitleAttributes);
        Assert.Equal(bodyContent, actualOptions.Html?.ToHtmlString());
        AssertContainsAttributes(bodyAttributes, actualOptions.BodyAttributes);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_NoTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, _) =
            CreateComponentGenerator<FeedbackOptions>(nameof(IComponentGenerator.GenerateFeedbackAsync));

        var tagHelper = new FeedbackTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A <{FeedbackTitleTagHelper.TagName}> or <{FeedbackTitleTagHelper.ShortTagName}> element must be provided.",
            ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    public void HeadingLevel_OutOfRange_ThrowsArgumentOutOfRangeException(int headingLevel)
    {
        // Arrange
        var (componentGenerator, _) =
            CreateComponentGenerator<FeedbackOptions>(nameof(IComponentGenerator.GenerateFeedbackAsync));

        var tagHelper = new FeedbackTagHelper(componentGenerator);

        // Act
        var ex = Record.Exception(() => tagHelper.HeadingLevel = headingLevel);

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(ex);
    }
}
