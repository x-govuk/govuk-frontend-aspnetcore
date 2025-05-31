using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationNavStartTagHelperTests() : TagHelperTestBase(ServiceNavigationNavStartTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsStartSlotOnContext()
    {
        // Arrange
        var content = "Content";

        var navContext = new ServiceNavigationNavContext();

        var context = CreateTagHelperContext(contexts: navContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationNavStartTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(content, navContext.NavigationStartSlot?.Html);
    }
}
