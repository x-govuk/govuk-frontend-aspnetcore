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
        var caption = "The caption";
        var captionClassName = CreateDummyClassName();
        var captionAttributes = CreateDummyDataAttributes();
        var headCell = new TableOptionsHead();
        var headAttributes = CreateDummyDataAttributes();
        var row = new TableOptionsRow();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tableContext = context.GetContextItem<TableContext>();

                tableContext.SetCaption(
                    new TemplateString(caption),
                    captionClassName,
                    new AttributeCollection(captionAttributes),
                    TableCaptionTagHelper.TagName);

                tableContext.SetHead(
                    [headCell],
                    new AttributeCollection(headAttributes),
                    TableHeadTagHelper.TagName);

                tableContext.AddRow(row, TableRowTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

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
        Assert.Equal(caption, actualOptions.Caption?.ToHtmlString());
        Assert.Equal(captionClassName, actualOptions.CaptionClasses);
        AssertContainsAttributes(captionAttributes, actualOptions.CaptionAttributes);
        Assert.NotNull(actualOptions.Head);
        Assert.Collection(actualOptions.Head, c => Assert.Same(headCell, c));
        AssertContainsAttributes(headAttributes, actualOptions.HeadAttributes);
        Assert.NotNull(actualOptions.Rows);
        Assert.Collection(actualOptions.Rows, r => Assert.Same(row, r));
        Assert.True(actualOptions.FirstCellIsHeader);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_WithNoChildren_InvokesComponentGeneratorWithNoCaptionOrHead()
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
        Assert.Null(actualOptions.Caption);
        Assert.Null(actualOptions.CaptionClasses);
        Assert.Null(actualOptions.CaptionAttributes);
        Assert.Null(actualOptions.Head);
        Assert.Null(actualOptions.HeadAttributes);
        Assert.Empty(actualOptions.Rows!);
        Assert.Null(actualOptions.FirstCellIsHeader);
    }
}
