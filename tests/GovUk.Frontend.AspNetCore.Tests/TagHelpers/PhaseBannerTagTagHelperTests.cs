using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PhaseBannerTagTagHelperTests : TagHelperTestBase<PhaseBannerTagTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsToContext()
    {
        // Arrange
        var content = "Tag";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var phaseBannerContext = new PhaseBannerContext();

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: phaseBannerContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new PhaseBannerTagTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(phaseBannerContext.Tag);
        Assert.Equal(TagName, phaseBannerContext.Tag?.TagName);
        Assert.Equal(content, phaseBannerContext.Tag?.Options.Html);
        Assert.Equal(className, phaseBannerContext.Tag?.Options.Classes);
        AssertContainsAttributes(attributes, phaseBannerContext.Tag?.Options.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_TagAlreadySpecified_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Tag";
        var phaseBannerContext = new PhaseBannerContext();
        phaseBannerContext.SetTag(new TagOptions(), PhaseBannerTagHelper.TagName);

        var context = CreateTagHelperContext(contexts: phaseBannerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new PhaseBannerTagTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
