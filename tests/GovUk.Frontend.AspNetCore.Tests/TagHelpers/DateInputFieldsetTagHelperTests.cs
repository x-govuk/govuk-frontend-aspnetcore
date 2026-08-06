using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputFieldsetTagHelperTests : TagHelperTestBase<DateInputFieldsetTagHelper>
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
                var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();
                fieldsetContext.SetLegend(isPageHeading, attributes: [], html: new TemplateString(legendContent), DateInputFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(dateInputContext.Fieldset?.Legend?.IsPageHeading);
        Assert.Equal(legendContent, dateInputContext.Fieldset?.Legend?.Html.ToHtmlString());
        Assert.Equal(isPageHeading, dateInputContext.Fieldset?.Legend?.IsPageHeading);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        var checkboxesFieldsetContext = new FormGroupFieldsetContext2(DateInputFieldsetTagHelper.TagName);
        dateInputContext.OpenFieldset(checkboxesFieldsetContext, []);
        checkboxesFieldsetContext.SetLegend(isPageHeading: false, attributes: [], html: new HtmlString("Existing legend"), DateInputFieldsetLegendTagHelper.TagName);
        dateInputContext.CloseFieldset();

        var context = CreateTagHelperContext(contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: [], html: new TemplateString("New legend"), DateInputFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputFieldsetTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() =>
        {
            tagHelper.Init(context);
            return tagHelper.ProcessAsync(context, output);
        });

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
