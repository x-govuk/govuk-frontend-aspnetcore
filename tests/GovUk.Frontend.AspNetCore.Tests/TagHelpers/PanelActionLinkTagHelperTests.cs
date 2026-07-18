using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelActionLinkTagHelperTests : TagHelperTestBase<PanelActionLinkTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsLinkActionToContext()
    {
        // Arrange
        var content = "No, change my age";
        var href = "#";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

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

        var tagHelper = new PanelActionLinkTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            actionsContext.Actions,
            action =>
            {
                Assert.Equal(content, action.Html);
                Assert.Null(action.Text);
                Assert.Equal(href, action.Href);
                Assert.Null(action.Type);
                Assert.Equal(className, action.Classes);
                AssertContainsAttributes(attributes, action.Attributes, except: "href");
            });
    }
}
