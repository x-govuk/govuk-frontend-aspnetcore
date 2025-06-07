using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterMetaItemTagHelperTests() : TagHelperTestBase(FooterMetaItemTagHelper.TagName, FooterMetaItemsTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var content = "Item";
        var href = "#";
        var linkAttributes = CreateDummyDataAttributes();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

        var footerContext = new FooterContext();
        var metaContext = new FooterMetaContext();
        var metaItemsContext = new FooterMetaItemsContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [footerContext, metaContext, metaItemsContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaItemTagHelper()
        {
            LinkAttributes = linkAttributes
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            metaItemsContext.Items,
            item =>
            {
                Assert.Equal(content, item.Html);
                Assert.Equal(href, item.Href);
                Assert.Null(item.Text);
                AssertContainsAttributes(linkAttributes, item.Attributes);
                AssertContainsAttributes(attributes, item.ItemAttributes, except: "href");
            });
    }
}
