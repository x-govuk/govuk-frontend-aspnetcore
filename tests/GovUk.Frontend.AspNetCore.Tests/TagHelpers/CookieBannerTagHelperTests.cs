using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CookieBannerTagHelperTests : TagHelperTestBase<CookieBannerTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var arialLabel = "label";
        var hidden = true;
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var message = new CookieBannerOptionsMessage();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var cookieBannerContext = context.GetContextItem<CookieBannerContext>();
                cookieBannerContext.Messages.Add(message);

                TagHelperContent content = new DefaultTagHelperContent();
                return Task.FromResult(content);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<CookieBannerOptions>(nameof(IComponentGenerator.GenerateCookieBannerAsync));

        var tagHelper = new CookieBannerTagHelper(componentGenerator, HtmlEncoder.Default)
        {
            AriaLabel = arialLabel,
            Hidden = hidden
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions);
        Assert.Equal(hidden, actualOptions.Hidden);
        Assert.Equal(className, actualOptions.Classes);
        Assert.Equal(arialLabel, actualOptions.AriaLabel);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        Assert.NotNull(actualOptions.Messages);
        Assert.Collection(actualOptions.Messages, m => Assert.Same(message, m));
    }
}
