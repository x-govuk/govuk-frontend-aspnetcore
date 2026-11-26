using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ExitThisPageTagHelperTests : TagHelperTestBase<ExitThisPageTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "Leave this page";
        var redirectUrl = "https://www.gov.uk";
        var activatedText = "Activated";
        var timedOutText = "Timed out";
        var pressTwoMoreTimesText = "Press two more times";
        var pressOneMoreTimeText = "Press one more time";
        var id = "exit";
        var classes = "custom-class";
        var dataFooAttribute = "bar";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            attributes: new Dictionary<string, string?>
            {
                { "id", id },
                { "class", classes },
                { "data-foo", dataFooAttribute }
            },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ExitThisPageOptions>(nameof(IComponentGenerator.GenerateExitThisPageAsync));

        var tagHelper = new ExitThisPageTagHelper(componentGenerator)
        {
            RedirectUrl = redirectUrl,
            ActivatedText = activatedText,
            TimedOutText = timedOutText,
            PressTwoMoreTimesText = pressTwoMoreTimesText,
            PressOneMoreTimeText = pressOneMoreTimeText
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Text);
        Assert.Equal(content, actualOptions.Html?.ToHtmlString());
        Assert.Equal(redirectUrl, actualOptions.RedirectUrl);
        Assert.Equal(activatedText, actualOptions.ActivatedText);
        Assert.Equal(timedOutText, actualOptions.TimedOutText);
        Assert.Equal(pressTwoMoreTimesText, actualOptions.PressTwoMoreTimesText);
        Assert.Equal(pressOneMoreTimeText, actualOptions.PressOneMoreTimeText);
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(classes, actualOptions.Classes);
        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(
            actualOptions.Attributes,
            kvp =>
            {
                Assert.Equal("data-foo", kvp.Key);
                Assert.Equal(dataFooAttribute, kvp.Value);
            });
    }

    [Fact]
    public async Task ProcessAsync_NoContent_PassesNullHtml()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ExitThisPageOptions>(nameof(IComponentGenerator.GenerateExitThisPageAsync));

        var tagHelper = new ExitThisPageTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Null(actualOptions.RedirectUrl);
    }
}
