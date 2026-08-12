using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FormGroupItemHintTagHelperTests : TagHelperTestBase<FormGroupItemHintTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        FormGroupItemContext itemContext = TagName == FormGroupItemHintTagHelper.RadiosTagName ?
            new RadiosItemContext() :
            new CheckboxesItemContext();

        var context = CreateTagHelperContext(contexts: itemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Hint");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new FormGroupItemHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Hint", itemContext.Hint?.Options.Html?.ToHtmlString());
    }
}
