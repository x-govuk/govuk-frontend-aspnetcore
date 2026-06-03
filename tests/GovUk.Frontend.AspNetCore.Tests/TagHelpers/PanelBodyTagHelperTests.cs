using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelBodyTagHelperTests : TagHelperTestBase<PanelBodyTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsBodyOnContext()
    {
        // Arrange
        var panelContext = new PanelContext();

        var context = CreateTagHelperContext(contexts: panelContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The body");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelBodyTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(panelContext.Body);
        Assert.Equal("The body", panelContext.Body.Value.Content);
    }

    [Fact]
    public async Task ProcessAsync_ParentAlreadyHasBody_ThrowsInvalidOperationException()
    {
        // Arrange
        var panelContext = new PanelContext();
        panelContext.SetBody(TemplateString.FromEncoded("The body"), null);

        var context = CreateTagHelperContext(contexts: panelContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("The body");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelBodyTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-panel-body> element is permitted within each <govuk-panel>.", ex.Message);
    }
}
