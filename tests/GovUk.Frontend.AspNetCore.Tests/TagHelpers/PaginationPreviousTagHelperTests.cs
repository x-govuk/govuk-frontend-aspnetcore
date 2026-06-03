using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PaginationPreviousTagHelperTests : TagHelperTestBase<PaginationPreviousTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsPreviousOnContext()
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

        var tagHelper = new PaginationPreviousTagHelper()
        {
            LabelText = "Previous page"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Previous page", paginationContext.Previous?.LabelText);
    }
}
