using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterCopyrightTagHelperTests : TagHelperTestBase<FooterCopyrightTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsCopyrightOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();
        var content = "Copyright content";

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

        var tagHelper = new FooterCopyrightTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var copyrightOptions = footerContext.Copyright?.Options;
        Assert.NotNull(copyrightOptions);
        Assert.Equal(content, copyrightOptions.Html?.ToHtmlString());
        Assert.Null(copyrightOptions.Text);
        AssertContainsAttributes(attributes, copyrightOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasCopyright_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext
        {
            Copyright = new(new FooterOptionsCopyright(), TagName)
        };

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("New content");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterCopyrightTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{PrimaryTagName}> or <{ShortTagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_FooterHasContentLicenceWithOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var contentLicenceTagName = GetOtherSpellingSiblingTagName(
            FooterContentLicenceTagHelper.TagName,
            FooterContentLicenceTagHelper.ShortTagName);

        var footerContext = new FooterContext
        {
            ContentLicence = new(new FooterOptionsContentLicence(), contentLicenceTagName)
        };

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var tagHelper = new FooterCopyrightTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{contentLicenceTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_TagModeIsSelfClosing_SetsCopyrightContentToNull()
    {
        // Arrange
        var footerContext = new FooterContext();

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(tagMode: TagMode.SelfClosing);

        var tagHelper = new FooterCopyrightTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var copyrightOptions = footerContext.Copyright?.Options;
        Assert.NotNull(copyrightOptions);
        Assert.Null(copyrightOptions.Html);
        Assert.Null(copyrightOptions.Text);
    }
}
