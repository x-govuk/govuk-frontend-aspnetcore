using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListRowActionsTagHelperTests() : TagHelperTestBase(SummaryListRowActionsTagHelper.TagName, parentTagName: SummaryListRowTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsAttributesToContext()
    {
        // Arrange
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
            attributes: attributes);

        var tagHelper = new SummaryListRowActionsTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(rowContext.Actions);
        Assert.Equal(className, rowContext.Actions.Classes);
        AssertContainsAttributes(attributes, rowContext.Actions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetActions(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new SummaryListRowActionsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
