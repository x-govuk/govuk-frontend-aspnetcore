using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectLabelTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_SetsLabelOnContext()
    {
        // Arrange
        var selectContext = new SelectContext(@for: null);

        var context = new TagHelperContext(
            tagName: SelectLabelTagHelper.TagName,
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        context.SetScopedContextItem(typeof(FormGroupContext3), selectContext);

        var output = new TagHelperOutput(
            SelectLabelTagHelper.TagName,
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Label");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectLabelTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.Label);
        Assert.Equal("Label", selectContext.Label.Html?.ToHtmlString());
    }
}
