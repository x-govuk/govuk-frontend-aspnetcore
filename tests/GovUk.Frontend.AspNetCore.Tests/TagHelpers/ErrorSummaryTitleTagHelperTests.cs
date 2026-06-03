using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ErrorSummaryTitleTagHelperTests : TagHelperTestBase<ErrorSummaryTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsTitleToContext()
    {
        // Arrange
        var titleContent = "Title content";

        var errorSummaryContext = new ErrorSummaryContext();

        var context = CreateTagHelperContext(contexts: errorSummaryContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new ErrorSummaryTitleTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(HtmlEncoder.Default.Encode("Title content"), errorSummaryContext.Title?.Html);
    }
}
