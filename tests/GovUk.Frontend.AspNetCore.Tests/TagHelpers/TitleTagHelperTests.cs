using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TitleTagHelperTests : TagHelperTestBase<TitleTagHelper>
{
    [Xunit.Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, true)]
    public async Task ProcessAsync_GeneratesExpectedOutput(
        bool prependErrorToTitleOption,
        bool pageHasErrors,
        bool expectErrorInTitle)
    {
        // Arrange
        var options = Options.Create(new GovUkFrontendOptions()
        {
            PrependErrorToTitle = prependErrorToTitleOption
        });

        var context = CreateTagHelperContext(tagName: "title");

        var output = CreateTagHelperOutput(tagName: "title",
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();
        containerErrorContext.ErrorSummaryHasBeenRendered = pageHasErrors;

        var tagHelper = new TitleTagHelper(options)
        {
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var html = output.RenderToElement();
        var startsWithError = html.InnerHtml.StartsWith("Error: ");

        if (expectErrorInTitle)
        {
            Assert.True(startsWithError);
        }
        else
        {
            Assert.False(startsWithError);
        }
    }
}
