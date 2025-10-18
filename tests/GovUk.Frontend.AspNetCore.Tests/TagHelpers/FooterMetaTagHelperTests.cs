using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterMetaTagHelperTests : TagHelperTestBase<FooterMetaTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsMetaOnContext()
    {
        // Arrange
        var visuallyHiddenTitle = "VisuallyHiddenTitle";
        var content = "Content";
        var contentAttributes = CreateDummyDataAttributes();
        var itemsAttributes = CreateDummyDataAttributes();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var item = new FooterOptionsMetaItem();

        var footerContext = new FooterContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: footerContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var metaContext = context.GetContextItem<FooterMetaContext>();
                metaContext.Content = new(content, new(contentAttributes), FooterMetaContentTagHelper.TagName);
                metaContext.Items = new([item], new(itemsAttributes), FooterMetaItemsTagHelper.TagName);

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaTagHelper()
        {
            VisuallyHiddenTitle = visuallyHiddenTitle
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var footerMetaOptions = footerContext.Meta?.Options;
        Assert.NotNull(footerMetaOptions);
        Assert.Equal(visuallyHiddenTitle, footerMetaOptions.VisuallyHiddenTitle);
        Assert.Null(footerMetaOptions.Text);
        Assert.Equal(content, footerMetaOptions.Html);
        AssertContainsAttributes(attributes, footerMetaOptions.Attributes);
        Assert.NotNull(footerMetaOptions.Items);
        Assert.Collection(footerMetaOptions.Items, i => Assert.Same(item, i));
        AssertContainsAttributes(contentAttributes, footerMetaOptions.ContentAttributes);
        AssertContainsAttributes(itemsAttributes, footerMetaOptions.ItemsAttributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasMeta_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext
        {
            Meta = new(new FooterOptionsMeta(), TagName)
        };

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasContentLicence_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var contentLicenceTagName = FooterContentLicenceTagHelper.TagName;
        footerContext.ContentLicence = new(new FooterOptionsContentLicence(), contentLicenceTagName);

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{contentLicenceTagName}>.", ex.Message);
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
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterMetaTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{copyrightTagName}>.", ex.Message);
    }
}
