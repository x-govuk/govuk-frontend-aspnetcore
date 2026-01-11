using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DetailsTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var summary = new HtmlString("The summary");
                detailsContext.SetSummary(new AttributeCollection(), summary);

                var text = new HtmlString("The text");
                detailsContext.SetText(new AttributeCollection(), text);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator());

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<details class=""govuk-details"">
    <summary class=""govuk-details__summary"">
        <span class=""govuk-details__summary-text"">The summary</span>
    </summary>
    <div class=""govuk-details__text"">The text</div>
</details>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithOpenSpecified_AddsOpenAttributeToOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var summary = new HtmlString("The summary");
                detailsContext.SetSummary(new AttributeCollection(), summary);

                var text = new HtmlString("The text");
                detailsContext.SetText(new AttributeCollection(), text);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator())
        {
            Open = true
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var element = output.RenderToElement();

        Assert.Equal("", element.Attributes["open"].Value);
    }

    [Fact]
    public async Task ProcessAsync_MissingSummary_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator())
        {
            Open = true
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-details-summary> element must be provided.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_MissingText_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var summary = new HtmlString("The summary");
                detailsContext.SetSummary(new AttributeCollection(), summary);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator())
        {
            Open = true
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-details-text> element must be provided.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithSummaryAttributes_IncludesAttributesInOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var summaryAttributes = new AttributeCollection();
                summaryAttributes.Add("data-test", "summary-value");
                var summary = new HtmlString("The summary");
                detailsContext.SetSummary(summaryAttributes, summary);

                var text = new HtmlString("The text");
                detailsContext.SetText(new AttributeCollection(), text);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator());

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var element = output.RenderToElement();
        var summaryElement = element.QuerySelector("summary");
        Assert.NotNull(summaryElement);
        Assert.Equal("summary-value", summaryElement.Attributes["data-test"]?.Value);
    }

    [Fact]
    public async Task ProcessAsync_WithTextAttributes_IncludesAttributesInOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-details",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-details",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var detailsContext = (DetailsContext)context.Items[typeof(DetailsContext)];

                var summary = new HtmlString("The summary");
                detailsContext.SetSummary(new AttributeCollection(), summary);

                var textAttributes = new AttributeCollection();
                textAttributes.Add("data-test", "text-value");
                var text = new HtmlString("The text");
                detailsContext.SetText(textAttributes, text);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTagHelper(TestUtils.CreateComponentGenerator());

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var element = output.RenderToElement();
        var textElement = element.QuerySelector(".govuk-details__text");
        Assert.NotNull(textElement);
        Assert.Equal("text-value", textElement.Attributes["data-test"]?.Value);
    }
}
