using GovUk.Frontend.AspNetCore.TagHelpers;
#pragma warning disable GFA0006 // Type or member is obsolete

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionTagHelperTests : TagHelperTestBase<CookieBannerMessageActionTagHelper>
{
    [Fact]
    public async Task ProcessAsync_DeprecatedTagName_AddsActionToContext()
    {
        // Arrange
        var text = "Action";
        var type = "type";

        var actionsContext = new CookieBannerMessageActionsContext(ParentTagName!);
        var messageContext = new CookieBannerMessageContext(CookieBannerMessageTagHelper.TagName) { Actions = actionsContext };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(contexts: [cookieBannerContext, messageContext, actionsContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new CookieBannerMessageActionTagHelper()
        {
            Text = text,
            Type = type
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
                Assert.Equal(type, action.Type);
                Assert.Null(action.Href);
            });
    }
}
