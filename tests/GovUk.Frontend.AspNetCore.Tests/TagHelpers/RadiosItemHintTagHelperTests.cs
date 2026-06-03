using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemHintTagHelperTests : TagHelperTestBase<RadiosItemHintTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        var radiosItemContext = new RadiosItemContext();

        var context = CreateTagHelperContext(contexts: radiosItemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hint");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosItemHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Hint", radiosItemContext.Hint?.Options.Html?.ToHtmlString());
    }
}
