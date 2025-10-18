using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationNavStartTagHelperTests : TagHelperTestBase<ServiceNavigationNavStartTagHelper>
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

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasStartSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var navContext = new ServiceNavigationNavContext
        {
            NavigationStartSlot = new("Existing start slot", TagName)
        };

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
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var navContext = new ServiceNavigationNavContext();
        navContext.Items.Add(new ServiceNavigationOptionsNavigationItem());
        navContext.FirstItemTagName = ServiceNavigationNavItemTagHelper.TagName;

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
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{ServiceNavigationNavItemTagHelper.TagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var navContext = new ServiceNavigationNavContext
        {
            NavigationEndSlot = new("End slot", ServiceNavigationNavEndTagHelper.TagName)
        };

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
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{ServiceNavigationNavEndTagHelper.TagName}>.", ex.Message);
    }
}
