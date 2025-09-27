using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputFieldsetLegendTagHelperTests() :
    TagHelperTestBase(DateInputFieldsetLegendTagHelper.TagName, parentTagName: DateInputFieldsetTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsLegendToContext()
    {
        // Arrange
        var legendContent = "Legend";
        var isPageHeading = true;
        var attributes = CreateDummyDataAttributes();

        var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: [], @for: null);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: fieldsetContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(legendContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetLegendTagHelper()
        {
            IsPageHeading = isPageHeading
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(fieldsetContext.Legend);
        Assert.Equal(legendContent, fieldsetContext.Legend.Html);
        Assert.Equal(isPageHeading, fieldsetContext.Legend.IsPageHeading);
        AssertContainsAttributes(attributes, fieldsetContext.Legend.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasLegend_ThrowsInvalidOperationException()
    {
        // Arrange
        var fieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: [], @for: null);

        fieldsetContext.SetLegend(isPageHeading: false, attributes: [], html: "Existing legend");

        var context = CreateTagHelperContext(contexts: fieldsetContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Legend content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetLegendTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
