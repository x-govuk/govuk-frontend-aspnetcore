using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageHeadingTagHelperTests : TagHelperTestBase<CookieBannerMessageHeadingTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsContentOnContext()
    {
        // Arrange
        var heading = "Content";
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
                tagHelperContent.SetContent(heading);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new CookieBannerMessageHeadingTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(messageContext.Heading);
        Assert.Equal(heading, messageContext.Heading?.Html?.ToHtmlString());
        AssertContainsAttributes(attributes, messageContext.Heading?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_MessageAlreadyHasHeading_ThrowsInvalidOperationException()
    {
        // Arrange
        var messageContext = new CookieBannerMessageContext(ParentTagName!)
        {
            Heading = new(new HtmlString("Heading"), TagName, [])
        };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageHeadingTagHelper();

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
    public async Task ProcessAsync_MessageAlreadyHasContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var contentTagName = GetSiblingTagName(
            CookieBannerMessageContentTagHelper.TagName,
            CookieBannerMessageContentTagHelper.ShortTagName);

        var messageContext = new CookieBannerMessageContext(ParentTagName!)
        {
            Content = new(new HtmlString("Content"), contentTagName, [])
        };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageHeadingTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{contentTagName}>.", ex.Message);
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

        var tagHelper = new CookieBannerMessageHeadingTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{actionsTagName}>.", ex.Message);
    }
}
