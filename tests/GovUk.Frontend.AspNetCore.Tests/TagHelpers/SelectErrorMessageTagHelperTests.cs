using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectErrorMessageTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_SetsErrorMessageOnContext()
    {
        // Arrange
        var selectContext = new SelectContext(@for: null);

        var context = new TagHelperContext(
            tagName: SelectErrorMessageTagHelper.TagName,
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        context.SetScopedContextItem(typeof(FormGroupContext3), selectContext);

        var output = new TagHelperOutput(
            SelectErrorMessageTagHelper.TagName,
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Error message");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SelectErrorMessageTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(selectContext.ErrorMessage);
        Assert.Equal("Error message", selectContext.ErrorMessage.Html?.ToHtmlString());
    }
}
