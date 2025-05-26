using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryCardTitleTagHelperTests() : TagHelperTestBase(SummaryCardTitleTagHelper.TagName, parentTagName: SummaryCardTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var titleContent = "Title";
        var headingLevel = 3;
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var summaryCardContext = new SummaryCardContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardTitleTagHelper()
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(titleContent, summaryCardContext.Title?.Html);
        Assert.Equal(headingLevel, summaryCardContext.Title?.HeadingLevel);
        Assert.Equal(className, summaryCardContext.Title?.Classes);
        AssertContainsAttributes(attributes, summaryCardContext.Title?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var titleContent = "Title";
        var headingLevel = 3;

        var summaryCardContext = new SummaryCardContext();
        summaryCardContext.SetTitle(new());

        var context = CreateTagHelperContext(contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardTitleTagHelper()
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Theory]
    [InlineData(SummaryCardActionsTagHelper.TagName)]
    public async Task ProcessAsync_ParentHasActions_ThrowsInvalidOperationException(string actionsTagName)
    {
        // Arrange
        var titleContent = "Title";
        var headingLevel = 3;

        var summaryCardContext = new SummaryCardContext();
        summaryCardContext.SetActions(new());

        var context = CreateTagHelperContext(contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardTitleTagHelper()
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{actionsTagName}>.", ex.Message);
    }

    [Theory]
    [InlineData(SummaryListTagHelper.TagName)]
    public async Task ProcessAsync_ParentHasSummaryList_ThrowsInvalidOperationException(string summaryListTagName)
    {
        // Arrange
        var titleContent = "Title";
        var headingLevel = 3;

        var summaryCardContext = new SummaryCardContext();
        summaryCardContext.SetSummaryList(new());

        var context = CreateTagHelperContext(contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardTitleTagHelper()
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{summaryListTagName}>.", ex.Message);
    }
}
