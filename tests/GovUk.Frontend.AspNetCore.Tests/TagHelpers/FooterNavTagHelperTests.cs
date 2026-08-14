using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterNavTagHelperTests : TagHelperTestBase<FooterNavTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsNavToContext()
    {
        // Arrange
        var titleContent = "Title";
        var columns = 2;
        var width = "two-thirds";
        var titleAttributes = CreateDummyDataAttributes();
        var itemsAttributes = CreateDummyDataAttributes();
        var attributes = CreateDummyDataAttributes();
        var item = new FooterOptionsNavigationItem();

        var footerContext = new FooterContext();

        var context = CreateTagHelperContext(attributes: attributes, contexts: footerContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var footerNavContext = context.GetContextItem<FooterNavContext>();
                footerNavContext.Title = new(new TemplateString(titleContent), new(titleAttributes), FooterNavTitleTagHelper.TagName);
                footerNavContext.Items = new([item], new(itemsAttributes), FooterNavItemsTagHelper.TagName);

                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTagHelper()
        {
            Columns = columns,
            Width = width
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            footerContext.Navigation,
            nav =>
            {
                Assert.Equal(columns, nav.Columns);
                Assert.Equal(width, nav.Width);
                Assert.Equal(titleContent, nav.Title?.ToHtmlString());
                AssertContainsAttributes(titleAttributes, nav.TitleAttributes);
                Assert.NotNull(nav.Items);
                Assert.Collection(
                    nav.Items,
                    i => Assert.Same(item, i));
                AssertContainsAttributes(itemsAttributes, nav.ItemsAttributes);
                AssertContainsAttributes(attributes, nav.Attributes);
            });
    }

    [Fact]
    public async Task ProcessAsync_ParentHasMeta_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var metaTagName = GetSiblingTagName(
            FooterMetaTagHelper.TagName,
            FooterMetaTagHelper.ShortTagName);
        footerContext.Meta = new(new FooterOptionsMeta(), metaTagName);

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{metaTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasContentLicence_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var contentLicenceTagName = GetSiblingTagName(
            FooterContentLicenceTagHelper.TagName,
            FooterContentLicenceTagHelper.ShortTagName);
        footerContext.ContentLicence = new(new FooterOptionsContentLicence(), contentLicenceTagName);

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTagHelper();

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
        var copyrightTagName = GetSiblingTagName(
            FooterCopyrightTagHelper.TagName,
            FooterCopyrightTagHelper.ShortTagName);
        footerContext.Copyright = new(new FooterOptionsCopyright(), copyrightTagName);

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                TagHelperContent tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult(tagHelperContent);
            });

        var tagHelper = new FooterNavTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{copyrightTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_FooterHasMetaWithOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var metaTagName = GetOtherSpellingSiblingTagName(
            FooterMetaTagHelper.TagName,
            FooterMetaTagHelper.ShortTagName);

        var footerContext = new FooterContext
        {
            Meta = new(new FooterOptionsMeta(), metaTagName)
        };

        var context = CreateTagHelperContext(contexts: footerContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new FooterNavTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{metaTagName}>; short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }

}
