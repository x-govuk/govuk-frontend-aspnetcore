using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextAreaBeforeInputTagHelperTests : TagHelperTestBase<TextAreaBeforeInputTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsBeforeInputOnContext()
    {
        // Arrange
        var content = "BeforeInput";
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

        var tagHelper = new TextAreaBeforeInputTagHelper(new NullLogger<TextAreaBeforeInputTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(textAreaContext.BeforeInput);
        Assert.Equal(content, textAreaContext.BeforeInput);
    }
}
