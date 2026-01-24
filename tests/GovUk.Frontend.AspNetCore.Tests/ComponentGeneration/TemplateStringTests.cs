using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class TemplateStringTests
{
    [Fact]
    public void Concat_TemplateStringFromStringAndTemplateStringFromString_ReturnsExpectedResult()
    {
        // Arrange
        var a = new TemplateString("<Hello &");
        var b = new TemplateString(" Welcome>");

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString("<Hello & Welcome>"), result);
    }

    [Fact]
    public void Concat_TemplateStringFromHtmlContentAndTemplateStringFromHtml_ReturnsExpectedResult()
    {
        // Arrange
        var a = new TemplateString(new HtmlString("&lt;Hello &amp;"));
        var b = new TemplateString(new HtmlString(" Welcome&gt;"));

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString(new HtmlString("&lt;Hello &amp; Welcome&gt;")), result);
    }

    [Fact]
    public void Concat_TemplateStringFromStringAndString_ReturnsExpectedResult()
    {
        // Arrange
        var a = new TemplateString("<Hello &");
        var b = " Welcome>";

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString("<Hello & Welcome>"), result);
    }

    [Fact]
    public void Concat_TemplateStringFromHtmlContentAndString_ReturnsExpectedResult()
    {
        // Arrange
        var a = new TemplateString(new HtmlString("&lt;Hello &amp;"));
        var b = " Welcome>";

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString(new HtmlString("&lt;Hello &amp; Welcome&gt;")), result);
    }

    [Fact]
    public void Concat_StringAndTemplateStringFromString_ReturnsExpectedResult()
    {
        // Arrange
        var a = "<Hello &";
        var b = new TemplateString(" Welcome>");

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString("<Hello & Welcome>"), result);
    }

    [Fact]
    public void Concat_StringAndTemplateStringFromHtmlContent_ReturnsExpectedResult()
    {
        // Arrange
        var a = "<Hello &";
        var b = new TemplateString(new HtmlString(" Welcome&gt;"));

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(new TemplateString(new HtmlString("&lt;Hello &amp; Welcome&gt;")), result);
    }

    [Fact]
    public void ToString_ForString_ReturnsExpectedResult()
    {
        // Arrange
        var templateString = new TemplateString("<Hello & Welcome>");

        // Act
        var result = templateString.ToHtmlString(HtmlEncoder.Default);

        // Assert
        Assert.Equal("&lt;Hello &amp; Welcome&gt;", result);
    }

    [Fact]
    public void ToString_ForHtmlContent_ReturnsExpectedResult()
    {
        // Arrange
        var content = new HtmlString("<strong>Bold</strong>");
        var templateString = new TemplateString(content);

        // Act
        var result = templateString.ToHtmlString(HtmlEncoder.Default);

        // Assert
        Assert.Equal("<strong>Bold</strong>", result);
    }

    [Fact]
    public void Coalesce_WithFirstNonEmptyValue_ReturnsFirstNonEmpty()
    {
        // Arrange
        var first = new TemplateString("First");
        var second = new TemplateString("Second");
        var third = new TemplateString("Third");

        // Act
        var result = TemplateString.Coalesce(first, second, third);

        // Assert
        Assert.Equal(first, result);
    }

    [Fact]
    public void Coalesce_WithNullAndEmptyValuesFirst_ReturnsFirstNonEmpty()
    {
        // Arrange
        var empty = TemplateString.Empty;
        var nonEmpty = new TemplateString("NonEmpty");
        var another = new TemplateString("Another");

        // Act
        var result = TemplateString.Coalesce(null, empty, nonEmpty, another);

        // Assert
        Assert.Equal(nonEmpty, result);
    }

    [Fact]
    public void Coalesce_WithAllNullOrEmpty_ReturnsEmpty()
    {
        // Arrange & Act
        var result = TemplateString.Coalesce(null, TemplateString.Empty, null);

        // Assert
        Assert.Equal(TemplateString.Empty, result);
    }

    [Fact]
    public void Coalesce_WithEmptyArray_ReturnsEmpty()
    {
        // Arrange & Act
        var result = TemplateString.Coalesce();

        // Assert
        Assert.Equal(TemplateString.Empty, result);
    }

    [Fact]
    public void Coalesce_WithSingleNonEmptyValue_ReturnsThatValue()
    {
        // Arrange
        var value = new TemplateString("Value");

        // Act
        var result = TemplateString.Coalesce(value);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Coalesce_WithHtmlContent_ReturnsFirstNonEmpty()
    {
        // Arrange
        var empty = TemplateString.Empty;
        var htmlContent = new TemplateString(new HtmlString("<strong>Bold</strong>"));
        var stringContent = new TemplateString("Plain");

        // Act
        var result = TemplateString.Coalesce(null, empty, htmlContent, stringContent);

        // Assert
        Assert.Equal(htmlContent, result);
    }

    [Fact]
    public void Coalesce_WithNullArray_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => TemplateString.Coalesce(null!));
    }

    [Fact]
    public void Coalesce_WithMixedTypes_ReturnsFirstNonEmpty()
    {
        // Arrange
        var emptyString = new TemplateString("");
        var htmlValue = new TemplateString(new HtmlString("HTML"));
        var stringValue = new TemplateString("String");

        // Act
        var result = TemplateString.Coalesce(null, emptyString, htmlValue, stringValue);

        // Assert
        Assert.Equal(htmlValue, result);
    }
}
