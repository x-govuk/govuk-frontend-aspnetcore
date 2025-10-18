using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterNavItemTagHelperTests : TagHelperTestBase<FooterNavItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var content = "Content";
        var href = "#";
        var linkAttributes = CreateDummyDataAttributes();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext();
        var footerNavItemsContext = new FooterNavItemsContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [footerContext, footerNavContext, footerNavItemsContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavItemTagHelper()
        {
            LinkAttributes = linkAttributes
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            footerNavItemsContext.Items,
            item =>
            {
                Assert.Equal(href, item.Href);
                Assert.Equal(content, item.Html);
                Assert.Null(item.Text);
                AssertContainsAttributes(linkAttributes, item.Attributes);
                AssertContainsAttributes(attributes, item.ItemAttributes, except: "href");
            });
    }
}
