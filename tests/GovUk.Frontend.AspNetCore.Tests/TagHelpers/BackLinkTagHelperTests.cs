using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class BackLinkTagHelperTests : TagHelperTestBase<BackLinkTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var href = "http://foo.com";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);
        var content = "My custom link content";

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<BackLinkOptions>(nameof(IComponentGenerator.GenerateBackLinkAsync));

        var tagHelper = new BackLinkTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(content, actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(href, actualOptions.Href);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes, except: "href");
    }
}
