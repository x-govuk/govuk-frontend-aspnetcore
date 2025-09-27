using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterMetaItemsTagHelperTests() : TagHelperTestBase(FooterMetaItemsTagHelper.TagName, FooterMetaTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetItemsOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();

        var footerContext = new FooterContext();
        var footerMetaContext = new FooterMetaContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [footerContext, footerMetaContext]);

        var output = CreateTagHelperOutput(attributes: attributes);

        var tagHelper = new FooterMetaItemsTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(footerMetaContext.Items);
        AssertContainsAttributes(attributes, footerMetaContext.Items?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerMetaContext = new FooterMetaContext
        {
            Items = new([], [], TagName)
        };

        var context = CreateTagHelperContext(contexts: [footerContext, footerMetaContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new FooterMetaItemsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasContent_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerMetaContext = new FooterMetaContext();
        var contentTagName = FooterMetaContentTagHelper.TagName;
        footerMetaContext.Content = new("Content", [], contentTagName);

        var context = CreateTagHelperContext(contexts: [footerContext, footerMetaContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new FooterMetaItemsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{contentTagName}>.", ex.Message);
    }
}
