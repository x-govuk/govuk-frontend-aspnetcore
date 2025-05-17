using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputFieldsetTagHelperTests() : TagHelperTestBase(DateInputFieldsetTagHelper.TagName, parentTagName: DateInputTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsFieldsetToContext()
    {
        // Arrange
        var legendContent = "Legend";
        var isPageHeading = true;

        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        var context = CreateTagHelperContext(contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<DateInputFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading, attributes: new(), html: legendContent);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(dateInputContext.Fieldset?.Legend?.IsPageHeading);
        Assert.Equal(legendContent, dateInputContext.Fieldset?.Legend?.Html);
        Assert.Equal(isPageHeading, dateInputContext.Fieldset?.Legend?.IsPageHeading);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        dateInputContext.OpenFieldset();
        var checkboxesFieldsetContext = new DateInputFieldsetContext(describedBy: null, attributes: new(), @for: null);
        checkboxesFieldsetContext.SetLegend(isPageHeading: false, attributes: new(), html: new HtmlString("Existing legend"));
        dateInputContext.CloseFieldset(checkboxesFieldsetContext);

        var context = CreateTagHelperContext(contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<DateInputFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new(), html: "New legend");

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
