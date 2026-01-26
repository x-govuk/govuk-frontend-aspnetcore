using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class SelectTagHelperTests : TagHelperTestBase<SelectTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "my-id";
        var describedBy = "describedby";
        var name = "my-name";
        var disabled = true;
        var labelClass = "additional-label-class";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var dataFooAttrValue = "foo";
        var labelContent = "The label";
        var hintContent = "The hint";

        var context = CreateTagHelperContext(className: className, attributes: attributes);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var selectContext = context.GetContextItem<SelectContext>();

                selectContext.SetLabel(
                    isPageHeading: false,
                    attributes: [],
                    labelContent,
                    SelectTagHelper.LabelTagName);

                selectContext.SetHint(
                    attributes: [],
                    hintContent,
                    SelectTagHelper.HintTagName);

                selectContext.AddItem(new SelectItem()
                {
                    Content = new HtmlString("First")
                });

                selectContext.AddItem(new SelectItem()
                {
                    Content = new HtmlString("Second"),
                    Value = "second",
                    Selected = true
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        SelectOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateSelectAsync(It.IsAny<SelectOptions>())).Callback<SelectOptions>(o => actualOptions = o);

        var tagHelper = new SelectTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            Id = id,
            DescribedBy = describedBy,
            Name = name,
            Disabled = disabled,
            LabelClass = labelClass,
            ViewContext = new ViewContext(),
            SelectAttributes = new Dictionary<string, string?>()
            {
                { "class", className },
                { "data-foo", dataFooAttrValue },
            }
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(id, actualOptions.Id?.ToHtmlString());
        Assert.Equal(name, actualOptions.Name?.ToHtmlString());
        Assert.Equal(describedBy, actualOptions.DescribedBy?.ToHtmlString());
        Assert.Equal(labelContent, actualOptions.Label?.Html?.ToHtmlString());
        Assert.Equal(hintContent, actualOptions.Hint?.Html?.ToHtmlString());
        Assert.Null(actualOptions.ErrorMessage);
        Assert.Equal(className, actualOptions.Classes?.ToHtmlString());

        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes,
            kvp =>
            {
                Assert.Equal("disabled", kvp.Key);
            },
            kvp =>
            {
                Assert.Equal("data-foo", kvp.Key);
                Assert.Equal(dataFooAttrValue, kvp.Value);
            });

        Assert.NotNull(actualOptions.Items);
        Assert.Equal(2, actualOptions.Items.Count);
        Assert.Equal("First", actualOptions.Items.First().Text);
        Assert.Equal("Second", actualOptions.Items.Last().Text);
        Assert.Equal("second", actualOptions.Items.Last().Value?.ToHtmlString());
        Assert.True(actualOptions.Items.Last().Selected);
    }

    [Fact]
    public async Task ProcessAsync_WithErrorMessage_GeneratesOptionsWithErrorMessage()
    {
        // Arrange
        var id = "my-id";
        var name = "my-name";
        var labelHtml = "The label";
        var errorHtml = "The error message";
        var errorVht = "visually hidden text";
        var errorDataFooAttribute = "bar";

        var context = new TagHelperContext(
            tagName: "govuk-select",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-select",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var selectContext = context.GetContextItem<SelectContext>();

                selectContext.SetLabel(
                    isPageHeading: false,
                    [],
                    new HtmlString(labelHtml),
                    SelectTagHelper.LabelTagName);

                selectContext.SetErrorMessage(
                    visuallyHiddenText: new HtmlString(errorVht),
                    attributes: new AttributeCollection()
                    {
                        { "data-foo", errorDataFooAttribute }
                    },
                    new HtmlString(errorHtml),
                    SelectTagHelper.ErrorMessageTagName);

                selectContext.AddItem(new SelectItem()
                {
                    Content = new HtmlString("First")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        SelectOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateSelectAsync(It.IsAny<SelectOptions>())).Callback<SelectOptions>(o => actualOptions = o);

        var tagHelper = new SelectTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            Id = id,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions?.ErrorMessage);
        Assert.Equal(errorHtml, actualOptions.ErrorMessage.Html?.ToHtmlString());
        Assert.Equal(errorVht, actualOptions.ErrorMessage.VisuallyHiddenText?.ToHtmlString());
        Assert.NotNull(actualOptions.ErrorMessage.Attributes);
        Assert.Collection(actualOptions.ErrorMessage.Attributes, kvp =>
        {
            Assert.Equal("data-foo", kvp.Key);
            Assert.Equal(errorDataFooAttribute, kvp.Value);
        });
    }

    [Fact]
    public async Task ProcessAsync_WithFor_GeneratesOptionsFromModelMetadata()
    {
        // Arrange
        var modelStateValue = "42";
        var displayName = "The label";
        var description = "The hint";
        var modelStateError = "The error message";

        var context = new TagHelperContext(
            tagName: "govuk-select",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-select",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var selectContext = context.GetContextItem<SelectContext>();

                selectContext.AddItem(new SelectItem()
                {
                    Content = new HtmlString("First"),
                    Value = "1"
                });

                selectContext.AddItem(new SelectItem()
                {
                    Content = new HtmlString("Second"),
                    Value = modelStateValue
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        modelHelperMock
            .Setup(mock => mock.GetFullHtmlFieldName(
                /*viewContext: */It.IsAny<ViewContext>(),
                /*expression: */It.IsAny<string>()))
            .Returns(nameof(Model.SimpleProperty));

        modelHelperMock
            .Setup(mock => mock.GetDisplayName(
                /*modelExplorer: */It.IsAny<ModelExplorer>(),
                /*expression: */It.IsAny<string>()))
            .Returns(displayName);

        modelHelperMock
            .Setup(mock => mock.GetDescription(/*modelExplorer: */It.IsAny<ModelExplorer>()))
            .Returns(description);

        modelHelperMock
            .Setup(mock => mock.GetValidationMessage(
                /*viewContext: */It.IsAny<ViewContext>(),
                /*modelExplorer: */It.IsAny<ModelExplorer>(),
                /*expression: */It.IsAny<string>()))
            .Returns(modelStateError);

        modelHelperMock
            .Setup(mock => mock.GetModelValue(
                /*viewContext: */It.IsAny<ViewContext>(),
                /*modelExplorer: */It.IsAny<ModelExplorer>(),
                /*expression: */It.IsAny<string>()))
            .Returns(modelStateValue);

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        SelectOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateSelectAsync(It.IsAny<SelectOptions>())).Callback<SelectOptions>(o => actualOptions = o);

        var tagHelper = new SelectTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions.Id);
        Assert.NotNull(actualOptions.Name);
        Assert.Equal(displayName, actualOptions.Label?.Html?.ToHtmlString());
        Assert.Equal(description, actualOptions.Hint?.Html?.ToHtmlString());
        Assert.Equal(modelStateError, actualOptions.ErrorMessage?.Html?.ToHtmlString());
    }

    private class Model
    {
        public string? SimpleProperty { get; set; }
    }
}
