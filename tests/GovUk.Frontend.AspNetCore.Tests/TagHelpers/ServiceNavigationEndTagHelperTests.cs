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
        Assert.Equal(content, serviceNavigationContext.EndSlot?.Html);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasEndSlot_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var serviceNavigationContext = new ServiceNavigationContext
        {
            EndSlot = new(new TemplateString("Existing end slot"), ServiceNavigationEndTagHelper.TagName)
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
}
