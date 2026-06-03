using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SkipLinkTagHelperTests : TagHelperTestBase<SkipLinkTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "Link content";
        var href = "#";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<SkipLinkOptions>(nameof(IComponentGenerator.GenerateSkipLinkAsync));

        var tagHelper = new SkipLinkTagHelper(componentGenerator)
        {
            Href = href
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(href, actualOptions.Href);
    }
}
