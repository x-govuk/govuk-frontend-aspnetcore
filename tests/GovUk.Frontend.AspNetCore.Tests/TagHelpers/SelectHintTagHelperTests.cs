using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectHintTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        var selectContext = new SelectContext(aspFor: null);

        var context = new TagHelperContext(
            tagName: SelectHintTagHelper.TagName,
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        context.SetScopedContextItem(typeof(FormGroupContext3), selectContext);

        var output = new TagHelperOutput(
            SelectHintTagHelper.TagName,
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hint");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.Hint);
        Assert.Equal("Hint", selectContext.Hint.Html?.ToHtmlString());
    }
}
