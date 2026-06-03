using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectHintTagHelperTests : TagHelperTestBase<SelectHintTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        var selectContext = new SelectContext(@for: null);

        var context = CreateTagHelperContext();
        context.SetContextItem(typeof(FormGroupContext3), selectContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hint");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.Hint);
        Assert.Equal("Hint", selectContext.Hint.Html?.ToHtmlString());
    }
}
