using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.Testing.Tests;

public class SimpleComponentTests
{
    [Fact]
    public async Task Tags_ReadsTextAndColour()
    {
        var doc = await Render.DocumentAsync(
            g => g.GenerateTagAsync(new TagOptions { Text = "Completed" }),
            g => g.GenerateTagAsync(new TagOptions { Text = "Pending", Classes = "govuk-tag--yellow" }));

        Assert.Collection(
            doc.Tags,
            tag =>
            {
                Assert.Equal("Completed", tag.Text);
                Assert.Null(tag.Colour);
            },
            tag =>
            {
                Assert.Equal("Pending", tag.Text);
                Assert.Equal("yellow", tag.Colour);
            });
    }

    [Fact]
    public async Task Panel_ReadsTitleAndBody()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePanelAsync(new PanelOptions
        {
            TitleText = "Application complete",
            Html = new HtmlString("Your reference number<br><strong>HDJ2123F</strong>")
        }));

        var panel = doc.Panel;

        Assert.NotNull(panel);
        Assert.Equal("Application complete", panel.Title);
        Assert.Equal("Your reference number HDJ2123F", panel.Body);
    }

    [Fact]
    public async Task Panel_WithoutBody_HasNullBody()
    {
        var doc = await Render.DocumentAsync(g => g.GeneratePanelAsync(new PanelOptions { TitleText = "Application complete" }));

        Assert.Null(doc.Panel!.Body);
    }

    [Fact]
    public async Task BackLink_ReadsTextAndHref()
    {
        var doc = await Render.DocumentAsync(g => g.GenerateBackLinkAsync(new BackLinkOptions { Href = "/previous" }));

        var backLink = doc.BackLink;

        Assert.NotNull(backLink);
        Assert.Equal("Back", backLink.Text);
        Assert.Equal("/previous", backLink.Href);
    }

    [Fact]
    public void SingleComponents_NoneOnPage_ReturnNull()
    {
        var doc = Render.Document("<p>Nothing here</p>");

        Assert.Null(doc.Panel);
        Assert.Null(doc.BackLink);
        Assert.Null(doc.Pagination);
        Assert.Null(doc.TaskList);
        Assert.Empty(doc.Tags);
        Assert.Empty(doc.Tables);
    }

    [Fact]
    public void HiddenInputs_ReadsNamesAndValues()
    {
        var doc = Render.Document(
            """
            <form>
              <input type="hidden" name="__RequestVerificationToken" value="abc123">
              <input type="hidden" name="Id" value="42">
              <input type="text" name="Name" value="Not hidden">
            </form>
            """);

        Assert.Equal(["__RequestVerificationToken", "Id"], doc.HiddenInputs.Select(i => i.Name));
        Assert.Equal("42", doc.GetHiddenInputValue("Id"));
        Assert.Null(doc.FindHiddenInputValue("Name"));

        var ex = Assert.Throws<ComponentLookupException>(() => doc.GetHiddenInputValue("Name"));
        Assert.Equal(
            """
            Hidden input not found.
            Expected: a hidden input named "Name"
            Found:    hidden inputs with the names "__RequestVerificationToken", "Id"
            """,
            ex.Message);
    }

    [Fact]
    public async Task Queries_OnAnElement_SearchOnlyWithinIt()
    {
        var tag = (await Render.Generator.GenerateTagAsync(new TagOptions { Text = "Inside" })).GetHtml();
        var doc = Render.Document($"<div id=\"one\">{tag}</div><div id=\"two\"><strong class=\"govuk-tag\">Outside</strong></div>");

        Assert.Equal(["Inside"], doc.GetElementById("one")!.Tags.Select(t => t.Text));
        Assert.Equal(2, doc.Tags.Count);
    }
}
