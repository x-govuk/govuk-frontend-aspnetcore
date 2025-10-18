using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SummaryCardActionTagHelperTests : TagHelperTestBase<SummaryCardActionTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsActionToContext()
    {
        // Arrange
        var href = "#";
        var content = "Change";
        var visuallyHiddenText = "vht";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        attributes.Add("href", href);

        var summaryCardContext = new SummaryCardContext();

        var actionsContext = new SummaryCardActionsContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [summaryCardContext, actionsContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new SummaryCardActionTagHelper()
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
                Assert.NotNull(action.Attributes);
                Assert.Equal(className, action.Classes);
                AssertContainsAttributes(attributes, action.Attributes, except: "href");
            });
    }
}
