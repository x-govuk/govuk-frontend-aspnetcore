using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosItemDividerTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsDividerToContextItems()
    {
        // Arrange
        var radiosContext = new RadiosContext(name: null, @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-radios-divider",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(RadiosContext), radiosContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios-divider",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml("Divider");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosItemDividerTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            radiosContext.Items,
            item =>
            {
                var divider = Assert.IsType<RadiosOptionsItem>(item);
                Assert.Equal("Divider", divider.Divider?.ToHtmlString());
            });
    }
}
