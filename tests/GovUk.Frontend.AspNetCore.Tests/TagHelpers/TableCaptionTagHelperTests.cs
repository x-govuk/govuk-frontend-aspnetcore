using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableCaptionTagHelperTests : TagHelperTestBase<TableCaptionTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsCaptionInTableContext()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Test Caption");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCaptionTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("Test Caption", tableContext.Caption);
        Assert.Null(output.TagName);
    }

    [Fact]
    public async Task ProcessAsync_WithClass_SetsCaptionClasses()
    {
        // Arrange
        var tableContext = new TableContext();
        var className = CreateDummyClassName();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            className: className,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Caption");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCaptionTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(className, tableContext.CaptionClasses);
    }

    [Fact]
    public async Task ProcessAsync_WhenCaptionAlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext
        {
            Caption = new("Existing Caption")
        };

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("New Caption");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCaptionTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TableCaptionTagHelper.TagName}> element is permitted within each <{TableTagHelper.TagName}>.", ex.Message);
    }
}
