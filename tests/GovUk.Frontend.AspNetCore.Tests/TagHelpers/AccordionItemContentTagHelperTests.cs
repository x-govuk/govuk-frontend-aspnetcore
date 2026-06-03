using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionItemContentTagHelperTests : TagHelperTestBase<AccordionItemContentTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsContentToContext()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext();

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemContentTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Content);
        Assert.Equal("Content", itemContext.Content?.Content?.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ItemAlreadyHasContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext();
        itemContext.SetContent(new AttributeCollection(), content: new TemplateString("Existing content"));

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemContentTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-accordion-item-content> is permitted for each <govuk-accordion-item>.", ex.Message);
    }
}
