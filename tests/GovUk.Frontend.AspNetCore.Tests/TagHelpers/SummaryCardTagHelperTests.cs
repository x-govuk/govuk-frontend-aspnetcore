using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryCardTagHelperTests() : TagHelperTestBase(SummaryCardTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_GeneratesExpectedOutput()
    {
        // Arrange
        var title = new SummaryListOptionsCardTitle();
        var actions = new SummaryListOptionsCardActions();
        var summaryList = new SummaryListOptions();
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var summaryCardContext = context.GetContextItem<SummaryCardContext>();
                summaryCardContext.SetTitle(title);
                summaryCardContext.SetActions(actions);
                summaryCardContext.SetSummaryList(summaryList);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<SummaryListOptions>(nameof(IComponentGenerator.GenerateSummaryListAsync));

        var tagHelper = new SummaryCardTagHelper(componentGenerator);

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.Card);
        Assert.Same(title, actualOptions.Card.Title);
        Assert.Same(actions, actualOptions.Card.Actions);
        Assert.Equal(className, actualOptions.Card.Classes);
        AssertContainsAttributes(attributes, actualOptions.Card.Attributes);
    }
}
