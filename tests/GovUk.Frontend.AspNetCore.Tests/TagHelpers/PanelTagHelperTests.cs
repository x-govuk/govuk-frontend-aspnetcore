using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PanelTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";
        var headingLevel = 3;
        var classes = "custom-class";
        var dataFooAttrValue = "bar";

        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: new TagHelperAttributeList()
            {
                { "class", classes },
                { "data-foo", dataFooAttrValue }
            },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        PanelOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GeneratePanelAsync(It.IsAny<PanelOptions>())).Callback<PanelOptions>(o => actualOptions = o);

        var tagHelper = new PanelTagHelper(componentGeneratorMock.Object)
        {
            HeadingLevel = headingLevel
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(headingLevel, actualOptions!.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml);
        Assert.Null(actualOptions.TitleText);
        Assert.Equal(bodyContent, actualOptions.Html);
        Assert.Null(actualOptions.Text);
        Assert.Equal(classes, actualOptions.Classes);
        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes, kvp =>
        {
            Assert.Equal("data-foo", kvp.Key);
            Assert.Equal(dataFooAttrValue, kvp.Value);
        });
    }

    [Fact]
    public async Task ProcessAsync_WithDefaultHeadingLevel_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";

        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        PanelOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GeneratePanelAsync(It.IsAny<PanelOptions>())).Callback<PanelOptions>(o => actualOptions = o);

        var tagHelper = new PanelTagHelper(componentGeneratorMock.Object);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(1, actualOptions!.HeadingLevel);
        Assert.Equal(titleContent, actualOptions.TitleHtml);
        Assert.Equal(bodyContent, actualOptions.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithTitleOnly_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";

        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        PanelOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GeneratePanelAsync(It.IsAny<PanelOptions>())).Callback<PanelOptions>(o => actualOptions = o);

        var tagHelper = new PanelTagHelper(componentGeneratorMock.Object);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(titleContent, actualOptions!.TitleHtml);
        Assert.Null(actualOptions.Html);
        Assert.Null(actualOptions.Text);
    }

    [Fact]
    public async Task ProcessAsync_WithTitleAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var titleDataAttr = "title-data";

        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
                var titleAttributes = new AttributeCollection(new Dictionary<string, string?> { { "data-title", titleDataAttr } });
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), titleAttributes);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        PanelOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GeneratePanelAsync(It.IsAny<PanelOptions>())).Callback<PanelOptions>(o => actualOptions = o);

        var tagHelper = new PanelTagHelper(componentGeneratorMock.Object);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions!.TitleAttributes);
        Assert.Collection(actualOptions.TitleAttributes, kvp =>
        {
            Assert.Equal("data-title", kvp.Key);
            Assert.Equal(titleDataAttr, kvp.Value);
        });
    }

    [Fact]
    public async Task ProcessAsync_WithBodyAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var titleContent = "Title";
        var bodyContent = "Body";
        var bodyDataAttr = "body-data";

        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
                panelContext.SetTitle(TemplateString.FromEncoded(titleContent), null);
                var bodyAttributes = new AttributeCollection(new Dictionary<string, string?> { { "data-body", bodyDataAttr } });
                panelContext.SetBody(TemplateString.FromEncoded(bodyContent), bodyAttributes);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        PanelOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GeneratePanelAsync(It.IsAny<PanelOptions>())).Callback<PanelOptions>(o => actualOptions = o);

        var tagHelper = new PanelTagHelper(componentGeneratorMock.Object);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions!.BodyAttributes);
        Assert.Collection(actualOptions.BodyAttributes, kvp =>
        {
            Assert.Equal("data-body", kvp.Key);
            Assert.Equal(bodyDataAttr, kvp.Value);
        });
    }

    [Fact]
    public async Task ProcessAsync_MissingTitle_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-panel",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-panel",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var panelContext = (PanelContext)context.Items[typeof(PanelContext)];
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
