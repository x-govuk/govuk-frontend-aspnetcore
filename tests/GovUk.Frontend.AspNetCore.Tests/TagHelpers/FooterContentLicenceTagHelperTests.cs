using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterContentLicenceTagHelperTests() : TagHelperTestBase(FooterContentLicenceTagHelper.TagName, FooterTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsContentLicenceOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();
        var content = "ContentLicence content";

        var footerContext = new FooterContext();

        var context = CreateTagHelperContext(attributes: attributes, contexts: footerContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterContentLicenceTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var contentLicenceOptions = footerContext.ContentLicence?.Options;
        Assert.NotNull(contentLicenceOptions);
        Assert.Equal(content, contentLicenceOptions.Html);
        Assert.Null(contentLicenceOptions.Text);
        AssertContainsAttributes(attributes, contentLicenceOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasContentLicence_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext
        {
            ContentLicence = new(new FooterOptionsContentLicence(), TagName)
        };

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("New content");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterContentLicenceTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasCopyright_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var copyrightTagName = FooterCopyrightTagHelper.TagName;
        footerContext.Copyright = new(new FooterOptionsCopyright(), copyrightTagName);

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Content");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterContentLicenceTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{copyrightTagName}>.", ex.Message);
    }
}
