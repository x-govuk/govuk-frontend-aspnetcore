using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerMessageTagHelperTests() : TagHelperTestBase(CookieBannerMessageTagHelper.TagName, CookieBannerTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsMessageToContext()
    {
        // Arrange
        var hidden = true;
        var role = "role";
        var headingContent = "Heading";
        var messageContent = "Message";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var headingAttributes = CreateDummyDataAttributes();
        var contentAttributes = CreateDummyDataAttributes();
        var actionsAttributes = CreateDummyDataAttributes();

        var actionText = "Action";
        var actionName = "Action";
        var actionType = "action-type";
        var actionValue = "Value";
        var actionClassName = CreateDummyClassName();
        var actionAttributes = CreateDummyDataAttributes();

        var cookieBannerContext = new CookieBannerContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: cookieBannerContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var messageContext = context.GetContextItem<CookieBannerMessageContext>();

                messageContext.Heading = new(headingContent, CookieBannerMessageHeadingTagHelper.TagName, new(headingAttributes));

                messageContext.Actions = new CookieBannerMessageActionsContext()
                {
                    Attributes = new(actionsAttributes)
                };
                messageContext.Actions.Actions.Add(new CookieBannerOptionsMessageAction()
                {
                    Text = actionText,
                    Type = actionType,
                    Href = null,
                    Name = actionName,
                    Value = actionValue,
                    Classes = actionClassName,
                    Attributes = new(actionAttributes)
                });

                messageContext.Content = new(messageContent, CookieBannerMessageContentTagHelper.TagName, new(contentAttributes));

                TagHelperContent content = new DefaultTagHelperContent();
                return Task.FromResult(content);
            });

        var tagHelper = new CookieBannerMessageTagHelper()
        {
            Hidden = hidden,
            Role = role
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            cookieBannerContext.Messages,
            message =>
            {
                Assert.Null(message.HeadingText);
                Assert.Equal(headingContent, message.HeadingHtml);
                Assert.Null(message.Text);
                Assert.Equal(messageContent, message.Html);
                Assert.NotNull(message.Actions);
                Assert.Collection(
                    message.Actions,
                    action =>
                    {
                        Assert.Equal(actionText, action.Text);
                        Assert.Equal(actionName, action.Name);
                        Assert.Equal(actionType, action.Type);
                        Assert.Equal(actionValue, action.Value);
                        Assert.Equal(actionClassName, action.Classes);
                        AssertContainsAttributes(actionAttributes, action.Attributes);
                    });
                Assert.Equal(hidden, message.Hidden);
                Assert.Equal(role, message.Role);
                Assert.Equal(className, message.Classes);
                AssertContainsAttributes(attributes, message.Attributes);
                AssertContainsAttributes(headingAttributes, message.HeadingAttributes);
                AssertContainsAttributes(contentAttributes, message.ContentAttributes);
                AssertContainsAttributes(actionsAttributes, message.ActionsAttributes);
            });
    }
}
