using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class HeaderTagHelperTests : TagHelperTestBase<HeaderTagHelper>
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

        var options = CreateOptions();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<HeaderOptions>(nameof(IComponentGenerator.GenerateHeaderAsync));

        var tagHelper = new HeaderTagHelper(componentGenerator, options, HtmlEncoder.Default)
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ProcessAsync_PassesRebrandFromOptions(bool rebrand)
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var options = CreateOptions(options => options.Rebrand = rebrand);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<HeaderOptions>(nameof(IComponentGenerator.GenerateHeaderAsync));

        var tagHelper = new HeaderTagHelper(componentGenerator, options, HtmlEncoder.Default);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Equal(rebrand, actualOptions.Rebrand);
    }
}
