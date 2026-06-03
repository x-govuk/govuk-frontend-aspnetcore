using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemConditionalTagHelperTests : TagHelperTestBase<RadiosItemConditionalTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsConditionalOnContext()
    {
        // Arrange
        var radiosItemContext = new RadiosItemContext();

        var context = CreateTagHelperContext(contexts: radiosItemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Conditional");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosItemConditionalTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Conditional", radiosItemContext.Conditional?.Options.Html?.ToHtmlString());
    }
}
