using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionsTagHelperTests : TagHelperTestBase<CookieBannerMessageActionsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsActionsOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();

        var messageContext = new CookieBannerMessageContext();
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput(attributes: attributes);

        var tagHelper = new CookieBannerMessageActionsTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(messageContext.Actions);
        AssertContainsAttributes(attributes, messageContext.Actions.Attributes);
    }
}
