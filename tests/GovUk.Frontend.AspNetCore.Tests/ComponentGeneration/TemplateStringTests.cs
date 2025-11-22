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
}
