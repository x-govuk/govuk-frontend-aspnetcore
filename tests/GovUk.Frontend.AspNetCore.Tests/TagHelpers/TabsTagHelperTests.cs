using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TabsTagHelperTests : TagHelperTestBase<TabsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "id";
        var title = "title";
        var firstItemId = "id1";
        var firstItemLabel = "First";
        var firstItemContent = "First content";
        var secondItemId = "id2";
        var secondItemLabel = "Second";
        var secondItemContent = "second content";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tabsContext = context.GetContextItem<TabsContext>();

                tabsContext.AddItem(new TabsOptionsItem()
                {
                    Id = firstItemId,
                    Label = firstItemLabel,
                    Panel = new TabsOptionsItemPanel()
                    {
                        Html = firstItemContent
                    }
                });

                tabsContext.AddItem(new TabsOptionsItem()
                {
                    Id = secondItemId,
                    Label = secondItemLabel,
                    Panel = new TabsOptionsItemPanel()
                    {
                        Html = secondItemContent
                    }
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TabsOptions>(nameof(IComponentGenerator.GenerateTabsAsync));

        var tagHelper = new TabsTagHelper(componentGenerator)
        {
            Id = id,
            Title = title
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(title, actualOptions.Title);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item =>
            {
                Assert.NotNull(item);
                Assert.Equal(firstItemId, item.Id);
                Assert.Equal(firstItemLabel, item.Label);
                Assert.Equal(firstItemContent, item.Panel?.Html);
            },
            item =>
            {
                Assert.NotNull(item);
                Assert.Equal(secondItemId, item.Id);
                Assert.Equal(secondItemLabel, item.Label);
                Assert.Equal(secondItemContent, item.Panel?.Html);
            });
    }
}
