using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class HtmlTagTests
{
    [Fact]
    public void HtmlTagWithInnerHtml_RendersCorrectly()
    {
        // Arrange
        var tag = new HtmlTag("button");
        tag.Attributes.AddBoolean("disabled");
        tag.Attributes.Add("type", "text");
        tag.InnerHtml.Append("Foo & bar");

        // Act
        var result = tag.ToHtmlString();

        // Assert
        var expected = "<button disabled type=\"text\">Foo &amp; bar</button>";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HtmlTag_WithSelfClosingRenderMode_RendersCorrectly()
    {
        // Arrange
        var tag = new HtmlTag("input");
        tag.Attributes.Add("type", "text");
        tag.TagRenderMode = TagRenderMode.SelfClosing;

        // Act
        var result = tag.ToHtmlString();

        // Assert
        var expected = "<input type=\"text\" />";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HtmlTag_WithStartTagRenderMode_RendersCorrectly()
    {
        // Arrange
        var tag = new HtmlTag("div");
        tag.Attributes.Add("class", "container");
        tag.TagRenderMode = TagRenderMode.StartTag;

        // Act
        var result = tag.ToHtmlString();

        // Assert
        var expected = "<div class=\"container\">";
        Assert.Equal(expected, result);
    }

    [Fact]
    public void HtmTag_WithEndTagRenderMode_RendersCorrectly()
    {
        // Arrange
        var tag = new HtmlTag("div");
        tag.TagRenderMode = TagRenderMode.EndTag;

        // Act
        var result = tag.ToHtmlString();

        // Assert
        var expected = "</div>";
        Assert.Equal(expected, result);
    }
}
