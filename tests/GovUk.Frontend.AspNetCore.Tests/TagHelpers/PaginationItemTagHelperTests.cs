using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationItemTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContextItems()
    {
        // Arrange
        var paginationContext = new PaginationContext();

        var context = new TagHelperContext(
            tagName: "govuk-pagination-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(PaginationContext), paginationContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-pagination-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml("Page 42");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PaginationItemTagHelper()
        {
            Current = true
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            paginationContext.Items,
            item =>
            {
                var paginationItem = Assert.IsType<PaginationOptionsItem>(item);
                Assert.True(paginationItem.Current);
                Assert.Equal("Page 42", paginationItem.Number?.ToHtmlString(HtmlEncoder.Default));
            });
    }
}
