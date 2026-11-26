using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TaskListItemTitleTagHelperTests : TagHelperTestBase<TaskListItemTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var content = "Company Directors";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var itemContext = new TaskListItemContext(ParentTagName!);

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TaskListItemTitleTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Title);
        Assert.Equal(content, itemContext.Title.Html?.ToHtmlString());
        Assert.Equal(className, itemContext.Title.Classes);
        AssertContainsAttributes(attributes, itemContext.Title.Attributes);
        Assert.DoesNotContain(itemContext.Title.Attributes!, a => a.Key == "class");
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemContext = new TaskListItemContext(ParentTagName!);
        itemContext.SetTitle(new(), TagName);

        var context = CreateTagHelperContext(contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{ShortTagName}> or <{PrimaryTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var hintTagName = GetSiblingTagName(
            TaskListItemHintTagHelper.TagName,
            TaskListItemHintTagHelper.ShortTagName);

        var itemContext = new TaskListItemContext(ParentTagName!);
        itemContext.SetHint(new(), hintTagName);

        var context = CreateTagHelperContext(contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{hintTagName}>.", ex.Message);
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

        var tagHelper = new TaskListItemTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{statusTagName}>.", ex.Message);
    }
}
