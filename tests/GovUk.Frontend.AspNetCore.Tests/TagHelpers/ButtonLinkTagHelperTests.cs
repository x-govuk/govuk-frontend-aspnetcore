using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ButtonLinkTagHelperTests : TagHelperTestBase<ButtonLinkTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "my-button";
        var isStartButton = true;
        var href = "http://foo.com";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);
        var content = "Button text";

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ButtonOptions>(nameof(IComponentGenerator.GenerateButtonAsync));

        var tagHelper = new ButtonLinkTagHelper(componentGenerator)
        {
            Id = id,
            IsStartButton = isStartButton,
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal("a", actualOptions.Element);
        Assert.Equal(HtmlEncoder.Default.Encode(content), actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Null(actualOptions.Name);
        Assert.Null(actualOptions.Type);
        Assert.Null(actualOptions.Value);
        Assert.Null(actualOptions.Disabled);
        Assert.Equal(href, actualOptions.Href);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes, except: "href");
        Assert.Null(actualOptions.PreventDoubleClick);
        Assert.Equal(isStartButton, actualOptions.IsStartButton);
        Assert.Equal(id, actualOptions.Id);
    }
}
