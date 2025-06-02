using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageContentTagHelperTests() : TagHelperTestBase(CookieBannerMessageContentTagHelper.TagName, CookieBannerTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsContentOnContext()
    {
        // Arrange
        var content = "Content";
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
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new CookieBannerMessageContentTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(messageContext.Content);
        Assert.Equal(content, messageContext.Content?.Html);
        AssertContainsAttributes(attributes, messageContext.Content?.Attributes);
    }
}
