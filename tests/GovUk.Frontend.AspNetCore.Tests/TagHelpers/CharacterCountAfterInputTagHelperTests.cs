using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CharacterCountAfterInputTagHelperTests : TagHelperTestBase<CharacterCountAfterInputTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsAfterInputOnContext()
    {
        // Arrange
        var content = "AfterInput";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var characterCountContext = new CharacterCountContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: characterCountContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CharacterCountAfterInputTagHelper(new NullLogger<CharacterCountAfterInputTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(characterCountContext.AfterInput);
        Assert.Equal(content, characterCountContext.AfterInput);
    }
}
