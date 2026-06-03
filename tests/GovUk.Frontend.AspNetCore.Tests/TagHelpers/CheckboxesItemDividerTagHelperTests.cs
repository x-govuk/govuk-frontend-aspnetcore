using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemDividerTagHelperTests : TagHelperTestBase<CheckboxesItemDividerTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsDividerToContextItems()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var context = CreateTagHelperContext(contexts: checkboxesContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString("Divider"));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemDividerTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var dividerItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal("Divider", dividerItem.Divider?.ToHtmlString());
            });
    }
}
