using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableTagHelperTests : TagHelperTestBase<TableTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var row = new List<TableOptionsColumn>
        {
            new TableOptionsColumn { Text = new("Cell 1") },
            new TableOptionsColumn { Text = new("Cell 2") }
        };
        var headCell = new TableOptionsHead { Text = new("Header 1") };
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tableContext = context.GetContextItem<TableContext>();
                tableContext.AddRow(row);
                tableContext.AddHeadCell(headCell);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TableOptions>(nameof(IComponentGenerator.GenerateTableAsync));

        var tagHelper = new TableTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Rows);
        Assert.Collection(actualOptions.Rows, r => Assert.Same(row, r));
        Assert.NotNull(actualOptions.Head);
        Assert.Collection(actualOptions.Head, h => Assert.Same(headCell, h));
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_WithCaption_SetsCaption()
    {
        // Arrange
        var caption = "Test Caption";
        var captionClasses = "caption-class";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tableContext = context.GetContextItem<TableContext>();
                tableContext.Caption = new(caption);
                tableContext.CaptionClasses = new(captionClasses);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TableOptions>(nameof(IComponentGenerator.GenerateTableAsync));

        var tagHelper = new TableTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(caption, actualOptions.Caption);
        Assert.Equal(captionClasses, actualOptions.CaptionClasses);
    }

    [Fact]
    public async Task ProcessAsync_WithFirstCellIsHeaderAttribute_SetsFirstCellIsHeader()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TableOptions>(nameof(IComponentGenerator.GenerateTableAsync));

        var tagHelper = new TableTagHelper(componentGenerator)
        {
            FirstCellIsHeader = true
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.True(actualOptions.FirstCellIsHeader);
    }

    [Fact]
    public async Task ProcessAsync_WithoutRows_SetsRowsToNull()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TableOptions>(nameof(IComponentGenerator.GenerateTableAsync));

        var tagHelper = new TableTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Rows);
    }

    [Fact]
    public async Task ProcessAsync_WithoutHead_SetsHeadToNull()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TableOptions>(nameof(IComponentGenerator.GenerateTableAsync));

        var tagHelper = new TableTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Null(actualOptions.Head);
    }
}
