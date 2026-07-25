using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosFieldsetTagHelperTests : TagHelperTestBase<RadiosFieldsetTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsFieldsetToContext()
    {
        // Arrange
        var radiosContext = new RadiosContext(name: null, @for: null);

        var context = CreateTagHelperContext(contexts: radiosContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new TemplateString("Legend"), RadiosFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetTagHelper();

        tagHelper.Init(context);

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

        var radiosFieldsetContext = new FormGroupFieldsetContext2(RadiosFieldsetTagHelper.TagName);
        radiosContext.OpenFieldset(radiosFieldsetContext, new AttributeCollection());
        radiosFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new TemplateString("Existing legend"), RadiosFieldsetLegendTagHelper.TagName);
        radiosContext.CloseFieldset();

        var context = CreateTagHelperContext(contexts: radiosContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new TemplateString("Legend"), RadiosFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() =>
        {
            tagHelper.Init(context);
            return tagHelper.ProcessAsync(context, output);
        });

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-radios-fieldset> element is permitted within each <govuk-radios>.", ex.Message);
    }
}
