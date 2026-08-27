using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class LanguageNavigationTagHelperTests : TagHelperTestBase<LanguageNavigationTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        LanguageNavigationOptionsItem[] items =
        [
            new LanguageNavigationOptionsItem { Lang = "en", Html = new TemplateString("English") },
            new LanguageNavigationOptionsItem { Lang = "cy", Href = "/cy", Html = new TemplateString("Cymraeg") }
        ];

        var ariaLabel = "Dewis iaith";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var languageNavigationContext = context.GetContextItem<LanguageNavigationContext>();

                foreach (var item in items)
                {
                    languageNavigationContext.AddItem(item);
                }

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<LanguageNavigationOptions>(nameof(IComponentGenerator.GenerateLanguageNavigationAsync));

        var tagHelper = new LanguageNavigationTagHelper(componentGenerator)
        {
            AriaLabel = ariaLabel
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(ariaLabel, actualOptions.AriaLabel);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
        Assert.NotNull(actualOptions.Items);
        Assert.Collection(
            actualOptions.Items,
            item => Assert.Same(items[0], item),
            item => Assert.Same(items[1], item));
    }
}
