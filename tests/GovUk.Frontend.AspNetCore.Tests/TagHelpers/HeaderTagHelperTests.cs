using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class HeaderTagHelperTests() : TagHelperTestBase(HeaderTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var homePageUrl = "https://example.com";
        var productName = "Product";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var containerAttributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(className: className, attributes: attributes);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<HeaderOptions>(nameof(IComponentGenerator.GenerateHeaderAsync));

        var tagHelper = new HeaderTagHelper(componentGenerator, HtmlEncoder.Default)
        {
            HomePageUrl = homePageUrl,
            ProductName = productName,
            ContainerAttributes = containerAttributes
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Equal(homePageUrl, actualOptions.HomePageUrl);
        Assert.Equal(productName, actualOptions.ProductName);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        AssertContainsAttributes(containerAttributes, actualOptions.ContainerAttributes);
        Assert.True(actualOptions.UseTudorCrown);
    }
}
