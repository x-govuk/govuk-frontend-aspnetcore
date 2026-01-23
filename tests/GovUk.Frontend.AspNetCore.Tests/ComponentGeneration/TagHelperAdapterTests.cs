using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class TagHelperAdapterTests
{
    [Fact]
    public void EmptyHtml()
    {
        // Arrange
        var html = "";
        var content = new HtmlString(html);

        // Act
        var result = TagHelperAdapter.UnwrapComponent(content);

        // Assert
        Assert.Null(result.TagName);
        Assert.Empty(result.Attributes);
        Assert.Equal("", result.InnerHtml.ToHtmlString(HtmlEncoder.Default));
    }

    [Theory]
    [InlineData("<br>", TagMode.StartTagOnly)]
    [InlineData("<br/>", TagMode.SelfClosing)]
    [InlineData("<br />", TagMode.SelfClosing)]
    public void VoidElementWithNoAttributes(string html, TagMode expectedTagMode)
    {
        // Arrange
        var content = new HtmlString(html);

        // Act
        var result = TagHelperAdapter.UnwrapComponent(content);

        // Assert
        Assert.Equal("br", result.TagName);
        Assert.Equal(expectedTagMode, result.TagMode);
        Assert.Empty(result.Attributes);
        Assert.Equal("", result.InnerHtml.ToHtmlString());
    }

    [Fact]
    public void InnerHtml()
    {
        // Arrange
        var html = "<div><a href=\"foo?bar=baz\">Hello world</a></div>";
        var content = new HtmlString(html);

        // Act
        var result = TagHelperAdapter.UnwrapComponent(content);

        // Assert
        Assert.Equal("<a href=\"foo?bar=baz\">Hello world</a>", result.InnerHtml.ToHtmlString());
    }

    [Fact]
    public void MalformedInnerHtml()
    {
        // Arrange
        var html = "<div><a href=https://some.url>Hello world</div>";
        var content = new HtmlString(html);

        // Act
        var result = TagHelperAdapter.UnwrapComponent(content);

        // Assert
        Assert.Equal("<a href=https://some.url>Hello world", result.InnerHtml.ToHtmlString());
    }

    [Fact]
    public void Attributes()
    {
        // Arrange
        var html = "<form novalidate class=\"foo\" disabled=\"\"></form>";
        var content = new HtmlString(html);

        // Act
        var result = TagHelperAdapter.UnwrapComponent(content);

        // Assert
        Assert.Collection(
            result.Attributes,
            attr =>
            {
                Assert.Equal("novalidate", attr.Name);
                Assert.Equal(HtmlAttributeValueStyle.Minimized, attr.ValueStyle);
            },
            attr =>
            {
                Assert.Equal("class", attr.Name);
                Assert.Equal("foo", attr.Value?.ToString());
                Assert.Equal(HtmlAttributeValueStyle.DoubleQuotes, attr.ValueStyle);
            },
            attr =>
            {
                Assert.Equal("disabled", attr.Name);
                Assert.Equal("", attr.Value?.ToString());
                Assert.Equal(HtmlAttributeValueStyle.DoubleQuotes, attr.ValueStyle);
            });
    }
}
