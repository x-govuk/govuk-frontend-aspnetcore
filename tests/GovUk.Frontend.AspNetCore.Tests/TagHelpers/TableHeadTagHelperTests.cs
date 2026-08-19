using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableHeadTagHelperTests : TagHelperTestBase<TableHeadTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHeadOnContext()
    {
        // Arrange
        var cell = new TableOptionsHead();

        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var headContext = context.GetContextItem<TableHeadContext>();
                headContext.AddCell(cell);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Head);
        Assert.Collection(tableContext.Head, c => Assert.Same(cell, c));
    }

    [Fact]
    public async Task ProcessAsync_WithNoCells_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext();

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableHeadTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"A {TestUtils.GetAllTagNameElementsMessage(TableHeadCellTagHelper.AllTagNames, "or")} element must be provided.",
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

        var tagHelper = new TableHeadTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Passing additional attributes is not supported.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasHead_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext();
        tableContext.SetHead([new TableOptionsHead()], TagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var headContext = context.GetContextItem<TableHeadContext>();
                headContext.AddCell(new TableOptionsHead());

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasRow_ThrowsInvalidOperationException()
    {
        // Arrange
        var rowTagName = GetSiblingTagName(TableRowTagHelper.TagName, TableRowTagHelper.ShortTagName);

        var tableContext = new TableContext();
        tableContext.AddRow([new TableOptionsColumn()], rowTagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var headContext = context.GetContextItem<TableHeadContext>();
                headContext.AddCell(new TableOptionsHead());

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableHeadTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{rowTagName}>.", ex.Message);
    }
}
