using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TaskListItemTagHelperTests : TagHelperTestBase<TaskListItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var title = new TaskListOptionsItemTitle();
        var hint = new TaskListOptionsItemHint();
        var status = new TaskListOptionsItemStatus();
        var href = "/task";
        var className = CreateDummyClassName();
        var dataAttributes = CreateDummyDataAttributes();
        var attributes = new Dictionary<string, string?>(dataAttributes) { { "href", href } };

        var taskListContext = new TaskListContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: taskListContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<TaskListItemContext>();
                itemContext.SetTitle(title, TaskListItemTitleTagHelper.TagName);
                itemContext.SetHint(hint, TaskListItemHintTagHelper.TagName);
                itemContext.SetStatus(status, TaskListItemStatusTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TaskListItemTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(taskListContext.Items);
        Assert.Same(title, item.Title);
        Assert.Same(hint, item.Hint);
        Assert.Same(status, item.Status);
        Assert.Equal(href, item.Href);
        Assert.Equal(className, item.Classes);
        AssertContainsAttributes(dataAttributes, item.Attributes);
        Assert.NotNull(item.Attributes);
        Assert.DoesNotContain(item.Attributes, a => a.Key is "href" or "class");
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_NoHref_AddsItemWithoutHrefToContext()
    {
        // Arrange
        var taskListContext = new TaskListContext();

        var context = CreateTagHelperContext(contexts: taskListContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<TaskListItemContext>();
                itemContext.SetTitle(new TaskListOptionsItemTitle(), TaskListItemTitleTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TaskListItemTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(taskListContext.Items);
        Assert.Null(item.Href);
        Assert.Null(item.Hint);
        Assert.Null(item.Status);
    }

    [Fact]
    public async Task ProcessAsync_MissingTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var taskListContext = new TaskListContext();

        var context = CreateTagHelperContext(contexts: taskListContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"A <{TaskListItemTitleTagHelper.TagName}> element must be provided.", ex.Message);
    }
}
