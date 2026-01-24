using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelTagHelperTests : TagHelperTestBase<PanelTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";
        var headingLevel = 3;
        var classes = "custom-class";
        var attributes = new Dictionary<string, string?> { { "data-foo", "bar" } };

        var context = CreateTagHelperContext(className: classes, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: classes,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator)
        {
            HeadingLevel = headingLevel
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(headingLevel, actualOptions.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml);
        Assert.Null(actualOptions.TitleText);
        Assert.Equal(bodyContent, actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(classes, actualOptions.Classes);
        AssertContainsAttributes(attributes, actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_WithDefaultHeadingLevel_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(1, actualOptions.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml);
        Assert.Equal(bodyContent, actualOptions.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithTitleOnly_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(titleContent, actualOptions.TitleHtml);
        Assert.Null(actualOptions.Html);
        Assert.Null(actualOptions.Text);
    }

    [Fact]
    public async Task ProcessAsync_WithTitleAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var titleAttributes = new Dictionary<string, string?> { { "data-title", "title-data" } };

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), new AttributeCollection(titleAttributes));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        AssertContainsAttributes(titleAttributes, actualOptions.TitleAttributes);
    }

    [Fact]
    public async Task ProcessAsync_WithBodyAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";
        var bodyAttributes = new Dictionary<string, string?> { { "data-body", "body-data" } };

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), new AttributeCollection(bodyAttributes));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        AssertContainsAttributes(bodyAttributes, actualOptions.BodyAttributes);
    }

    [Fact]
    public async Task ProcessAsync_MissingTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetBody(TemplateString.FromEncoded("Body"), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelTagHelper(TestUtils.CreateComponentGenerator())
        {
            HeadingLevel = 3
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-panel-title> element must be provided.", ex.Message);
    }
}
