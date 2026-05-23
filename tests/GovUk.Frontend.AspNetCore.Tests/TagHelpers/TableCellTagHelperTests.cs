using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableCellTagHelperTests : TagHelperTestBase<TableCellTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsCellToRowContext()
    {
        // Arrange
        var tableContext = new TableContext();
        var rowContext = new TableRowContext();

        var context = CreateTagHelperContext(contexts: new object[] { tableContext, rowContext });

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Cell Content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCellTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(rowContext.Cells);
        Assert.Equal("Cell Content", rowContext.Cells.First().Html);
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_WithFormat_SetsFormat()
    {
        // Arrange
        var rowContext = new TableRowContext();

        var context = CreateTagHelperContext(contexts: rowContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("123");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCellTagHelper
        {
            Format = "numeric"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(rowContext.Cells);
        Assert.Equal("numeric", rowContext.Cells.First().Format);
    }

    [Fact]
    public async Task ProcessAsync_WithColSpan_SetsColSpan()
    {
        // Arrange
        var rowContext = new TableRowContext();

        var context = CreateTagHelperContext(contexts: rowContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Cell");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCellTagHelper
        {
            ColSpan = 2
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(rowContext.Cells);
        Assert.Equal(2, rowContext.Cells.First().ColSpan);
    }

    [Fact]
    public async Task ProcessAsync_WithRowSpan_SetsRowSpan()
    {
        // Arrange
        var rowContext = new TableRowContext();

        var context = CreateTagHelperContext(contexts: rowContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Cell");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCellTagHelper
        {
            RowSpan = 3
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(rowContext.Cells);
        Assert.Equal(3, rowContext.Cells.First().RowSpan);
    }

    [Fact]
    public async Task ProcessAsync_WithClassAndAttributes_PreservesClassAndAttributes()
    {
        // Arrange
        var rowContext = new TableRowContext();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(contexts: rowContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Cell");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCellTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Single(rowContext.Cells);
        Assert.Equal(className, rowContext.Cells.First().Classes);
        AssertContainsAttributes(attributes, rowContext.Cells.First().Attributes);
    }
}
