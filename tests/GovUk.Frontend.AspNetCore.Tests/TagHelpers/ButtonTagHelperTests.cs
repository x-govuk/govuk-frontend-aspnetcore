using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ButtonTagHelperTests : TagHelperTestBase<ButtonTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var disabled = true;
        var id = "my-button";
        var isStartButton = true;
        var name = "MyButton";
        var preventDoubleClick = true;
        var type = "button";
        var value = "Value";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
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

        var tagHelper = new ButtonTagHelper(componentGenerator)
        {
            Disabled = disabled,
            Id = id,
            IsStartButton = isStartButton,
            Name = name,
            PreventDoubleClick = preventDoubleClick,
            Type = type,
            Value = value
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal("button", actualOptions.Element);
        Assert.Equal(HtmlEncoder.Default.Encode(content), actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(name, actualOptions.Name);
        Assert.Equal(type, actualOptions.Type);
        Assert.Equal(value, actualOptions.Value);
        Assert.Equal(disabled, actualOptions.Disabled);
        Assert.Null(actualOptions.Href);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        Assert.Equal(preventDoubleClick, actualOptions.PreventDoubleClick);
        Assert.Equal(isStartButton, actualOptions.IsStartButton);
        Assert.Equal(id, actualOptions.Id);
    }
}
