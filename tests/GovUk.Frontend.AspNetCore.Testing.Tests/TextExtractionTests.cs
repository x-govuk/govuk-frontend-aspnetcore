namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class TextExtractionTests
{
    [Theory]
    [InlineData("  Some\n   text  ", "Some text")]
    [InlineData("Visible<span class=\"govuk-visually-hidden\"> hidden</span>", "Visible")]
    [InlineData("One<br>Two", "One Two")]
    [InlineData("<p>One</p><p>Two</p>", "One Two")]
    [InlineData("<strong>Bold</strong> text", "Bold text")]
    [InlineData("Text<script>var x = 1;</script>", "Text")]
    public void GetText(string html, string expected)
    {
        var element = Render.Document($"<div id=\"root\">{html}</div>").GetElementById("root");

        Assert.Equal(expected, TextExtraction.GetText(element));
    }

    [Fact]
    public void GetLines_SplitsOnLineBreaksAndBlocks()
    {
        var element = Render.Document(
            """
            <div id="root">
              72 Guild Street<br>London<br><br>
              <ul><li>First</li><li>Second <em>item</em></li></ul>
              <p></p>
            </div>
            """).GetElementById("root")!;

        Assert.Equal(["72 Guild Street", "London", "First", "Second item"], TextExtraction.GetLines(element));
    }
}
