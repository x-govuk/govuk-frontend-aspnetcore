using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputItemTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddItemToContext()
    {
        // Arrange
        var dateInputContext = new DateInputContext(haveExplicitValue: false, aspFor: null);

        var context = new TagHelperContext(
            tagName: "govuk-date-input-day",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>()
            {
                { typeof(DateInputContext), dateInputContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-date-input-day",
            attributes: new TagHelperAttributeList(),
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<DateInputItemContext>();
                itemContext.SetLabel(new HtmlString("Label"), attributes: new AttributeDictionary());

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputItemTagHelper()
        {
            AutoComplete = "off",
            Id = "my-day",
            InputMode = "im",
            Name = "my_day",
            Pattern = "*",
            Value = 2
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            dateInputContext.Items.Values,
            item =>
            {
                Assert.Equal("off", item.AutoComplete);
                Assert.Equal("my-day", item.Id);
                Assert.Equal("im", item.InputMode);
                Assert.Equal("Label", item.LabelContent?.ToHtmlString());
                Assert.Equal("my_day", item.Name);
                Assert.Equal("*", item.Pattern);
                Assert.Equal(2, item.Value);
                Assert.True(item.ValueSpecified);
            });
    }
}
