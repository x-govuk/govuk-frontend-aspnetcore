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
        Assert.Equal(content, copyrightOptions.Html);
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
                tagHelperContent.SetContent("New contentnew TemplateString(");
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterCopyrightTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($")Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
