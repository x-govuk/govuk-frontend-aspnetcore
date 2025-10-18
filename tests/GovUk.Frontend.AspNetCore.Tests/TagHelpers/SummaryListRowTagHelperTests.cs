using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListRowTagHelperTests : TagHelperTestBase<SummaryListRowTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsRowToContext()
    {
        // Arrange
        var key = new SummaryListOptionsRowKey();
        var value = new SummaryListOptionsRowValue();
        var actions = new SummaryListOptionsRowActions();

        var attributes = CreateDummyDataAttributes();
        var className = CreateDummyClassName();

        var summaryListContext = new SummaryListContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: summaryListContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var rowContext = context.GetContextItem<SummaryListRowContext>();
                rowContext.SetKey(key);
                rowContext.SetValue(value);
                rowContext.SetActions(actions);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var row = Assert.Single(summaryListContext.Rows);
        Assert.Equal(className, row.Classes);
        AssertContainsAttributes(attributes, row.Attributes);
        Assert.Same(key, row.Key);
        Assert.Same(value, row.Value);
        Assert.Same(actions, row.Actions);
    }

    [Fact]
    public async Task ProcessAsync_RowIsMissingKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var summaryListContext = new SummaryListContext();

        var context = CreateTagHelperContext(contexts: summaryListContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"A <{SummaryListRowKeyTagHelper.TagName}> element must be provided.", ex.Message);
    }
}
