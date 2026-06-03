using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosFieldsetLegendTagHelperTests : TagHelperTestBase<RadiosFieldsetLegendTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsLegendToContext()
    {
        // Arrange
        var fieldsetContext = new RadiosFieldsetContext(describedBy: null, @for: null);

        var context = CreateTagHelperContext(contexts: fieldsetContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Legend content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetLegendTagHelper()
        {
            IsPageHeading = true
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Legend content", fieldsetContext.Legend?.Html?.ToHtmlString());
        Assert.True(fieldsetContext.Legend?.IsPageHeading);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasLegend_ThrowsInvalidOperationException()
    {
        // Arrange
        var fieldsetContext = new RadiosFieldsetContext(describedBy: null, @for: null);

        fieldsetContext.SetLegend(
            isPageHeading: false,
            attributes: new AttributeCollection(),
            html: new TemplateString("Existing legend"),
            RadiosFieldsetLegendTagHelper.TagName);

        var context = CreateTagHelperContext(contexts: fieldsetContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Legend content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosFieldsetLegendTagHelper()
        {
            IsPageHeading = true
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-radios-fieldset-legend> element is permitted within each <govuk-radios-fieldset>.", ex.Message);
    }
}
