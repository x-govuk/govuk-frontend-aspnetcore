using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TaskListItemHintTagHelperTests : TagHelperTestBase<TaskListItemHintTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        var content = "Ensure the plan is up to date";
        var attributes = CreateDummyDataAttributes();

        var itemContext = new TaskListItemContext(ParentTagName!);

        var context = CreateTagHelperContext(attributes: attributes, contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TaskListItemHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Hint);
        Assert.Equal(content, itemContext.Hint.Html?.ToHtmlString());
        AssertContainsAttributes(attributes, itemContext.Hint.Attributes);
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemContext = new TaskListItemContext(ParentTagName!);
        itemContext.SetHint(new(), TagName);

        var context = CreateTagHelperContext(contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemHintTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{ShortTagName}> or <{PrimaryTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasStatus_ThrowsInvalidOperationException()
    {
        // Arrange
        var statusTagName = GetSiblingTagName(
            TaskListItemStatusTagHelper.TagName,
            TaskListItemStatusTagHelper.ShortTagName);

        var itemContext = new TaskListItemContext(ParentTagName!);
        itemContext.SetStatus(new(), statusTagName);

        var context = CreateTagHelperContext(contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemHintTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{statusTagName}>.", ex.Message);
    }
}
