using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryCardActionsTagHelperTests() : TagHelperTestBase(SummaryCardActionsTagHelper.TagName, parentTagName: SummaryCardTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsActionsToContext()
    {
        // Arrange
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var item1Href = "href";
        var item1Content = "First item";
        var item1Vht = "vht";
        var item1Class = CreateDummyClassName();
        var item1Attributes = CreateDummyDataAttributes();

        var item1 = new SummaryListOptionsCardActionsItem()
        {
            Href = item1Href,
            Text = null,
            Html = item1Content,
            VisuallyHiddenText = item1Vht,
            Classes = item1Class,
            Attributes = new(item1Attributes)
        };

        var summaryCardContext = new SummaryCardContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var actionsContext = context.GetContextItem<SummaryCardActionsContext>();
                actionsContext.AddItem(item1);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardActionsTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(summaryCardContext.Actions);

        Assert.Equal(className, summaryCardContext.Actions.Classes);
        AssertContainsAttributes(attributes, summaryCardContext.Actions.Attributes);
        Assert.NotNull(summaryCardContext.Actions.Items);
        Assert.Collection(
            summaryCardContext.Actions.Items,
            item => Assert.Same(item1, item));
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var summaryCardContext = new SummaryCardContext();
        summaryCardContext.SetActions(new());

        var context = CreateTagHelperContext(contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardActionsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Theory]
    [InlineData(SummaryListTagHelper.TagName)]
    public async Task ProcessAsync_ParentHasSummaryList_ThrowsInvalidOperationException(string summaryListTagName)
    {
        // Arrange
        var summaryCardContext = new SummaryCardContext();
        summaryCardContext.SetSummaryList(new());

        var context = CreateTagHelperContext(contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardActionsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{summaryListTagName}>.", ex.Message);
    }
}
