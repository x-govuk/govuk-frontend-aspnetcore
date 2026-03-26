using GovUk.Frontend.AspNetCore.ComponentGeneration;

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
}
