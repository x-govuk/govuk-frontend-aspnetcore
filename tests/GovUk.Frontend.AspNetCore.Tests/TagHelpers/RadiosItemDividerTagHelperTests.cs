using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemDividerTagHelperTests : TagHelperTestBase<RadiosItemDividerTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsDividerToContextItems()
    {
        // Arrange
        var radiosContext = new RadiosContext(name: null, @for: null);

        var context = CreateTagHelperContext(contexts: radiosContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml("Divider");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosItemDividerTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            radiosContext.Items,
            item =>
            {
                var divider = Assert.IsType<RadiosOptionsItem>(item);
                Assert.Equal("Divider", divider.Divider?.ToHtmlString());
            });
    }
}
