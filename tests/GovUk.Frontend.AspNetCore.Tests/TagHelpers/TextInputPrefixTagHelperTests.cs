using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextInputPrefixTagHelperTests : TagHelperTestBase<TextInputPrefixTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsPrefixOnContext()
    {
        // Arrange
        var content = "Prefix";
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

        var tagHelper = new TextInputPrefixTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(inputContext.Prefix);
        Assert.Equal(content, inputContext.Prefix!.Html);
    }
}
