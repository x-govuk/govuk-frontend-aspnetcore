using GovUk.Frontend.AspNetCore.HtmlGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FormGroupTagHelperBaseTests
{
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, "new", "new")]
    [InlineData("initial", null, "initial")]
    [InlineData("initial", "", "initial")]
    [InlineData("initial", "new", "initial new")]
    public void AppendToDescribedBy(string? initialValue, string? newValue, string? expectedResult)
    {
        // Arrange

        // Act
        var result = FormGroupTagHelperBase.AppendToDescribedBy(initialValue, newValue);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void GenerateErrorMessage_ErrorMessageSetOnContext_ReturnsContent()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetErrorMessage(visuallyHiddenText: "vht", attributes: null, content: new HtmlString("Error message"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test",
            ViewContext = TestUtils.CreateViewContext()
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public void GenerateErrorMessage_AddsErrorToFormErrorContext()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetErrorMessage(visuallyHiddenText: "vht", attributes: null, content: new HtmlString("Error message"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test",
            ViewContext = TestUtils.CreateViewContext()
        };

        // Act
        tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.Collection(
            tagHelper.ViewContext.HttpContext.GetContainerErrorContext().Errors,
            error =>
            {
                Assert.Equal("Error message", error.Html);
            });
    }

    [Fact]
    public void GenerateErrorMessage_ForModelStateHasErrors_ReturnsContent()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = TestUtils.CreateViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetValidationMessage(viewContext, modelExplorer, modelExpression)).Returns("ModelState error");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(errorMessage);
    }

    [Fact]
    public void GenerateErrorMessage_ErrorMessageOnContextAndModelStateUsesContextError_ReturnsContent()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetErrorMessage(visuallyHiddenText: "vht", attributes: null, content: new HtmlString("Context error"));

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = TestUtils.CreateViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetValidationMessage(viewContext, modelExplorer, modelExpression)).Returns("ModelState error");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(errorMessage);
        Assert.Contains("Context error", errorMessage?.ToHtmlString());
    }

    [Fact]
    public void GenerateErrorMessage_ForModelStateHasErrorsButIgnoreModelStateErrorsSet_ReturnsNull()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetValidationMessage(viewContext, modelExplorer, modelExpression)).Returns("ModelState error");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            IgnoreModelStateErrors = true,
            ViewContext = viewContext
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.Null(errorMessage);
    }

    [Fact]
    public void GenerateErrorMessage_NoErrorMessageOnContextOrModelStateErrors_ReturnsNull()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = "Foo";

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetValidationMessage(viewContext, modelExplorer, modelExpression)).Returns((string?)null);

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.Null(errorMessage);
    }

    [Fact]
    public void GenerateErrorMessage_NoErrorMessageOnContextOrFor_ReturnsNull()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.Null(errorMessage);
    }

    [Fact]
    public void GenerateErrorMessage_NonEmptyErrorMessage_AddsErrorMessageIdToDescribedBy()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetErrorMessage(visuallyHiddenText: null, attributes: null, content: new HtmlString("Context error"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test",
            ViewContext = TestUtils.CreateViewContext()
        };

        // Act
        var errorMessage = tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(errorMessage);
        var element = errorMessage!.RenderToElement();
        Assert.Equal("test-error", element.GetAttribute("id"));
        Assert.Contains("test-error", tagHelper.DescribedBy?.Split(' ') ?? Array.Empty<string>());
    }

    [Fact]
    public void GenerateErrorMessage_EmptyErrorMessage_DoesNotAddErrorMessageIdToDescribedBy()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        tagHelper.GenerateErrorMessage(tagHelperContext, formGroupContext);

        // Assert
        Assert.DoesNotContain("test-error", tagHelper.DescribedBy?.Split(' ') ?? Array.Empty<string>());
    }

    [Fact]
    public void GenerateHint_HintOnContext_ReturnsContent()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetHint(attributes: null, content: new HtmlString("Hint"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var hint = tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(hint);
    }

    [Fact]
    public void GenerateHint_NonEmptyModelMetadataDescription_ReturnsContent()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();

        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDescription(modelExplorer)).Returns("ModelState hint");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var hint = tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(hint);
    }

    [Fact]
    public void GenerateHint_HintOnContextAndModelMetadataDescriptionUsesContextHint_ReturnsContent()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetHint(attributes: null, content: new HtmlString("Context hint"));

        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDescription(modelExplorer)).Returns("ModelState hint");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var hint = tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(hint);
        Assert.Contains("Context hint", hint!.ToHtmlString());
    }

    [Fact]
    public void GenerateHint_NoHintOnContextOrModelMetadataDescription_ReturnsNull()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var hint = tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.Null(hint);
    }

    [Fact]
    public void GenerateHint_NonEmptyHint_AddsHintIdToDescribedBy()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetHint(attributes: null, content: new HtmlString("Hint"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var hint = tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.NotNull(hint);
        var element = hint!.RenderToElement();
        Assert.Equal("test-hint", element.GetAttribute("id"));
        Assert.Contains("test-hint", tagHelper.DescribedBy?.Split(' ') ?? Array.Empty<string>());
    }

    [Fact]
    public void GenerateHint_EmptyHint_DoesNotAddHintIdToDescribedBy()
    {
        // Arrange
        var tagHelperContext = new TagHelperContext(
            tagName: "test",
            allAttributes: new TagHelperAttributeList(),
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var formGroupContext = new TestFormGroupContext();

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        tagHelper.GenerateHint(tagHelperContext, formGroupContext);

        // Assert
        Assert.DoesNotContain("test-hint", tagHelper.DescribedBy?.Split(' ') ?? Array.Empty<string>());
    }

    [Fact]
    public void GenerateLabel_NoLabelContentOnContextOrFor_ThrowsInvalidOperationException()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var ex = Record.Exception(() => tagHelper.GenerateLabel(formGroupContext, labelClass: null));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Label content must be specified when the 'asp-for' attribute is not specified.", ex.Message);
    }

    [Fact]
    public void GenerateLabel_LabelContentOnContext_ReturnsContentFromContext()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetLabel(isPageHeading: false, attributes: null, content: new HtmlString("Context label"));

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test"
        };

        // Act
        var label = tagHelper.GenerateLabel(formGroupContext, labelClass: null);

        // Assert
        Assert.NotNull(label);
        Assert.Contains("Context label", label.ToHtmlString());
    }

    [Fact]
    public void GenerateLabel_NoLabelContentOnContext_ReturnsContentFromFor()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDisplayName(modelExplorer, modelExpression)).Returns("ModelMetadata label");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var label = tagHelper.GenerateLabel(formGroupContext, labelClass: null);

        // Assert
        Assert.NotNull(label);
        Assert.Contains("ModelMetadata label", label.ToHtmlString());
    }

    [Fact]
    public void GenerateLabel_LabelContentOnContextAndFor_ReturnsContentFromContext()
    {
        // Arrange
        var formGroupContext = new TestFormGroupContext();
        formGroupContext.SetLabel(isPageHeading: false, attributes: null, content: new HtmlString("Context label"));

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDisplayName(modelExplorer, modelExpression)).Returns("ModelMetadata label");

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = new ModelExpression(modelExpression, modelExplorer),
            Id = "test",
            ViewContext = viewContext
        };

        // Act
        var label = tagHelper.GenerateLabel(formGroupContext, labelClass: null);

        // Assert
        Assert.NotNull(label);
        Assert.Contains("Context label", label.ToHtmlString());
    }

    [Fact]
    public void ResolveFieldsetLegendContent_ContextHasContent_ReturnsContextContent()
    {
        // Arrange
        ModelExpression? @for = null;

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test",
            For = @for
        };

        var fieldsetContext = new TestFormGroupFieldsetContext(
            fieldsetTagName: "test-fieldset",
            legendTagName: "test-fieldset-legend",
            attributes: new AttributeDictionary(),
            @for);

        fieldsetContext.SetLegend(isPageHeading: false, attributes: null, content: new HtmlString("Context name"));

        // Act
        var result = tagHelper.ResolveFieldsetLegendContent(fieldsetContext);

        // Assert
        Assert.Equal("Context name", result.ToHtmlString());
    }

    [Fact]
    public void ResolveFieldsetLegendContent_ContextDoesNotHaveContentButForIsSpecified_ReturnsModelMetadataDisplayName()
    {
        // Arrange
        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDisplayName(modelExplorer, modelExpression)).Returns("ModelMetadata name");

        var @for = new ModelExpression(modelExpression, modelExplorer);

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = @for,
            Id = "test",
            ViewContext = viewContext
        };

        var fieldsetContext = new TestFormGroupFieldsetContext(
            fieldsetTagName: "test-fieldset",
            legendTagName: "test-fieldset-legend",
            attributes: new AttributeDictionary(),
            @for);

        // Act
        var result = tagHelper.ResolveFieldsetLegendContent(fieldsetContext);

        // Assert
        Assert.Equal("ModelMetadata name", result.ToHtmlString());
    }

    [Fact]
    public void ResolveFieldsetLegendContent_ContextHasBothContentAndForIsSpecified_ReturnsContextContent()
    {
        // Arrange
        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.SimpleProperty);

        var modelHelper = new Mock<IModelHelper>();
        modelHelper.Setup(mock => mock.GetDisplayName(modelExplorer, modelExpression)).Returns("ModelMetadata name");

        var @for = new ModelExpression(modelExpression, modelExplorer);

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            For = @for,
            Id = "test",
            ViewContext = viewContext
        };

        var fieldsetContext = new TestFormGroupFieldsetContext(
            fieldsetTagName: "test-fieldset",
            legendTagName: "test-fieldset-legend",
            attributes: new AttributeDictionary(),
            @for);

        fieldsetContext.SetLegend(isPageHeading: false, attributes: null, content: new HtmlString("Context name"));

        // Act
        var result = tagHelper.ResolveFieldsetLegendContent(fieldsetContext);

        // Assert
        Assert.Equal("Context name", result.ToHtmlString());
    }

    [Fact]
    public void ResolveFieldsetLegendContent_ContextHasNoContentAndForNotIsSpecified_ThrowsInvalidOperationException()
    {
        // Arrange
        ModelExpression? @for = null;

        var modelHelper = new Mock<IModelHelper>();

        var tagHelper = new TestFormGroupTagHelper(new ComponentGenerator(), modelHelper.Object)
        {
            Id = "test",
            For = @for
        };

        var fieldsetContext = new TestFormGroupFieldsetContext(
            fieldsetTagName: "test-fieldset",
            legendTagName: "test-fieldset-legend",
            attributes: new AttributeDictionary(),
            @for);

        // Act
        var ex = Record.Exception(() => tagHelper.ResolveFieldsetLegendContent(fieldsetContext));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Fieldset legend content must be specified the 'asp-for' attribute is not specified.", ex.Message);
    }

    private class Model
    {
        public string? SimpleProperty { get; set; }
    }

    private class TestFormGroupTagHelper : FormGroupTagHelperBase
    {
        public TestFormGroupTagHelper(IGovUkHtmlGenerator htmlGenerator, IModelHelper modelHelper) :
            base(htmlGenerator, modelHelper)
        {
        }

        public string? Id { get; set; }

        private protected override FormGroupContext CreateFormGroupContext() => new TestFormGroupContext();

        private protected override IHtmlContent GenerateFormGroupContent(
            TagHelperContext tagHelperContext,
            FormGroupContext formGroupContext,
            TagHelperOutput tagHelperOutput,
            IHtmlContent content,
            out bool haveError)
        {
            var contentBuilder = new HtmlContentBuilder();

            var label = GenerateLabel(formGroupContext, labelClass: null);
            contentBuilder.AppendHtml(label);

            var hint = GenerateHint(tagHelperContext, formGroupContext);
            if (hint is not null)
            {
                contentBuilder.AppendHtml(hint);
            }

            var errorMessage = GenerateErrorMessage(tagHelperContext, formGroupContext);
            if (errorMessage is not null)
            {
                contentBuilder.AppendHtml(errorMessage);
            }

            haveError = errorMessage is not null;

            contentBuilder.Append("Test content");

            return contentBuilder;
        }

        private protected override string ResolveIdPrefix() => Id ?? throw new InvalidOperationException("Id has not been set.");
    }

    private class TestFormGroupContext : FormGroupContext
    {
        protected override string ErrorMessageTagName => "test-error-message";

        protected override string HintTagName => "test-hint";

        protected override string LabelTagName => "test-label";

        protected override string RootTagName => "test";
    }

    private class TestFormGroupFieldsetContext : FormGroupFieldsetContext
    {
        public TestFormGroupFieldsetContext(
            string fieldsetTagName,
            string legendTagName,
            AttributeDictionary attributes,
            ModelExpression? @for)
            : base(fieldsetTagName, legendTagName, attributes, @for)
        {
        }
    }
}
