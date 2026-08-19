using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TableCaptionTagHelperTests : TagHelperTestBase<TableCaptionTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsCaptionOnContext()
    {
        // Arrange
        var content = "The caption";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var tableContext = new TableContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: tableContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new TableCaptionTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(tableContext.Caption);
        Assert.Equal(content, tableContext.Caption.Value.Content.ToHtmlString());
        Assert.Equal(className, tableContext.Caption.Value.Classes);
        AssertContainsAttributes(attributes, tableContext.Caption.Value.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasCaption_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableContext = new TableContext();
        tableContext.SetCaption(new TemplateString("Existing"), null, [], TagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableCaptionTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"Only one {GetAllTagNameElementsMessage("or")} element is permitted within each <{ParentTagName}>.",
            ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasHead_ThrowsInvalidOperationException()
    {
        // Arrange
        var headTagName = GetSiblingTagName(TableHeadTagHelper.TagName, TableHeadTagHelper.ShortTagName);

        var tableContext = new TableContext();
        tableContext.SetHead([new TableOptionsHead()], headTagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableCaptionTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{headTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasRow_ThrowsInvalidOperationException()
    {
        // Arrange
        var rowTagName = GetSiblingTagName(TableRowTagHelper.TagName, TableRowTagHelper.ShortTagName);

        var tableContext = new TableContext();
        tableContext.AddRow([new TableOptionsColumn()], rowTagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableCaptionTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"<{TagName}> must be specified before <{rowTagName}>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_SiblingUsesOtherTagNameSpelling_ThrowsInvalidOperationException()
    {
        // Arrange
        var rowTagName = GetOtherSpellingSiblingTagName(TableRowTagHelper.TagName, TableRowTagHelper.ShortTagName);

        var tableContext = new TableContext();
        tableContext.AddRow([new TableOptionsColumn()], rowTagName);

        var context = CreateTagHelperContext(contexts: tableContext);

        var output = CreateTagHelperOutput();

        var tagHelper = new TableCaptionTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            $"<{TagName}> cannot be used alongside <{rowTagName}>; " +
                "short tag names and govuk- prefixed tag names cannot be mixed.",
            ex.Message);
    }
}
