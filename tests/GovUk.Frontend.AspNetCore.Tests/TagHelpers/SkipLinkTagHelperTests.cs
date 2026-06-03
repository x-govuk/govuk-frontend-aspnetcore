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

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        SkipLinkOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateSkipLinkAsync(It.IsAny<SkipLinkOptions>())).Callback<SkipLinkOptions>(o => actualOptions = o);

        var tagHelper = new SkipLinkTagHelper(componentGeneratorMock.Object)
        {
            Href = href
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(content, actualOptions!.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(href, actualOptions.Href);
    }
}
