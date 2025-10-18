using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionLinkTagHelperTests : TagHelperTestBase<CookieBannerMessageActionLinkTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsActionToContext()
    {
        // Arrange
        var text = "Action";
        var href = "#";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

        var actionsContext = new CookieBannerMessageActionsContext();
        var messageContext = new CookieBannerMessageContext() { Actions = actionsContext };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [cookieBannerContext, messageContext, actionsContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes);

        var tagHelper = new CookieBannerMessageActionLinkTagHelper()
        {
            Text = text
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            actionsContext.Actions,
            action =>
            {
                Assert.Equal(text, action.Text);
                Assert.Equal(className, action.Classes);
                Assert.Equal(href, action.Href);
                Assert.Null(action.Name);
                Assert.Null(action.Value);
                Assert.Null(action.Type);
                AssertContainsAttributes(attributes, action.Attributes, except: "href");
            });
    }
}
