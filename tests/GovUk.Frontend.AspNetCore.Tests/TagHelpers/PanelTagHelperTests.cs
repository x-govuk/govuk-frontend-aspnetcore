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
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(headingLevel, actualOptions.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml?.ToHtmlString());
        Assert.Null(actualOptions.TitleText);
        Assert.Equal(bodyContent, actualOptions.Html?.ToHtmlString());
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
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(1, actualOptions.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml?.ToHtmlString());
        Assert.Equal(bodyContent, actualOptions.Html?.ToHtmlString());
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
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(titleContent, actualOptions.TitleHtml?.ToHtmlString());
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
        tagHelper.Init(context);

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
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        AssertContainsAttributes(bodyAttributes, actualOptions.BodyAttributes);
    }

    [Fact]
    public async Task ProcessAsync_WithActions_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var className = "govuk-panel--interruption";
        var actions = new PanelActionsOptions
        {
            Items = [new PanelActionsItemOptions { Text = "Yes", Type = "button" }],
            Classes = "actions-class"
        };

        var context = CreateTagHelperContext(className: className);

        var output = CreateTagHelperOutput(
            className: className,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetActions(actions, PanelActionsTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<PanelOptions>(nameof(IComponentGenerator.GeneratePanelAsync));

        var tagHelper = new PanelTagHelper(componentGenerator);
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Same(actions, actualOptions.Actions);
    }

    [Fact]
    public async Task ProcessAsync_WithActionsButNotInterruptionVariant_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = context.GetContextItem<PanelContext>();
                panelContext.SetTitle(TemplateString.FromEncoded("Title"), null);

                // Use the short tag name to verify the actual tag name is used in the message.
                panelContext.SetActions(
                    new PanelActionsOptions
                    {
                        Items = [new PanelActionsItemOptions { Text = "Yes", Type = "button" }]
                    },
                    PanelActionsTagHelper.ShortTagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new PanelTagHelper(TestUtils.CreateComponentGenerator());
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "The <panel-actions> element can only be used when the 'govuk-panel--interruption' class is specified on the <govuk-panel> element.",
            ex.Message);
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
        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-panel-title> element must be provided.", ex.Message);
    }
}
