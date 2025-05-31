using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationNavItemTagHelperTests() : TagHelperTestBase(ServiceNavigationNavItemTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var active = false;
        var current = true;
        var content = "Content";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var navContext = new ServiceNavigationNavContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: navContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationNavItemTagHelper
        {
            Active = active,
            Current = current
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            navContext.Items,
            item =>
            {
                AssertContainsAttributes(attributes, item.Attributes);
                Assert.Equal(active, item.Active);
                Assert.Equal(current, item.Current);
                Assert.Equal(content, item.Html);
                Assert.Null(item.Text);
            });
    }
}
