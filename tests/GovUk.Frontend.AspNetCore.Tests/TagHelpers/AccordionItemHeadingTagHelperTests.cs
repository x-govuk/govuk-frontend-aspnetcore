using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionItemHeadingTagHelperTests : TagHelperTestBase<AccordionItemHeadingTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsHeadingToContext()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext(ParentTagName!);

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Summary content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemHeadingTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Heading);
        Assert.Equal("Summary content", itemContext.Heading?.Content.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ItemAlreadyHasHeader_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext(ParentTagName!);
        itemContext.SetHeading(new AttributeCollection(), content: new TemplateString("Existing heading"), TagName);

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Summary content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemHeadingTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ItemAlreadyHasSummary_ThrowsInvalidOperationException()
    {
        // Arrange
        var summaryTagName = GetSiblingTagName(
            AccordionItemSummaryTagHelper.TagName,
            AccordionItemSummaryTagHelper.ShortTagName);

        var accordionContext = new AccordionContext();
        var itemContext = new AccordionItemContext(ParentTagName!);
        itemContext.SetSummary(attributes: new AttributeCollection(), content: new TemplateString("Summary"), summaryTagName);

        var context = CreateTagHelperContext(contexts: [accordionContext, itemContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Summary content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemHeadingTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{summaryTagName}>.", ex.Message);
    }
}
