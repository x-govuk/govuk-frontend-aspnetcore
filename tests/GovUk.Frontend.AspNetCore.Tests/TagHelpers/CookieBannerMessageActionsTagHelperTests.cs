using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionsTagHelperTests : TagHelperTestBase<CookieBannerMessageActionsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsActionsOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();

        var messageContext = new CookieBannerMessageContext(ParentTagName!);
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

    [Fact]
    public async Task ProcessAsync_MessageAlreadyHasActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var messageContext = new CookieBannerMessageContext(ParentTagName!)
        {
            Actions = new CookieBannerMessageActionsContext(TagName)
        };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageActionsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }
}
