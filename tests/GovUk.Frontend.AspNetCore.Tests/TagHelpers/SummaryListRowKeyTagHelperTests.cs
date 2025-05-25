using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListRowKeyTagHelperTests() : TagHelperTestBase(SummaryListRowKeyTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsValueToContext()
    {
        // Arrange
        var content = "Key";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            className: className,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowKeyTagHelper();

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(rowContext.Key);
        Assert.Equal(content, rowContext.Key.Html);
        Assert.Equal(className, rowContext.Key.Classes);
        AssertContainsAttributes(attributes, rowContext.Key.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Key";

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetKey(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowKeyTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-summary-list-row-key> element is permitted within each <govuk-summary-list-row>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Key";

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetValue(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowKeyTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-summary-list-row-key> must be specified before <govuk-summary-list-row-value>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentHasActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var content = "Key";

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();
        rowContext.SetActions(new());

        var context = CreateTagHelperContext(contexts: [summaryListContext, rowContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowKeyTagHelper();

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-summary-list-row-key> must be specified before <govuk-summary-list-row-actions>.", ex.Message);
    }
}
