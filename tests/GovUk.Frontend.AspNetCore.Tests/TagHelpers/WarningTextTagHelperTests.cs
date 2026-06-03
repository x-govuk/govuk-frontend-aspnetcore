using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class WarningTextTagHelperTests : TagHelperTestBase<WarningTextTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var iconFallbackText = "Danger";
        var content = "Warning message";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        WarningTextOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateWarningTextAsync(It.IsAny<WarningTextOptions>())).Callback<WarningTextOptions>(o => actualOptions = o);

        var tagHelper = new WarningTextTagHelper(componentGeneratorMock.Object, HtmlEncoder.Default)
        {
            IconFallbackText = iconFallbackText
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(content, actualOptions!.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(iconFallbackText, actualOptions.IconFallbackText);
    }
}
