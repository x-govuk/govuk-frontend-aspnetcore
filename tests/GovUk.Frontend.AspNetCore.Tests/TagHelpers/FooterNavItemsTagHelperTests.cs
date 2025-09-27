using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FooterNavItemsTagHelperTests() : TagHelperTestBase(FooterNavItemsTagHelper.TagName, FooterNavTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsItemsOnContext()
    {
        // Arrange
        var attributes = CreateDummyDataAttributes();

        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext();

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: [footerContext, footerNavContext]);

        var output = CreateTagHelperOutput(attributes: attributes);

        var tagHelper = new FooterNavItemsTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(footerNavContext.Items);
        AssertContainsAttributes(attributes, footerNavContext.Items?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasItems_ThrowsInvalidOperationException()
    {
        // Arrange
        var footerContext = new FooterContext();
        var footerNavContext = new FooterNavContext
        {
            Items = new([], [], TagName)
        };

        var context = CreateTagHelperContext(contexts: [footerContext, footerNavContext]);

        var output = CreateTagHelperOutput();

        var tagHelper = new FooterNavItemsTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
