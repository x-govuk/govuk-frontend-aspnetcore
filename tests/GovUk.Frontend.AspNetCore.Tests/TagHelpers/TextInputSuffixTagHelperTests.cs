using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextInputSuffixTagHelperTests() : TagHelperTestBase(TextInputSuffixTagHelper.TagName, TextInputTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsSuffixOnContext()
    {
        // Arrange
        var content = "Suffix";
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

        var tagHelper = new TextInputSuffixTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(inputContext.Suffix);
        Assert.Equal(content, inputContext.Suffix!.Html);
    }
}
