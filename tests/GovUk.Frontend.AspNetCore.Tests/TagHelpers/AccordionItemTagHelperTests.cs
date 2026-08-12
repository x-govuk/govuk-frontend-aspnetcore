using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionItemTagHelperTests : TagHelperTestBase<AccordionItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = CreateTagHelperContext(contexts: accordionContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetHeading(new AttributeCollection(), new TemplateString("Heading"), AccordionItemHeadingTagHelper.TagName);
                itemContext.SetSummary(new AttributeCollection(), new TemplateString("Summary"), AccordionItemSummaryTagHelper.TagName);
                itemContext.SetContent(new AttributeCollection(), new TemplateString("Content"), AccordionItemContentTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(accordionContext.Items);
        Assert.NotNull(item.Heading);
        Assert.Equal("Heading", item.Heading.Html?.ToHtmlString());
        Assert.NotNull(item.Summary);
        Assert.Equal("Summary", item.Summary.Html?.ToHtmlString());
        Assert.NotNull(item.Content);
        Assert.Equal("Content", item.Content.Html?.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_NoHeading_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = CreateTagHelperContext(contexts: accordionContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetSummary(new AttributeCollection(), new TemplateString("Summary"), AccordionItemSummaryTagHelper.TagName);
                itemContext.SetContent(new AttributeCollection(), new TemplateString("Content"), AccordionItemContentTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A <{AccordionItemHeadingTagHelper.TagName}> or <{AccordionItemHeadingTagHelper.ShortTagName}> element must be provided.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_NoContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = CreateTagHelperContext(contexts: accordionContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetHeading(new AttributeCollection(), new TemplateString("Heading"), AccordionItemHeadingTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A <{AccordionItemContentTagHelper.TagName}> or <{AccordionItemContentTagHelper.ShortTagName}> element must be provided.",
            ex.Message);
    }
}
