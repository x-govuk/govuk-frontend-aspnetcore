using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
#pragma warning disable GFA0007 // Type or member is obsolete

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ObsoleteSummaryCardTitleTagHelperTests : TagHelperTestBase<ObsoleteSummaryCardTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_DeprecatedTagName_SetsTitleOnContext()
    {
        // Arrange
        var titleContent = "Title";
        var headingLevel = 3;

        var summaryCardContext = new SummaryCardContext();

        var context = CreateTagHelperContext(tagName: ShortTagNames.Title, contexts: summaryCardContext);

        var output = CreateTagHelperOutput(
            tagName: ShortTagNames.Title,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(titleContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new ObsoleteSummaryCardTitleTagHelper()
        {
            HeadingLevel = headingLevel
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(titleContent, summaryCardContext.Title?.Html?.ToHtmlString());
        Assert.Equal(headingLevel, summaryCardContext.Title?.HeadingLevel);
    }
}
