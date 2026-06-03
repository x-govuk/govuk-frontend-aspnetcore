using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationNextTagHelperTests : TagHelperTestBase<PaginationNextTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsNextOnContext()
    {
        // Arrange
        var paginationContext = new PaginationContext();

        var context = CreateTagHelperContext(contexts: paginationContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PaginationNextTagHelper()
        {
            LabelText = "Next page"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Next page", paginationContext.Next?.LabelText);
    }
}
