using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableRowTagHelperTests : TagHelperTestBase<TableRowTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsRowToContext()
    {
        // Arrange
        var cell = new TableOptionsColumn();

        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var rowContext = context.GetContextItem<TableRowContext>();
                rowContext.AddCell(cell);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableRowTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var row = Assert.Single(tableContext.Rows);
        Assert.Collection(row, c => Assert.Same(cell, c));
    }

    [Fact]
    public async Task ProcessAsync_WithNoCells_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableRowTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A {TestUtils.GetAllTagNameElementsMessage(TableCellTagHelper.AllTagNames, "or")} element must be provided.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithAttributes_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext();

        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(attributes: attributes, contexts: tableContext);

        var output = CreateTagHelperOutput(attributes: attributes);

        var tagHelper = new TableRowTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Passing additional attributes is not supported.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var captionTagName = GetOtherSpellingSiblingTagName(
            TableCaptionTagHelper.TagName,
            TableCaptionTagHelper.ShortTagName);

        var tableContext = new TableContext();
        tableContext.SetCaption(new TemplateString("The caption"), null, [], captionTagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var rowContext = context.GetContextItem<TableRowContext>();
                rowContext.AddCell(new TableOptionsColumn());

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableRowTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{captionTagName}>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
