using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class GenericHeaderLogoTagHelperTests : TagHelperTestBase<GenericHeaderLogoTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsLogoToContext()
    {
        // Arrange
        var headerContext = new GenericHeaderContext();

        var logoClass = CreateDummyClassName();
        var logoAttributes = CreateDummyDataAttributes();
        var linkDataAttribute = "link-value";

        var context = CreateTagHelperContext(
            className: logoClass,
            attributes: logoAttributes,
            contexts: headerContext);

        var output = CreateTagHelperOutput(
            className: logoClass,
            attributes: logoAttributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Logo content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new GenericHeaderLogoTagHelper()
        {
            LinkAttributes = new Dictionary<string, string?>()
            {
                { "data-link", linkDataAttribute }
            }
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(headerContext.Logo);
        var logo = headerContext.Logo!.Value;
        Assert.Equal("Logo content", logo.Content.ToHtmlString());
        AssertContainsAttributes(logoAttributes, logo.Attributes);
        Assert.Contains(logo.Attributes, a => a.Key == "class" && a.Value == logoClass);
        Assert.Contains(logo.LinkAttributes, a => a.Key == "data-link" && a.Value == linkDataAttribute);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasLogo_ThrowsInvalidOperationException()
    {
        // Arrange
        var headerContext = new GenericHeaderContext();

        headerContext.SetLogo(
            new HtmlString("Existing logo"),
            new AttributeCollection(),
            new AttributeCollection());

        var context = CreateTagHelperContext(contexts: headerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Logo content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new GenericHeaderLogoTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }
}
