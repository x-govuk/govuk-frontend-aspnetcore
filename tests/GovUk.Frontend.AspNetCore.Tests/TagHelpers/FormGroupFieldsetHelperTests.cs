using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

// The fieldset resolution is shared by the checkboxes, date input and radios components;
// CheckboxesContext is used here as a representative implementation of IFormGroupWithFieldset.
public class FormGroupFieldsetHelperTests
{
    private const string DisplayName = "The legend from the model";

    [Fact]
    public void GetFieldsetOptions_NoFieldsetOrLegendSpecified_ReturnsNull()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var options = GetFieldsetOptions(context);

        // Assert
        Assert.Null(options);
    }

    [Fact]
    public void GetFieldsetOptions_WithExplicitFieldsetElement_ReturnsOptionsFromFieldsetElement()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        OpenAndCloseFieldset(context, legendHtml: "Legend", fieldsetClassName: "fieldset-class");

        // Act
        var options = GetFieldsetOptions(context);

        // Assert
        Assert.NotNull(options);
        Assert.Equal("fieldset-class", options.Classes?.ToHtmlString());
        Assert.Equal("Legend", options.Legend?.Html?.ToHtmlString());
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendElementOnly_GeneratesFieldset()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        SetImplicitLegend(context, "Legend");

        // Act
        var options = GetFieldsetOptions(context);

        // Assert
        Assert.NotNull(options);
        Assert.Equal("Legend", options.Legend?.Html?.ToHtmlString());
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendElementAndFieldsetAttributes_AddsAttributesToGeneratedFieldset()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        SetImplicitLegend(context, "Legend");

        // Act
        var options = GetFieldsetOptions(
            context,
            fieldsetAttributes: new AttributeCollection(new Dictionary<string, string?>()
            {
                { "class", "generated-fieldset-class" },
                { "data-foo", "bar" }
            }));

        // Assert
        Assert.NotNull(options);
        Assert.Equal("generated-fieldset-class", options.Classes?.ToHtmlString());
        Assert.Contains(options.Attributes!, a => a.Key == "data-foo" && a.Value?.ToHtmlString() == "bar");
        Assert.Equal("Legend", options.Legend?.Html?.ToHtmlString());
    }

    [Fact]
    public void GetFieldsetOptions_WithFieldsetAttributeAndFor_GeneratesFieldsetWithLegendFromModelMetadata()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var options = GetFieldsetOptions(context, generateFieldset: true);

        // Assert
        Assert.NotNull(options);
        Assert.Equal(DisplayName, options.Legend?.Text);
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendAttributesAndFor_GeneratesFieldsetWithLegendFromModelMetadata()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var options = GetFieldsetOptions(
            context,
            legendAttributes: new AttributeCollection(new Dictionary<string, string?>()
            {
                { "class", "generated-legend-class" }
            }));

        // Assert
        Assert.NotNull(options);
        Assert.Equal(DisplayName, options.Legend?.Text);
        Assert.Equal("generated-legend-class", options.Legend?.Classes?.ToHtmlString());
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendIsPageHeadingAndFor_GeneratesFieldsetWithPageHeadingLegend()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var options = GetFieldsetOptions(context, legendIsPageHeading: true);

        // Assert
        Assert.NotNull(options);
        Assert.Equal(DisplayName, options.Legend?.Text);
        Assert.True(options.Legend?.IsPageHeading);
    }

    [Fact]
    public void GetFieldsetOptions_WithIsPageHeadingInLegendAttributes_GeneratesFieldsetWithPageHeadingLegend()
    {
        // Arrange
        // Razor may bind 'legend-is-page-heading' to the 'legend-*' dictionary rather than the property
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var options = GetFieldsetOptions(
            context,
            legendAttributes: new AttributeCollection(new Dictionary<string, string?>()
            {
                { "is-page-heading", "true" }
            }));

        // Assert
        Assert.NotNull(options);
        Assert.True(options.Legend?.IsPageHeading);
        Assert.DoesNotContain(options.Legend!.Attributes!, a => a.Key == "is-page-heading");
    }

    [Fact]
    public void GetFieldsetOptions_WithInvalidIsPageHeadingInLegendAttributes_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: CreateModelExpression());

        // Act
        var ex = Record.Exception(() => GetFieldsetOptions(
            context,
            legendAttributes: new AttributeCollection(new Dictionary<string, string?>()
            {
                { "is-page-heading", "yes" }
            })));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'legend-is-page-heading' attribute must be 'true' or 'false'.", ex.Message);
    }

    [Fact]
    public void GetFieldsetOptions_WithFieldsetAttributeButNoLegendElementOrFor_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        // Act
        var ex = Record.Exception(() => GetFieldsetOptions(context, generateFieldset: true));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-checkboxes-fieldset-legend> element must be provided.", ex.Message);
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendElementOutsideOfExplicitFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        OpenAndCloseFieldset(context, legendHtml: "Legend");
        SetImplicitLegend(context, "Another legend");

        // Act
        var ex = Record.Exception(() => GetFieldsetOptions(context));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "<govuk-checkboxes-fieldset-legend> must be inside <govuk-checkboxes-fieldset>.",
            ex.Message);
    }

    [Fact]
    public void GetFieldsetOptions_WithExplicitFieldsetAndFieldsetAttributes_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        OpenAndCloseFieldset(context, legendHtml: "Legend");

        // Act
        var ex = Record.Exception(() => GetFieldsetOptions(context, generateFieldset: true));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal(
            "'fieldset' and 'legend-*' attributes cannot be specified on <govuk-checkboxes> when a <govuk-checkboxes-fieldset> element is used.",
            ex.Message);
    }

    [Fact]
    public void GetFieldsetOptions_WithLegendElementAndLegendAttributes_CombinesAttributes()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var legendElementAttributes = new AttributeCollection(new Dictionary<string, string?>()
        {
            { "class", "legend-element-class" },
            { "data-from-element", "yes" }
        });

        context.ImplicitFieldset.SetLegend(
            isPageHeading: null,
            attributes: legendElementAttributes,
            html: new HtmlString("Legend"),
            CheckboxesFieldsetLegendTagHelper.TagName);

        // Act
        var options = GetFieldsetOptions(
            context,
            legendAttributes: new AttributeCollection(new Dictionary<string, string?>()
            {
                { "class", "root-legend-class" },
                { "data-from-root", "yes" }
            }));

        // Assert
        Assert.NotNull(options);
        Assert.Equal("root-legend-class legend-element-class", options.Legend?.Classes?.ToHtmlString());
        Assert.Contains(options.Legend!.Attributes!, a => a.Key == "data-from-root" && a.Value?.ToHtmlString() == "yes");
        Assert.Contains(options.Legend!.Attributes!, a => a.Key == "data-from-element" && a.Value?.ToHtmlString() == "yes");
    }

    [Fact]
    public void GetFieldsetOptions_WithConflictingLegendAttributes_LegendElementAttributeWins()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        context.ImplicitFieldset.SetLegend(
            isPageHeading: null,
            attributes: new AttributeCollection(new Dictionary<string, string?>() { { "data-foo", "from-element" } }),
            html: new HtmlString("Legend"),
            CheckboxesFieldsetLegendTagHelper.TagName);

        // Act
        var options = GetFieldsetOptions(
            context,
            legendAttributes: new AttributeCollection(new Dictionary<string, string?>() { { "data-foo", "from-root" } }));

        // Assert
        Assert.NotNull(options);
        Assert.Contains(options.Legend!.Attributes!, a => a.Key == "data-foo" && a.Value?.ToHtmlString() == "from-element");
    }

    private static FieldsetOptions? GetFieldsetOptions(
        CheckboxesContext context,
        bool generateFieldset = false,
        AttributeCollection? fieldsetAttributes = null,
        AttributeCollection? legendAttributes = null,
        bool? legendIsPageHeading = null) =>
        context.GetFieldsetOptions(
            CreateModelHelper(),
            generateFieldset,
            fieldsetAttributes ?? new AttributeCollection(),
            legendAttributes ?? new AttributeCollection(),
            legendIsPageHeading);

    private static void OpenAndCloseFieldset(
        CheckboxesContext context,
        string? legendHtml = null,
        string? fieldsetClassName = null)
    {
        var attributes = new AttributeCollection();

        if (fieldsetClassName is not null)
        {
            attributes.Add("class", new TemplateString(fieldsetClassName));
        }

        var fieldsetContext = new FormGroupFieldsetContext2(CheckboxesFieldsetTagHelper.TagName);
        context.OpenFieldset(fieldsetContext, attributes);

        if (legendHtml is not null)
        {
            fieldsetContext.SetLegend(
                isPageHeading: null,
                attributes: new AttributeCollection(),
                html: new HtmlString(legendHtml),
                CheckboxesFieldsetLegendTagHelper.TagName);
        }

        context.CloseFieldset();
    }

    private static void SetImplicitLegend(CheckboxesContext context, string html) =>
        context.ImplicitFieldset.SetLegend(
            isPageHeading: null,
            attributes: new AttributeCollection(),
            html: new HtmlString(html),
            CheckboxesFieldsetLegendTagHelper.TagName);

    private static ModelExpression CreateModelExpression()
    {
        var modelExplorer = new EmptyModelMetadataProvider()
            .GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.SimpleProperty));

        return new ModelExpression(nameof(Model.SimpleProperty), modelExplorer);
    }

    private static IModelHelper CreateModelHelper()
    {
        var modelHelperMock = new Mock<IModelHelper>();

        modelHelperMock
            .Setup(mock => mock.GetDisplayName(It.IsAny<ModelExplorer>(), It.IsAny<string>()))
            .Returns(DisplayName);

        return modelHelperMock.Object;
    }

    private class Model
    {
        public string? SimpleProperty { get; set; }
    }
}
