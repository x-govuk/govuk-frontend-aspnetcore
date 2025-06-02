using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageHeadingTagHelperTests() : TagHelperTestBase(CookieBannerMessageHeadingTagHelper.TagName, CookieBannerTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsContentOnContext()
    {
        // Arrange
        var heading = "Content";
        var attributes = CreateDummyDataAttributes();

        var messageContext = new CookieBannerMessageContext();
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(heading);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new CookieBannerMessageHeadingTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(messageContext.Heading);
        Assert.Equal(heading, messageContext.Heading?.Html);
        AssertContainsAttributes(attributes, messageContext.Heading?.Attributes);
    }
}
