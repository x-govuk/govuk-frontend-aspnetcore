using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterMetaContentTagHelperTests() : TagHelperTestBase(FooterMetaContentTagHelper.TagName, FooterMetaTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetContentOnContext()
    {
        // Arrange
        var content = "Content";
        var attributes = CreateDummyDataAttributes();

        var footerContext = new FooterContext();
        var footerMetaContext = new FooterMetaContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [footerContext, footerMetaContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaContentTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(content, footerMetaContext.Content?.Html);
        AssertContainsAttributes(attributes, footerMetaContext.Content?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerMetaContext = new FooterMetaContext
        {
            Content = new("Content", [], TagName)
        };

        var context = CreateTagHelperContext(contexts: [footerContext, footerMetaContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new FooterMetaContentTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
