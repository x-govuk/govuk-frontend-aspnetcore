using GovUk.Frontend.AspNetCore.Views;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.Views;

public class AdditionalAttributesTagHelperTests
{
    [Fact]
    public void Process_WithAttributes_AddsThemToOutput()
    {
        // Arrange
        var (context, output) = CreateContextAndOutput();

        var tagHelper = new AdditionalAttributesTagHelper()
        {
            AdditionalAttributes = new AttributeDictionary
            {
                { "data-foo", "bar" },
                { "data-empty", null! }
            }
        };

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("bar", Assert.Single(output.Attributes, a => a.Name == "data-foo").Value);

        var emptyAttribute = Assert.Single(output.Attributes, a => a.Name == "data-empty");
        Assert.Null(emptyAttribute.Value);
        Assert.Equal(HtmlAttributeValueStyle.Minimized, emptyAttribute.ValueStyle);
    }

    [Fact]
    public void Process_WithAttributeThatIsAlreadyOnTheElement_ReplacesIt()
    {
        // Arrange
        var (context, output) = CreateContextAndOutput();
        output.Attributes.SetAttribute("lang", "en");

        var tagHelper = new AdditionalAttributesTagHelper()
        {
            AdditionalAttributes = new AttributeDictionary { { "lang", "cy" } }
        };

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("cy", Assert.Single(output.Attributes, a => a.Name == "lang").Value);
    }

    [Fact]
    public void Process_WithNoAttributes_LeavesOutputAlone()
    {
        // Arrange
        var (context, output) = CreateContextAndOutput();
        output.Attributes.SetAttribute("class", "govuk-main-wrapper");

        var tagHelper = new AdditionalAttributesTagHelper();

        // Act
        tagHelper.Process(context, output);

        // Assert
        Assert.Equal("govuk-main-wrapper", Assert.Single(output.Attributes).Value);
    }

    private static (TagHelperContext Context, TagHelperOutput Output) CreateContextAndOutput()
    {
        var context = new TagHelperContext(
            tagName: "main",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "main",
            attributes: new TagHelperAttributeList(),
            getChildContentAsync: (useCachedResult, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        return (context, output);
    }
}
