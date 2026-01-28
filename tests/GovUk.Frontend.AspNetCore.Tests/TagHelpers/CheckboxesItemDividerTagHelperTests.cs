using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemDividerTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsDividerToContextItems()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: null, aspFor: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-divider",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-divider",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString("Divider"));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemDividerTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var dividerItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal("Divider", dividerItem.Divider?.ToHtmlString());
            });
    }
}
