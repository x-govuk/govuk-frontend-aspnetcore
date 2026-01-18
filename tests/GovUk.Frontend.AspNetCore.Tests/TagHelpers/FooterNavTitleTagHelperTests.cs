using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterNavTitleTagHelperTests : TagHelperTestBase<FooterNavTitleTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var content = "Title";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [footerContext, footerNavContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTitleTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(footerNavContext.Title);
        Assert.Equal(content, footerNavContext.Title?.Html);
        AssertContainsAttributes(attributes, footerNavContext.Title?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext
        {
            Title = new(new TemplateString("Title"), [], TagName)
        };

        var context = CreateTagHelperContext(contexts: [footerContext, footerNavContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("New titlenew TemplateString(");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTitleTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($")Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext();
        var itemsTagName = FooterNavItemsTagHelper.TagName;
        footerNavContext.Items = new([], [], itemsTagName);

        var context = CreateTagHelperContext(contexts: [footerContext, footerNavContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Titlenew TemplateString(");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTitleTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($")<{TagName}> must be specified before <{itemsTagName}>.", ex.Message);
    }
}
