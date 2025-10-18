using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryCardSummaryListTagHelperTests : TagHelperTestBase<SummaryCardSummaryListTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHaveCardOnContext()
    {
        // Arrange
        var summaryListContext = new SummaryListContext();

        var context = CreateTagHelperContext(contexts: summaryListContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new SummaryCardSummaryListTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(summaryListContext.HaveCard);
    }
}
