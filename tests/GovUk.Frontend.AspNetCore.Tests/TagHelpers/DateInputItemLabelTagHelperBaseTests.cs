using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public abstract class DateInputItemLabelTagHelperBaseTests<T> : TagHelperTestBase<T>
    where T : DateInputItemLabelTagHelperBase, new()
{
    [Fact]
    public async Task ProcessAsync_SetsLabelOnContext()
    {
        // Arrange
        var labelContent = "Label";
        var attributes = CreateDummyDataAttributes();

        var itemContext = new DateInputItemContext(ParentTagName!, TagName);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: itemContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(labelContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new T();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(itemContext.Label);
        Assert.Equal(labelContent, itemContext.Label?.Html);
        AssertContainsAttributes(attributes, itemContext.Label?.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_AlreadyGotLabel_ThrowsInvalidOperationException()
    {
        // Arrange
        var itemContext = new DateInputItemContext(ParentTagName!, TagName);
        itemContext.SetLabel(html: new TemplateString("Existing label"), attributes: [], TagName);

        var context = CreateTagHelperContext(contexts: itemContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("New label");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new T();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{ParentTagName}>.", ex.Message);
    }
}
