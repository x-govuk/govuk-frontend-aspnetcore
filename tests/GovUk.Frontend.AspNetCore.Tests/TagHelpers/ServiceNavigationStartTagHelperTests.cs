using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationStartTagHelperTests : TagHelperTestBase<ServiceNavigationStartTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsStartSlotOnContext()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext();

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationStartTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(content, serviceNavigationContext.StartSlot?.Html);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasStartSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext
        {
            StartSlot = new(new TemplateString("Existing start slot"), TagName)
        };

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationStartTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasNav_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext
        {
            Nav = new ServiceNavigationNavContext() { TagName = ServiceNavigationTagHelper.TagName }
        };

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationStartTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{ServiceNavigationTagHelper.TagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext
        {
            EndSlot = new(new TemplateString("End slot"), ServiceNavigationEndTagHelper.TagName)
        };

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationStartTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{ServiceNavigationEndTagHelper.TagName}>.", ex.Message);
    }
}
