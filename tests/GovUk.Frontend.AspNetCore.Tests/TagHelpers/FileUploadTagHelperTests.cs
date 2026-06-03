using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FileUploadTagHelperTests : TagHelperTestBase<FileUploadTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "my-id";
        var describedBy = "describedby";
        var name = "my-name";
        var disabled = true;
        var multiple = true;
        var labelClass = "additional-label-class";
        var classes = "custom-class";
        var dataFooAttrValue = "foo";
        var labelHtml = "The label";
        var hintHtml = "The hint";
        var wrapperDataBarAttribute = "bar";
        var wrapperClasses = "wrapper-class";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetLabel(
                    isPageHeading: false,
                    attributes: [],
                    labelHtml,
                    FileUploadLabelTagHelper.TagName);

                inputContext.SetHint(
                    attributes: [],
                    hintHtml,
                    FileUploadHintTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            Id = id,
            DescribedBy = describedBy,
            Name = name,
            Multiple = multiple,
            Disabled = disabled,
            LabelClass = labelClass,
            ViewContext = new ViewContext(),
            InputAttributes = new Dictionary<string, string?>()
            {
                { "class", classes },
                { "data-foo", dataFooAttrValue },
            },
            WrapperAttributes = new Dictionary<string, string?>()
            {
                { "class", wrapperClasses },
                { "data-bar", wrapperDataBarAttribute },
            }
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(name, actualOptions.Name);
        Assert.Equal(multiple, actualOptions.Multiple);
        Assert.Equal(disabled, actualOptions.Disabled);
        Assert.Equal(describedBy, actualOptions.DescribedBy);
        Assert.Equal(labelHtml, actualOptions.Label?.Html);
        Assert.Equal(hintHtml, actualOptions.Hint?.Html);
        Assert.Null(actualOptions.ErrorMessage);
        Assert.Equal(classes, actualOptions.Classes);

        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes, kvp =>
        {
            Assert.Equal("data-foo", kvp.Key);
            Assert.Equal(dataFooAttrValue, kvp.Value);
        });

        Assert.Equal(wrapperClasses, actualOptions.WrapperClasses);

        Assert.NotNull(actualOptions.WrapperAttributes);
        Assert.Collection(actualOptions.WrapperAttributes, kvp =>
        {
            Assert.Equal("data-bar", kvp.Key);
            Assert.Equal(wrapperDataBarAttribute, kvp.Value);
        });
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

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetLabel(
                    isPageHeading: false,
                    [],
                    labelHtml,
                    FileUploadLabelTagHelper.TagName);

                inputContext.SetErrorMessage(
                    visuallyHiddenText: errorVht,
                    attributes: new AttributeCollection(
                        new AttributeCollection.Attribute("data-foo", errorDataFooAttribute, Optional: false)),
                    errorHtml,
                    FileUploadErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
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
        Assert.Equal(errorHtml, actualOptions.ErrorMessage.Html);
        Assert.Equal(errorVht, actualOptions.ErrorMessage.VisuallyHiddenText);
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

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
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
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
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
        Assert.Equal(displayName, actualOptions.Label?.Html);
        Assert.Equal(description, actualOptions.Hint?.Html);
        Assert.Equal(modelStateError, actualOptions.ErrorMessage?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndExplicitLabel_UsesSpecifiedLabel()
    {
        // Arrange
        var modelStateValue = "42";
        var modelStateDisplayName = "ModelState label";
        var labelHtml = "Explicit label";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetLabel(
                    isPageHeading: false,
                    [],
                    labelHtml,
                    FileUploadLabelTagHelper.TagName);

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
            .Returns(modelStateDisplayName);

        modelHelperMock
            .Setup(mock => mock.GetModelValue(
                /*viewContext: */It.IsAny<ViewContext>(),
                /*modelExplorer: */It.IsAny<ModelExplorer>(),
                /*expression: */It.IsAny<string>()))
            .Returns(modelStateValue);

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            ViewContext = new ViewContext(),
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(labelHtml, actualOptions?.Label?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndExplicitHint_UsesSpecifiedHint()
    {
        // Arrange
        var modelStateValue = "42";
        var displayName = "The label";
        var modelStateDescription = "The hint";
        var hintHtml = "Explicit hint";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetHint([], hintHtml, FileUploadHintTagHelper.TagName);

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
            .Returns(modelStateDescription);

        modelHelperMock
            .Setup(mock => mock.GetModelValue(
                /*viewContext: */It.IsAny<ViewContext>(),
                /*modelExplorer: */It.IsAny<ModelExplorer>(),
                /*expression: */It.IsAny<string>()))
            .Returns(modelStateValue);

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            ViewContext = new ViewContext(),
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(hintHtml, actualOptions?.Hint?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndExplicitErrorMessage_UsesSpecifiedErrorMessage()
    {
        // Arrange
        var modelStateValue = "42";
        var displayName = "The label";
        var modelStateError = "ModelState error";
        var errorHtml = "Explicit error";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetErrorMessage(
                    visuallyHiddenText: null,
                    [],
                    errorHtml,
                    FileUploadErrorMessageTagHelper.TagName);

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
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(errorHtml, actualOptions?.ErrorMessage?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndNoExplicitErrorMessageAndIgnoreModelStateErrorTrue_DoesNotRenderErrorMessage()
    {
        // Arrange
        var modelStateValue = "42";
        var displayName = "The label";
        var modelStateError = "ModelState error";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
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
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            ViewContext = new ViewContext(),
            IgnoreModelStateErrors = true
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(actualOptions?.ErrorMessage);
    }

    [Fact]
    public async Task ProcessAsync_WithForAndAndExplicitErrorMessageAndIgnoreModelStateErrorTrue_DoesRenderErrorMessage()
    {
        // Arrange
        var modelStateValue = "42";
        var displayName = "The label";
        var modelStateError = "ModelState error";
        var errorHtml = "Explicit error";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetErrorMessage(
                    visuallyHiddenText: null,
                    [],
                    errorHtml,
                    FileUploadErrorMessageTagHelper.TagName);

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
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            For = new ModelExpression(nameof(Model.SimpleProperty), modelExplorer),
            IgnoreModelStateErrors = true,
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal(errorHtml, actualOptions?.ErrorMessage?.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithError_AddsErrorWithCorrectFieldIdToContainerErrorContext()
    {
        // Arrange
        var id = "my-id";
        var name = "my-name";
        var labelHtml = "The label";
        var errorHtml = "The error message";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetLabel(
                    isPageHeading: false,
                    [],
                    labelHtml,
                    FileUploadLabelTagHelper.TagName);

                inputContext.SetErrorMessage(
                    visuallyHiddenText: null,
                    [],
                    errorHtml,
                    FileUploadErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            Id = id,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            tagHelper.ViewContext.HttpContext.GetPageErrorContext().Errors,
            error =>
            {
                Assert.Equal(errorHtml, error.Html);
                Assert.Equal($"#{id}", error.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithTextProperties_GeneratesOptionsWithTextProperties()
    {
        // Arrange
        var id = "my-id";
        var name = "my-name";
        var labelHtml = "The label";
        var chooseFilesButtonText = "Choose files";
        var dropInstructionText = "Drop files here";
        var enteredDropZoneText = "You are in the drop zone";
        var leftDropZoneText = "You have left the drop zone";
        var multipleFilesChosenTextOne = "1 file chosen";
        var multipleFilesChosenTextOther = "{count} files chosen";
        var noFileChosenText = "No file chosen";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var inputContext = context.GetContextItem<FileUploadContext>();

                inputContext.SetLabel(
                    isPageHeading: false,
                    attributes: [],
                    labelHtml,
                    FileUploadLabelTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FileUploadOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFileUploadAsync(It.IsAny<FileUploadOptions>())).Callback<FileUploadOptions>(o => actualOptions = o);

        var tagHelper = new FileUploadTagHelper(componentGeneratorMock.Object, modelHelperMock.Object)
        {
            Id = id,
            Name = name,
            ViewContext = new ViewContext(),
            ChooseFilesButtonText = chooseFilesButtonText,
            DropInstructionText = dropInstructionText,
            EnteredDropZoneText = enteredDropZoneText,
            LeftDropZoneText = leftDropZoneText,
            MultipleFilesChosenTextOne = multipleFilesChosenTextOne,
            MultipleFilesChosenTextOther = multipleFilesChosenTextOther,
            NoFileChosenText = noFileChosenText,
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(chooseFilesButtonText, actualOptions.ChooseFilesButtonText);
        Assert.Equal(dropInstructionText, actualOptions.DropInstructionText);
        Assert.Equal(enteredDropZoneText, actualOptions.EnteredDropZoneText);
        Assert.Equal(leftDropZoneText, actualOptions.LeftDropZoneText);
        Assert.NotNull(actualOptions.MultipleFilesChosenText);
        Assert.Equal(multipleFilesChosenTextOne, actualOptions.MultipleFilesChosenText.One);
        Assert.Equal(multipleFilesChosenTextOther, actualOptions.MultipleFilesChosenText.Other);
        Assert.Equal(noFileChosenText, actualOptions.NoFileChosenText);
    }

    private class Model
    {
        public string? SimpleProperty { get; set; }
    }
}
