using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class InsetTextTagHelperTests : TagHelperTestBase<InsetTextTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "Inset text";
        var id = "id";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        InsetTextOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateInsetTextAsync(It.IsAny<InsetTextOptions>())).Callback<InsetTextOptions>(o => actualOptions = o);

        var tagHelper = new InsetTextTagHelper(componentGeneratorMock.Object)
        {
            Id = id
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(content, actualOptions!.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(id, actualOptions.Id);
    }
}
