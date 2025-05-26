using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListRowValueTagHelperTests() : TagHelperTestBase(SummaryListRowValueTagHelper.TagName, parentTagName: SummaryListRowTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsValueToContext()
    {
        // Arrange
        var content = "Value";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowValueTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(rowContext.Value);
        Assert.Equal(content, rowContext.Value.Html);
        Assert.Equal(className, rowContext.Value.Classes);
        AssertContainsAttributes(attributes, rowContext.Value.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Value";

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetValue(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowValueTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Theory]
    [InlineData(SummaryListRowActionsTagHelper.TagName)]
    public async Task ProcessAsync_ParentHasActions_ThrowsInvalidOperationException(string actionsTagName)
    {
        // Arrange
        var content = "Value";

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetActions(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowValueTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{actionsTagName}>.", ex.Message);
    }
}
