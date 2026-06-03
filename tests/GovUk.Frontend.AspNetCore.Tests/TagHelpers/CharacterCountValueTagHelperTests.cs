using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CharacterCountValueTagHelperTests : TagHelperTestBase<CharacterCountValueTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsValueOnContext()
    {
        // Arrange
        var characterCountContext = new CharacterCountContext();

        var context = CreateTagHelperContext(contexts: characterCountContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Value");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CharacterCountValueTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Value", characterCountContext.Value);
    }
}
