using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DetailsTextTagHelperTests : TagHelperTestBase<DetailsTextTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsContentOnContext()
    {
        // Arrange
        var detailsContext = new DetailsContext();
        detailsContext.SetSummary(
            [],
            new HtmlString("The summary"),
            GetSiblingTagName(DetailsSummaryTagHelper.TagName, DetailsSummaryTagHelper.ShortTagName));

        var context = CreateTagHelperContext(contexts: detailsContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The text");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTextTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("The text", detailsContext.Text?.Content.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasText_ThrowsInvalidOperationException()
    {
        // Arrange
        var detailsContext = new DetailsContext();
        detailsContext.SetSummary(
            [],
            new HtmlString("The summary"),
            GetSiblingTagName(DetailsSummaryTagHelper.TagName, DetailsSummaryTagHelper.ShortTagName));
        detailsContext.SetText([], new HtmlString("The text"), TagName);

        var context = CreateTagHelperContext(contexts: detailsContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The text");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DetailsTextTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }
}
