using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesFieldsetTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsFieldsetToContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<CheckboxesFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new HtmlString("Legend"), CheckboxesFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesFieldsetTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(checkboxesContext.Fieldset?.Legend?.IsPageHeading);
        Assert.Equal("Legend", checkboxesContext.Fieldset?.Legend?.Html?.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var checkboxesFieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        checkboxesContext.OpenFieldset(checkboxesFieldsetContext, new AttributeCollection());
        checkboxesFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new HtmlString("Existing legend"), CheckboxesFieldsetLegendTagHelper.TagName);
        checkboxesContext.CloseFieldset();

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<CheckboxesFieldsetContext>();
                fieldsetContext.SetLegend(isPageHeading: true, attributes: new AttributeCollection(), html: new HtmlString("Legend"), CheckboxesFieldsetLegendTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesFieldsetTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() =>
        {
            tagHelper.Init(context);
            return tagHelper.ProcessAsync(context, output);
        });

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-checkboxes-fieldset> element is permitted within each <govuk-checkboxes>.", ex.Message);
    }
}
