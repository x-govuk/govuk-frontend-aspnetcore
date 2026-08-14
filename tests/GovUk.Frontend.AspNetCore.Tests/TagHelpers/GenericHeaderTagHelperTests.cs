using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class GenericHeaderTagHelperTests : TagHelperTestBase<GenericHeaderTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var homePageUrl = "https://example.com";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var containerAttributes = CreateDummyDataAttributes();
        var logoContent = "Logo content";
        var logoAttributes = new AttributeCollection(CreateDummyDataAttributes());
        var linkAttributes = new AttributeCollection(CreateDummyDataAttributes());
        var content = "Additional content";

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var headerContext = context.GetContextItem<GenericHeaderContext>();

                headerContext.SetLogo(
                    new HtmlString(logoContent),
                    logoAttributes,
                    linkAttributes);

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<GenericHeaderOptions>(nameof(IComponentGenerator.GenerateGenericHeaderAsync));

        var tagHelper = new GenericHeaderTagHelper(componentGenerator)
        {
            HomePageUrl = homePageUrl,
            ContainerAttributes = containerAttributes
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Equal(homePageUrl, actualOptions.Url);
        Assert.Equal(logoContent, actualOptions.LogoHtml?.ToHtmlString());
        Assert.Same(logoAttributes, actualOptions.LogoAttributes);
        Assert.Same(linkAttributes, actualOptions.LinkAttributes);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        AssertContainsAttributes(containerAttributes, actualOptions.ContainerAttributes);
        Assert.Equal(content, actualOptions.Html?.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_MissingLogo_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, _) = CreateComponentGenerator<GenericHeaderOptions>(nameof(IComponentGenerator.GenerateGenericHeaderAsync));

        var tagHelper = new GenericHeaderTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A <{GenericHeaderLogoTagHelper.TagName}> or <{GenericHeaderLogoTagHelper.ShortTagName}> element must be provided.",
            ex.Message);
    }
}
