using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableBodyTagHelperTests : TagHelperTestBase<TableBodyTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SuppressesOutput()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var tagHelper = new TableBodyTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(output.TagName);
    }
}
