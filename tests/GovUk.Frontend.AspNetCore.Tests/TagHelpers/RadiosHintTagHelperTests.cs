using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosHintTagHelperTests : TagHelperTestBase<RadiosHintTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsHintOnContext()
    {
        // Arrange
        var hintContent = "Hint text";
        var attributes = CreateDummyDataAttributes();

        var radiosContext = new RadiosContext(name: null, @for: null);

        var tagHelperAttributes = new TagHelperAttributeList();
        foreach (var attr in attributes)
        {
            tagHelperAttributes.Add(
                new TagHelperAttribute(
                    attr.Key,
                    attr.Value,
                    attr.Value is not null ? HtmlAttributeValueStyle.DoubleQuotes : HtmlAttributeValueStyle.Minimized));
        }

        var contextItems = new Dictionary<object, object>
        {
            { typeof(RadiosContext), radiosContext },
            { typeof(FormGroupContext3), radiosContext }
        };

        var context = new TagHelperContext(
            tagName: "govuk-radios-hint",
            allAttributes: tagHelperAttributes,
            items: contextItems,
            uniqueId: "test");

        var output = CreateTagHelperOutput(
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(hintContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new RadiosHintTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(radiosContext.Hint);
        Assert.Equal(hintContent, radiosContext.Hint?.Html?.ToHtmlString());
        AssertContainsAttributes(attributes, radiosContext.Hint?.Attributes);
    }
}
