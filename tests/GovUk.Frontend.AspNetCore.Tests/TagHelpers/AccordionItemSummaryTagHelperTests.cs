using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionItemSummaryTagHelperTests : TagHelperTestBase<AccordionItemSummaryTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsSummaryToContext()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext();

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Summary content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemSummaryTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Summary);
        Assert.Equal("Summary content", itemContext.Summary?.Content.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ItemAlreadyHasSummary_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext();
        itemContext.SetSummary(new AttributeCollection(), new TemplateString("Existing summary"));

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Summary content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemSummaryTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-accordion-item-summary> is permitted for each <govuk-accordion-item>.", ex.Message);
    }
}
