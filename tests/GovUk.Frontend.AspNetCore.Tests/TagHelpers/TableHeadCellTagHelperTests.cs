using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableHeadCellTagHelperTests : TagHelperTestBase<TableHeadCellTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsCellToContext()
    {
        // Arrange
        var content = "Date";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var headContext = new TableHeadContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: headContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadCellTagHelper()
        {
            Format = "numeric",
            ColSpan = 2,
            RowSpan = 3
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var cell = Assert.Single(headContext.Cells);
        Assert.Null(cell.Text);
        Assert.Equal(content, cell.Html?.ToHtmlString());
        Assert.Equal("numeric", cell.Format);
        Assert.Equal(2, cell.ColSpan);
        Assert.Equal(3, cell.RowSpan);
        Assert.Equal(className, cell.Classes);
        AssertContainsAttributes(attributes, cell.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_WithoutOptionalAttributes_AddsCellToContext()
    {
        // Arrange
        var headContext = new TableHeadContext();

        var context = CreateTagHelperContext(contexts: headContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableHeadCellTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var cell = Assert.Single(headContext.Cells);
        Assert.Null(cell.Format);
        Assert.Null(cell.ColSpan);
        Assert.Null(cell.RowSpan);
        Assert.Null(cell.Classes);
    }
}
