using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesErrorMessageTagHelperTests : TagHelperTestBase<CheckboxesErrorMessageTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsErrorMessageOnContext()
    {
        // Arrange
        var errorContent = "Error message";
        var visuallyHiddenText = "Error:";
        var attributes = CreateDummyDataAttributes();

        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: checkboxesContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(errorContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesErrorMessageTagHelper()
        {
            VisuallyHiddenText = visuallyHiddenText
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(checkboxesContext.ErrorMessage);
        Assert.Equal(errorContent, checkboxesContext.ErrorMessage?.Html?.ToHtmlString());
        Assert.Equal(visuallyHiddenText, checkboxesContext.ErrorMessage?.VisuallyHiddenText);
        AssertContainsAttributes(attributes, checkboxesContext.ErrorMessage?.Attributes);
    }
}
