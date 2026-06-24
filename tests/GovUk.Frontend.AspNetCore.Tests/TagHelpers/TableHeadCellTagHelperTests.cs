using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableHeadCellTagHelperTests : TagHelperTestBase<TableHeadCellTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsHeadCellToTableContext()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Header Cell");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Single(tableContext.Head);
        Assert.Equal("Header Cell", tableContext.Head.First().Html);
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_WithFormat_SetsFormat()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Price");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper
        {
            Format = "numeric"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Single(tableContext.Head);
        Assert.Equal("numeric", tableContext.Head.First().Format);
    }

    [Fact]
    public async Task ProcessAsync_WithColSpan_SetsColSpan()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Header");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper
        {
            ColSpan = 3
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Single(tableContext.Head);
        Assert.Equal(3, tableContext.Head.First().ColSpan);
    }

    [Fact]
    public async Task ProcessAsync_WithRowSpan_SetsRowSpan()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Header");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper
        {
            RowSpan = 2
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Single(tableContext.Head);
        Assert.Equal(2, tableContext.Head.First().RowSpan);
    }

    [Fact]
    public async Task ProcessAsync_WithClassAndAttributes_PreservesClassAndAttributes()
    {
        // Arrange
        var tableContext = new TableContext();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Header");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Single(tableContext.Head);
        Assert.Equal(className, tableContext.Head.First().Classes);
        AssertContainsAttributes(attributes, tableContext.Head.First().Attributes);
    }
}
