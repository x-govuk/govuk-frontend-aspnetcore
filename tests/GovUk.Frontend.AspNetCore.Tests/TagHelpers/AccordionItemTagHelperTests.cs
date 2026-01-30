using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class AccordionItemTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = new TagHelperContext(
            tagName: "govuk-accordion-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(AccordionContext), accordionContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-accordion-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetHeading(new AttributeCollection(), new TemplateString("Heading"));
                itemContext.SetSummary(new AttributeCollection(), new TemplateString("Summary"));
                itemContext.SetContent(new AttributeCollection(), new TemplateString("Content"));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var item = Assert.Single(accordionContext.Items);
        Assert.NotNull(item.Heading);
        Assert.Equal("Heading", item.Heading.Html);
        Assert.NotNull(item.Summary);
        Assert.Equal("Summary", item.Summary.Html);
        Assert.NotNull(item.Content);
        Assert.Equal("Content", item.Content.Html);
    }

    [Fact]
    public async Task ProcessAsync_NoHeading_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = new TagHelperContext(
            tagName: "govuk-accordion-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(AccordionContext), accordionContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-accordion-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetSummary(new AttributeCollection(), new TemplateString("Summary"));
                itemContext.SetContent(new AttributeCollection(), new TemplateString("Content"));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-accordion-item-heading> element must be provided.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_NoContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var accordionContext = new AccordionContext();

        var context = new TagHelperContext(
            tagName: "govuk-accordion-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(AccordionContext), accordionContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-accordion-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<AccordionItemContext>();
                itemContext.SetHeading(new AttributeCollection(), new TemplateString("Heading"));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new AccordionItemTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-accordion-item-content> element must be provided.", ex.Message);
    }
}
