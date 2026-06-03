using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectLabelTagHelperTests : TagHelperTestBase<SelectLabelTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsLabelOnContext()
    {
        // Arrange
        var selectContext = new SelectContext(@for: null);

        var context = CreateTagHelperContext();
        context.SetContextItem(typeof(FormGroupContext3), selectContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Label");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectLabelTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.Label);
        Assert.Equal("Label", selectContext.Label.Html?.ToHtmlString());
    }
}
