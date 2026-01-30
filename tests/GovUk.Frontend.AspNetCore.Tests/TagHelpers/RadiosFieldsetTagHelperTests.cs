using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosFieldsetTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsFieldsetToContext()
    {
        // Arrange
        var radiosContext = new RadiosContext(name: null, @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-radios-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(RadiosContext), radiosContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<RadiosFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new TemplateString("Legend"));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(radiosContext.Fieldset?.Legend?.IsPageHeading);
        Assert.Equal("Legend", radiosContext.Fieldset?.Legend?.Html?.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var radiosContext = new RadiosContext(name: null, @for: null);

        radiosContext.OpenFieldset();
        var radiosFieldsetContext = new RadiosFieldsetContext(describedBy: null, attributes: new AttributeCollection(), @for: null);
        radiosFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new TemplateString("Existing legend"));
        radiosContext.CloseFieldset(radiosFieldsetContext);

        var context = new TagHelperContext(
            tagName: "govuk-radios-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(RadiosContext), radiosContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<RadiosFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new TemplateString("Legend"));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-radios-fieldset> element is permitted within each <govuk-radios>.", ex.Message);
    }
}
