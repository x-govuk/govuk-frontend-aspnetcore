using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageActionButtonTagHelperTests : TagHelperTestBase<CookieBannerMessageActionButtonTagHelper>
{
    /// <summary>
    /// The spelling of the message the actions under test are inside; short names pair with short
    /// names all the way up.
    /// </summary>
    private string MessageTagName => UsesShortTagName ?
        CookieBannerMessageTagHelper.ShortTagName :
        CookieBannerMessageTagHelper.TagName;

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

        var actionsContext = new CookieBannerMessageActionsContext(ParentTagName!);
        var messageContext = new CookieBannerMessageContext(MessageTagName) { Actions = actionsContext };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [cookieBannerContext, messageContext, actionsContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes);

        var tagHelper = new CookieBannerMessageActionButtonTagHelper()
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

    [Fact]
    public async Task ProcessAsync_WithGeneratedFormAction_SetsFormActionAttribute()
    {
        // Arrange
        var formAction = "/cookies/accept";
        var attributes = new Dictionary<string, string?> { { "formaction", formAction } };

        var actionsContext = new CookieBannerMessageActionsContext(ParentTagName!);
        var messageContext = new CookieBannerMessageContext(MessageTagName) { Actions = actionsContext };
        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [cookieBannerContext, messageContext, actionsContext]);

        var output = CreateTagHelperOutput(attributes: attributes);

        var tagHelper = new CookieBannerMessageActionButtonTagHelper()
        {
            Text = "Accept analytics cookies",
            Type = "submit"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            actionsContext.Actions,
            action => AssertContainsAttributes(attributes, action.Attributes));
    }
}
