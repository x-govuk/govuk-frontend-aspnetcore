using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosErrorMessageTagHelperTests : TagHelperTestBase<RadiosErrorMessageTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsErrorMessageOnContext()
    {
        // Arrange
        var errorContent = "Error message";
        var visuallyHiddenText = "Error:";
        var attributes = CreateDummyDataAttributes();

        var radiosContext = new RadiosContext(name: null, @for: null);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: radiosContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(errorContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosErrorMessageTagHelper()
        {
            VisuallyHiddenText = visuallyHiddenText
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(radiosContext.ErrorMessage);
        Assert.Equal(errorContent, radiosContext.ErrorMessage?.Html?.ToHtmlString());
        Assert.Equal(visuallyHiddenText, radiosContext.ErrorMessage?.VisuallyHiddenText);
        AssertContainsAttributes(attributes, radiosContext.ErrorMessage?.Attributes);
    }
}
