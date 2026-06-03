using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class BreadcrumbsTagHelperTests : TagHelperTestBase<BreadcrumbsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        BreadcrumbsOptionsItem[] items =
        [
            new BreadcrumbsOptionsItem()
            {
                Href = "first",
                Html = "First"
            },
            new BreadcrumbsOptionsItem()
            {
                Href = "second",
                Html = "Second"
            },
            new BreadcrumbsOptionsItem()
            {
                Html = "Last"
            }
        ];

        var collapseOnMobile = true;
        var labelText = "Label text";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var breadcrumbsContext = context.GetContextItem<BreadcrumbsContext>();

                foreach (var item in items)
                {
                    breadcrumbsContext.AddItem(item);
                }

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<BreadcrumbsOptions>(nameof(IComponentGenerator.GenerateBreadcrumbsAsync));

        var tagHelper = new BreadcrumbsTagHelper(componentGenerator)
        {
            CollapseOnMobile = collapseOnMobile,
            LabelText = labelText,
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(collapseOnMobile, actualOptions.CollapseOnMobile);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Same(items[0], item),
            item => Assert.Same(items[1], item),
            item => Assert.Same(items[2], item));
        Assert.Equal(labelText, actualOptions.LabelText);
    }
}
