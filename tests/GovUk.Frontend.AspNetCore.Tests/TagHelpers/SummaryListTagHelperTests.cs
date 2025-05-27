using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListTagHelperTests() : TagHelperTestBase(SummaryListTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var row = new SummaryListOptionsRow();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var summaryListContent = context.GetContextItem<SummaryListContext>();
                summaryListContent.AddRow(row);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<SummaryListOptions>(nameof(IComponentGenerator.GenerateSummaryListAsync));

        var tagHelper = new SummaryListTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Rows);
        Assert.Collection(actualOptions.Rows, r => Assert.Same(row, r));
        Assert.Null(actualOptions.Card);
        Assert.Equal(className, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_WithinSummaryCard_AddsSummaryListToCardContext()
    {
        // Arrange
        var cardContext = new SummaryCardContext();

        var context = CreateTagHelperContext(contexts: cardContext);

        var output = CreateTagHelperOutput();

        var (componentGenerator, _) = CreateComponentGenerator<SummaryListOptions>(nameof(IComponentGenerator.GenerateSummaryListAsync));

        var tagHelper = new SummaryListTagHelper(componentGenerator);

        tagHelper.Init(context);
        context.GetContextItem<SummaryListContext>().HaveCard = true;

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(output.TagName);
        Assert.NotNull(cardContext.SummaryList);
    }

    [Fact]
    public async Task ProcessAsync_WithinSummaryCardAndAlreadyGotSummaryList_ThrowsInvalidOperationException()
    {
        // Arrange
        var cardContext = new SummaryCardContext();
        cardContext.SetSummaryList(new());

        var context = CreateTagHelperContext(contexts: cardContext);

        var output = CreateTagHelperOutput();

        var (componentGenerator, _) = CreateComponentGenerator<SummaryListOptions>(nameof(IComponentGenerator.GenerateSummaryListAsync));

        var tagHelper = new SummaryListTagHelper(componentGenerator);

        tagHelper.Init(context);
        context.GetContextItem<SummaryListContext>().HaveCard = true;

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal($"Only one <{TagName}> element is permitted within each <{SummaryCardTagHelper.TagName}>.", ex.Message);
    }
}
