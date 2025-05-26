using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryListRowActionTagHelperTests() : TagHelperTestBase(SummaryListRowActionTagHelper.TagName, parentTagName: SummaryListRowTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_AddsActionToContext()
    {
        // Arrange
        var content = "Change";
        var href = "href";
        var visuallyHiddenText = "vht";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

        var summaryListContext = new SummaryListContext();

        var rowContext = new SummaryListRowContext();

        var actionsContext = new SummaryListRowActionsContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [summaryListContext, rowContext, actionsContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryListRowActionTagHelper()
        {
            VisuallyHiddenText = visuallyHiddenText
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actionsContext.Items);
        Assert.Collection(
            actionsContext.Items,
            action =>
            {
                Assert.Equal(content, action.Html);
                Assert.Equal(visuallyHiddenText, action.VisuallyHiddenText);
                Assert.Equal(href, action.Href);
                AssertContainsAttributes(attributes, action.Attributes, except: href);
                Assert.Equal(className, action.Classes);
            });
    }
}
