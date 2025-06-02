using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionTagHelperTests() : TagHelperTestBase(CookieBannerMessageActionTagHelper.TagName, CookieBannerTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsActionToContext()
    {
        // Arrange
        var text = "Action";
        var name = "Name";
        var type = "type";
        var value = "Value";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

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

        var tagHelper = new CookieBannerMessageActionTagHelper()
        {
            Name = name,
            Text = text,
            Type = type,
            Value = value
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
                Assert.Null(action.Href);
                Assert.Equal(name, action.Name);
                Assert.Equal(value, action.Value);
                Assert.Equal(type, action.Type);
                AssertContainsAttributes(attributes, action.Attributes);
            });
    }
}
