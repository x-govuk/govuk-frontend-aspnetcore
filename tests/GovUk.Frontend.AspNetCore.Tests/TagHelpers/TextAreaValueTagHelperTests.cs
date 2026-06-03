using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextAreaValueTagHelperTests : TagHelperTestBase<TextAreaValueTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsValueOnContext()
    {
        // Arrange
        var textAreaContext = new TextAreaContext();

        var context = CreateTagHelperContext(contexts: textAreaContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Value");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TextAreaValueTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Value", textAreaContext.Value?.ToHtmlString());
    }
}
