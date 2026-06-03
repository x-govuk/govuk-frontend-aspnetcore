using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemConditionalTagHelperTests : TagHelperTestBase<CheckboxesItemConditionalTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsConditionalOnContext()
    {
        // Arrange
        var checkboxesItemContext = new CheckboxesItemContext();

        var context = CreateTagHelperContext(contexts: checkboxesItemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Conditional");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemConditionalTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Conditional", checkboxesItemContext.Conditional?.Options.Html?.ToHtmlString());
    }
}
