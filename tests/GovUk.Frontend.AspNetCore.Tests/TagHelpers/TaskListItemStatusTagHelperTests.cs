using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TaskListItemStatusTagHelperTests : TagHelperTestBase<TaskListItemStatusTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsStatusOnContext()
    {
        // Arrange
        var content = "<strong class=\"govuk-tag govuk-tag--blue\">Incomplete</strong>";
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
                tagHelperContent.SetHtmlContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TaskListItemStatusTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Status);
        Assert.Equal(content, itemContext.Status.Html?.ToHtmlString());
        Assert.Null(itemContext.Status.Tag);
        Assert.Equal(className, itemContext.Status.Classes);
        AssertContainsAttributes(attributes, itemContext.Status.Attributes);
        Assert.DoesNotContain(itemContext.Status.Attributes!, a => a.Key == "class");
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasStatus_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemContext = new TaskListItemContext(ParentTagName!);
        itemContext.SetStatus(new(), TagName);

        var context = CreateTagHelperContext(contexts: [new TaskListContext(), itemContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new TaskListItemStatusTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{ShortTagName}> or <{PrimaryTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }
}
