using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableRowTagHelperTests : TagHelperTestBase<TableRowTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsRowToTableContext()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var rowContext = context.GetContextItem<TableRowContext>();
                rowContext.AddCell(new TableOptionsColumn { Text = new("Cell 1") });
                rowContext.AddCell(new TableOptionsColumn { Text = new("Cell 2") });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableRowTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(tableContext.Rows);
        Assert.Equal(2, tableContext.Rows.First().Count);
        Assert.Null(output.TagName);
    }
}
