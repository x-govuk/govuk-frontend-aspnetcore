using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelActionsTagHelperTests : TagHelperTestBase<PanelActionsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsActionsOnPanelContext()
    {
        // Arrange
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();

        var panelContext = new PanelContext();

        var context = CreateTagHelperContext(
            className: className,
            attributes: attributes,
            contexts: [panelContext]);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var actionsContext = context.GetContextItem<PanelActionsContext>();
                actionsContext.Actions.Add(new PanelActionsItemOptions { Text = "Yes", Type = "button" });

                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var tagHelper = new PanelActionsTagHelper();
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(panelContext.Actions);
        Assert.Equal(className, panelContext.Actions.Classes);
        AssertContainsAttributes(attributes, panelContext.Actions.Attributes);
        Assert.Collection(panelContext.Actions.Items!, action => Assert.Equal("Yes", action.Text));
    }

    [Fact]
    public async Task ProcessAsync_AlreadyGotActions_ThrowsInvalidOperationException()
    {
        // Arrange
        var panelContext = new PanelContext();
        panelContext.SetActions(new PanelActionsOptions(), PanelActionsTagHelper.TagName);

        var context = CreateTagHelperContext(contexts: [panelContext]);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        var tagHelper = new PanelActionsTagHelper();
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <govuk-panel-actions> element is permitted within each <govuk-panel>.", ex.Message);
    }
}
