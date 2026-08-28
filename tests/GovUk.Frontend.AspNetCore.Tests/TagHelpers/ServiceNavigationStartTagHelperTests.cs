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
        Assert.Equal(content, serviceNavigationContext.StartSlot?.Html.ToHtmlString());
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

        var navTagName = GetSiblingTagName(
            ServiceNavigationNavTagHelper.TagName,
            ServiceNavigationNavTagHelper.ShortTagName);

        var serviceNavigationContext = new ServiceNavigationContext
        {
            Nav = new ServiceNavigationNavContext() { TagName = navTagName }
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
        Assert.Equal($"<{TagName}> must be specified before <{navTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var endTagName = GetSiblingTagName(
            ServiceNavigationEndTagHelper.TagName,
            ServiceNavigationEndTagHelper.ShortTagName);

        var serviceNavigationContext = new ServiceNavigationContext
        {
            EndSlot = new(new TemplateString("End slot"), null, endTagName)
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
        Assert.Equal($"<{TagName}> must be specified before <{endTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasNavWithOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        // The spelling is checked before the ordering, so this is the error even though a
        // <start> cannot follow the navigation whichever way it is spelled
        var navTagName = GetOtherSpellingSiblingTagName(
            ServiceNavigationNavTagHelper.TagName,
            ServiceNavigationNavTagHelper.ShortTagName);

        var serviceNavigationContext = new ServiceNavigationContext
        {
            Nav = new ServiceNavigationNavContext() { TagName = navTagName }
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
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{navTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
