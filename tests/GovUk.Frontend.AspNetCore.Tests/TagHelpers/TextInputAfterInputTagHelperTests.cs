using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextInputAfterInputTagHelperTests : TagHelperTestBase<TextInputAfterInputTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsAfterInputOnContext()
    {
        // Arrange
        var content = "AfterInput";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var inputContext = new TextInputContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: inputContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TextInputAfterInputTagHelper(new NullLogger<TextInputAfterInputTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(inputContext.AfterInput);
        Assert.Equal(content, inputContext.AfterInput);
    }
}
