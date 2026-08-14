using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageContentTagHelperTests : TagHelperTestBase<CookieBannerMessageContentTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsContentOnContext()
    {
        // Arrange
        var content = "Content";
        var attributes = CreateDummyDataAttributes();

        var messageContext = new CookieBannerMessageContext(ParentTagName!);
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
        Assert.Equal(content, messageContext.Content?.Html?.ToHtmlString());
        AssertContainsAttributes(attributes, messageContext.Content?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_MessageAlreadyHasContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var messageContext = new CookieBannerMessageContext(ParentTagName!)
        {
            Content = new(new HtmlString("Content"), TagName, [])
        };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageContentTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_MessageAlreadyHasActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var actionsTagName = GetSiblingTagName(
            CookieBannerMessageActionsTagHelper.TagName,
            CookieBannerMessageActionsTagHelper.ShortTagName);

        var messageContext = new CookieBannerMessageContext(ParentTagName!)
        {
            Actions = new CookieBannerMessageActionsContext(actionsTagName)
        };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageContentTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{actionsTagName}>.", ex.Message);
    }
}
