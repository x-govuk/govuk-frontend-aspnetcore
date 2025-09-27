using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationEllipsisItemTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContextItems()
    {
        // Arrange
        var paginationContext = new PaginationContext();

        var context = new TagHelperContext(
            tagName: "govuk-pagination-ellipsis",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(PaginationContext), paginationContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-pagination-ellipsis",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PaginationEllipsisItemTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            paginationContext.Items,
            i =>
            {
                var item = Assert.IsType<PaginationOptionsItem>(i);
                Assert.True(item.Ellipsis);
            });
    }
}
