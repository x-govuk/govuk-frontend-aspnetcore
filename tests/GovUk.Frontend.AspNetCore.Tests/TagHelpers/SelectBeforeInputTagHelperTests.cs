using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectBeforeInputTagHelperTests : TagHelperTestBase<SelectBeforeInputTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsBeforeInputOnContext()
    {
        // Arrange
        var content = "BeforeInput";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var selectContext = new SelectContext(@for: null);

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: selectContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectBeforeInputTagHelper(new NullLogger<SelectBeforeInputTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.BeforeInput);
        Assert.Equal(content, selectContext.BeforeInput);
    }
}
