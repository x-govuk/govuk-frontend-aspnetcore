using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelActionButtonTagHelperTests : TagHelperTestBase<PanelActionButtonTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsButtonActionToContext()
    {
        // Arrange
        var content = "Yes, this is correct";
        var type = "submit";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var actionsContext = new PanelActionsContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [actionsContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelActionButtonTagHelper()
        {
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
                Assert.Equal(content, action.Html.ToHtmlString());
                Assert.Null(action.Text);
                Assert.Equal(type, action.Type);
                Assert.Null(action.Href);
                Assert.Equal(className, action.Classes);
                AssertContainsAttributes(attributes, action.Attributes);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithGeneratedFormAction_SetsFormActionAttribute()
    {
        // Arrange
        var formAction = "/Home/Confirm";
        var attributes = new Dictionary<string, string?> { { "formaction", formAction } };

        var actionsContext = new PanelActionsContext();

        var context = CreateTagHelperContext(attributes: attributes, contexts: [actionsContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent("Confirm");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelActionButtonTagHelper() { Type = "submit" };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            actionsContext.Actions,
            action => AssertContainsAttributes(attributes, action.Attributes));
    }

    [Fact]
    public async Task ProcessAsync_WithNoType_DefaultsTypeToButton()
    {
        // Arrange
        var actionsContext = new PanelActionsContext();

        var context = CreateTagHelperContext(contexts: [actionsContext]);
        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent("Action");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelActionButtonTagHelper();
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(actionsContext.Actions, action => Assert.Equal("button", action.Type));
    }
}
