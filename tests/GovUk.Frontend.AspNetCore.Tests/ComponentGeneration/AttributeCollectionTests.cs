using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class AttributeCollectionTests
{
    [Fact]
    public void Add_ClassMultiplesTimes()
    {
        // Arrange
        var attributes = new AttributeCollection();

        // Act
        attributes.Add("class", "govuk-button");
        attributes.Add("class", "govuk-button--primary");

        // Assert
        Assert.Equal("govuk-button govuk-button--primary", attributes["class"]?.ToHtmlString());
    }

    [Fact]
    public void Add_ClassMultiplesTimesWithAttribute()
    {
        // Arrange
        var attributes = new AttributeCollection();

        // Act
        attributes.Add(new AttributeCollection.Attribute("class", "govuk-button", Optional: false));
        attributes.Add(new AttributeCollection.Attribute("class", "govuk-button--primary", Optional: false));

        // Assert
        Assert.Equal("govuk-button govuk-button--primary", attributes["class"]?.ToHtmlString());
    }

    [Fact]
    public void Add_AriaDescribedByMultipleTimes()
    {
        // Arrange
        var attributes = new AttributeCollection();

        // Act
        attributes.Add("aria-describedby", "id1");
        attributes.Add("aria-describedby", "id2");

        // Assert
        Assert.Equal("id1 id2", attributes["aria-describedby"]?.ToHtmlString());
    }

    [Fact]
    public void Add_AriaDescribedByMultipleTimesWithAttribute()
    {
        // Arrange
        var attributes = new AttributeCollection();

        // Act
        attributes.Add(new AttributeCollection.Attribute("aria-describedby", "id1", Optional: false));
        attributes.Add(new AttributeCollection.Attribute("aria-describedby", "id2", Optional: false));

        // Assert
        Assert.Equal("id1 id2", attributes["aria-describedby"]?.ToHtmlString());
    }

    [Theory]
    [InlineData("a&b")]
    [InlineData("a<b>c")]
    [InlineData("say \"hi\"")]
    public void ReadingAndWritingAnAttributeFromRazor_RoundTripsExactly(string value)
    {
        // Razor hands over attribute values already encoded, as an IHtmlContent. Reading one back out
        // and putting it in again must not treat it as text, or it gets encoded a second time.
        var encoded = HtmlEncoder.Default.Encode(value);

        var attributes = new AttributeCollection(
            [new TagHelperAttribute("data-test", new HtmlString(encoded), HtmlAttributeValueStyle.DoubleQuotes)]);

        // Act
        var viaIndexer = attributes["data-test"];
        var viaEnumerator = attributes.Single().Value;
        Assert.True(attributes.Remove("data-test", out var viaRemove));

        // Assert
        foreach (var read in new[] { viaIndexer, viaEnumerator, viaRemove })
        {
            Assert.NotNull(read);

            var tag = new HtmlTag("div", attrs => attrs.With("data-test", read));
            var element = HtmlHelper.ParseHtmlElement(tag.ToHtmlString());

            Assert.Equal(value, element.GetAttribute("data-test"));
        }
    }
}
