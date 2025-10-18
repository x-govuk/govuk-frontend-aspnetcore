using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PhaseBannerTagHelperTests : TagHelperTestBase<PhaseBannerTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var content = "Content";
        var tagOptions = new TagOptions()
        {
            Text = null,
            Html = "Tag",
            Classes = null,
            Attributes = null
        };
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var phaseBannerContext = context.GetContextItem<PhaseBannerContext>();

                phaseBannerContext.SetTag(tagOptions, PhaseBannerTagHelper.TagName);

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<PhaseBannerOptions>(nameof(IComponentGenerator.GeneratePhaseBannerAsync));

        var tagHelper = new PhaseBannerTagHelper(componentGenerator, HtmlEncoder.Default);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Text);
        Assert.Equal(content, actualOptions.Html);
        Assert.Same(tagOptions, actualOptions.Tag);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_MissingTag_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Content";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<PhaseBannerOptions>(nameof(IComponentGenerator.GeneratePhaseBannerAsync));

        var tagHelper = new PhaseBannerTagHelper(componentGenerator, HtmlEncoder.Default);

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"A <{PhaseBannerTagTagHelper.TagName}> element must be provided.", ex.Message);
    }
}
