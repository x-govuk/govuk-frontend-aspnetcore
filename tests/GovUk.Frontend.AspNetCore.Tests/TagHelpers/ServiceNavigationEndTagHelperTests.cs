using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ServiceNavigationEndTagHelperTests : TagHelperTestBase<ServiceNavigationEndTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsEndSlotOnContext()
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

        var tagHelper = new ServiceNavigationEndTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(content, serviceNavigationContext.EndSlot?.Html.ToHtmlString());
        Assert.Null(serviceNavigationContext.EndSlot?.Align);
    }

    [Fact]
    public async Task ProcessAsync_WithAlign_SetsAlignOnContext()
    {
        // Arrange
        var serviceNavigationContext = new ServiceNavigationContext();

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Content");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationEndTagHelper()
        {
            Align = ServiceNavigationEndSlotAlign.Inline
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(ServiceNavigationEndSlotAlign.Inline, serviceNavigationContext.EndSlot?.Align);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext
        {
            EndSlot = new(new TemplateString("Existing end slot"), null, TagName)
        };

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationEndTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Act
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasStartSlotWithOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var startTagName = GetOtherSpellingSiblingTagName(
            ServiceNavigationStartTagHelper.TagName,
            ServiceNavigationStartTagHelper.ShortTagName);

        var serviceNavigationContext = new ServiceNavigationContext
        {
            StartSlot = new(new TemplateString("Start slot"), startTagName)
        };

        var context = CreateTagHelperContext(contexts: serviceNavigationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new ServiceNavigationEndTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{startTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
