using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TaskListTagHelperTests : TagHelperTestBase<TaskListTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        TaskListOptionsItem[] items =
        [
            new TaskListOptionsItem { Href = "first" },
            new TaskListOptionsItem()
        ];

        var idPrefix = "tasks";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var taskListContext = context.GetContextItem<TaskListContext>();

                foreach (var item in items)
                {
                    taskListContext.AddItem(item);
                }

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<TaskListOptions>(nameof(IComponentGenerator.GenerateTaskListAsync));

        var tagHelper = new TaskListTagHelper(componentGenerator)
        {
            IdPrefix = idPrefix
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Same(items[0], item),
            item => Assert.Same(items[1], item));
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }
}
