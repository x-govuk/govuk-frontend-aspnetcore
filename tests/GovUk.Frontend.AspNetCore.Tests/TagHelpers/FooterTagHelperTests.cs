using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterTagHelperTests() : TagHelperTestBase(FooterTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var containerClass = CreateDummyClassName();
        var metaOptions = new FooterOptionsMeta();
        var navigation = new FooterOptionsNavigation();
        var contentLicence = new FooterOptionsContentLicence();
        var copyright = new FooterOptionsCopyright();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(attributes: attributes, className: className);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            className: className,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var footerContext = context.GetContextItem<FooterContext>();
                footerContext.Meta = (metaOptions, FooterMetaTagHelper.TagName);
                footerContext.Navigation.Add(navigation);
                footerContext.ContentLicence = (contentLicence, FooterContentLicenceTagHelper.TagName);
                footerContext.Copyright = (copyright, FooterCopyrightTagHelper.TagName);

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<FooterOptions>(nameof(IComponentGenerator.GenerateFooterAsync));

        var tagHelper = new FooterTagHelper(componentGenerator, HtmlEncoder.Default)
        {
            ContainerClass = containerClass
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Same(metaOptions, actualOptions.Meta);
        Assert.NotNull(actualOptions.Navigation);
        Assert.Collection(actualOptions.Navigation, nav => Assert.Same(nav, navigation));
        Assert.Same(contentLicence, actualOptions.ContentLicence);
        Assert.Same(copyright, actualOptions.Copyright);
        Assert.Equal(containerClass, actualOptions.ContainerClasses);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }
}
