using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelTitleTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_SetsTitleOnContext()
    {
        // Arrange
        var panelContext = new PanelContext();

        var context = new TagHelperContext(
            tagName: "govuk-panel-title",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(PanelContext), panelContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel-title",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The title");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelTitleTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(panelContext.Title);
        Assert.Equal("The title", panelContext.Title.Value.Content);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var panelContext = new PanelContext();
        panelContext.SetTitle(TemplateString.FromEncoded("The title"), null);

        var context = new TagHelperContext(
            tagName: "govuk-panel-title",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(PanelContext), panelContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel-title",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The title");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-panel-title> element is permitted within each <govuk-panel>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasBody_ThrowsInvalidOperationException()
    {
        // Arrange
        var panelContext = new PanelContext();
        panelContext.SetBody(TemplateString.FromEncoded("The body"), null);

        var context = new TagHelperContext(
            tagName: "govuk-panel-title",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(PanelContext), panelContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel-title",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The title");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelTitleTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-panel-title> must be specified before <govuk-panel-body>.", ex.Message);
    }
}
