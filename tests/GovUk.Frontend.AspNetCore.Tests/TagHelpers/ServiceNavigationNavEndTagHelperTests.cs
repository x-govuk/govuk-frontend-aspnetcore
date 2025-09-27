using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationNavEndTagHelperTests() : TagHelperTestBase(ServiceNavigationNavEndTagHelper.TagName, ServiceNavigationNavTagHelper.TagName)
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

        var tagHelper = new ServiceNavigationNavEndTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(content, navContext.NavigationEndSlot?.Html);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var navContext = new ServiceNavigationNavContext
        {
            NavigationEndSlot = new("Existing end slot", ServiceNavigationNavEndTagHelper.TagName)
        };

        var context = CreateTagHelperContext(contexts: navContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationNavEndTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
