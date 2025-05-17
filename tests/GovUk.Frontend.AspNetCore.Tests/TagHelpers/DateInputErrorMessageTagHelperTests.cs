using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputErrorMessageTagHelperTests() : TagHelperTestBase(DateInputTagHelper.ErrorMessageTagName, parentTagName: DateInputTagHelper.TagName)
{
    [Fact]
    public async Task ProcessAsync_SetsErrorMessageAndErrorComponentsOnContext()
    {
        // Arrange
        var errorContent = "Error message";
        var errorItems = DateInputItems.Day | DateInputItems.Month;
        var attributes = CreateDummyDataAttributes();

        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        var context = CreateTagHelperContext(
            attributes: attributes,
            contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(errorContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputErrorMessageTagHelper()
        {
            ErrorItems = errorItems
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(dateInputContext.ErrorMessage);
        Assert.Equal(errorContent, dateInputContext.ErrorMessage.Html);
        Assert.Equal(errorItems, dateInputContext.ErrorFields);
        AssertContainsAttributes(attributes, dateInputContext.ErrorMessage.Attributes);
    }
}
