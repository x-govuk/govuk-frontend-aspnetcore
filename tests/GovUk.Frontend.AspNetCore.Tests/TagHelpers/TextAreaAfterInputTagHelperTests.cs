using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextAreaAfterInputTagHelperTests : TagHelperTestBase<TextAreaAfterInputTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsAfterInputOnContext()
    {
        // Arrange
        var content = "AfterInput";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var textAreaContext = new TextAreaContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: textAreaContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TextAreaAfterInputTagHelper(new NullLogger<TextAreaAfterInputTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(textAreaContext.AfterInput);
        Assert.Equal(content, textAreaContext.AfterInput);
    }
}
