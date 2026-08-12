using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FormGroupItemConditionalTagHelperTests : TagHelperTestBase<FormGroupItemConditionalTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsConditionalOnContext()
    {
        // Arrange
        FormGroupItemContext itemContext = TagName == FormGroupItemConditionalTagHelper.CheckboxesTagName ?
            new CheckboxesItemContext() :
            new RadiosItemContext();

        var context = CreateTagHelperContext(contexts: itemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Conditional");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new FormGroupItemConditionalTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Conditional", itemContext.Conditional?.Html?.ToHtmlString());
    }
}
