using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FieldsetTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var describedBy = "describedby";
        var role = "therole";
        var legendText = "Legend text";
        var mainContent = "Main content";

        var context = new TagHelperContext(
            tagName: "govuk-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FieldsetContext>();

                fieldsetContext.SetLegend(
                    isPageHeading: false,
                    attributes: new AttributeCollection(),
                    content: new HtmlString(legendText));

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(mainContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FieldsetOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFieldsetAsync(It.IsAny<FieldsetOptions>())).Callback<FieldsetOptions>(o => actualOptions = o);

        var tagHelper = new FieldsetTagHelper(componentGeneratorMock.Object)
        {
            DescribedBy = describedBy,
            Role = role
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(describedBy, actualOptions!.DescribedBy);
        Assert.Equal(role, actualOptions.Role);
        Assert.Equal(mainContent, actualOptions.Html);
        Assert.NotNull(actualOptions.Legend);
        Assert.False(actualOptions.Legend!.IsPageHeading);
        Assert.Equal(legendText, actualOptions.Legend.Html);
        Assert.NotNull(actualOptions.Legend.Attributes);
        Assert.Empty(actualOptions.Legend.Attributes);
        Assert.Null(actualOptions.Classes);
        Assert.NotNull(actualOptions.Attributes);
        Assert.Empty(actualOptions.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_LegendHasIsPageHeading_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var describedBy = "describedby";
        var role = "therole";
        var legendText = "Legend text";
        var mainContent = "Main content";

        var context = new TagHelperContext(
            tagName: "govuk-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FieldsetContext>();

                fieldsetContext.SetLegend(
                    isPageHeading: true,
                    attributes: new AttributeCollection(),
                    content: new HtmlString(legendText));

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(mainContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FieldsetOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFieldsetAsync(It.IsAny<FieldsetOptions>())).Callback<FieldsetOptions>(o => actualOptions = o);

        var tagHelper = new FieldsetTagHelper(componentGeneratorMock.Object)
        {
            DescribedBy = describedBy,
            Role = role
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(describedBy, actualOptions!.DescribedBy);
        Assert.Equal(role, actualOptions.Role);
        Assert.Equal(mainContent, actualOptions.Html);
        Assert.NotNull(actualOptions.Legend);
        Assert.True(actualOptions.Legend!.IsPageHeading);
        Assert.Equal(legendText, actualOptions.Legend.Html);
        Assert.NotNull(actualOptions.Legend.Attributes);
        Assert.Empty(actualOptions.Legend.Attributes);
    }

    [Fact]
    public async Task ProcessAsync_MissingLegend_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-fieldset",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FieldsetContext>();

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Main content");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();

        var tagHelper = new FieldsetTagHelper(componentGeneratorMock.Object)
        {
            DescribedBy = "describedby",
            Role = "therole"
        };

        tagHelper.Init(context);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("A <govuk-fieldset-legend> element must be provided.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithCustomAttributesAndClasses_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var describedBy = "describedby";
        var role = "therole";
        var legendText = "Legend text";
        var mainContent = "Main content";
        var customClass = "custom-class";
        var dataFooAttrValue = "bar";

        var context = new TagHelperContext(
            tagName: "govuk-fieldset",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-fieldset",
            attributes: new TagHelperAttributeList()
            {
                { "class", customClass },
                { "data-foo", dataFooAttrValue }
            },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var fieldsetContext = context.GetContextItem<FieldsetContext>();

                fieldsetContext.SetLegend(
                    isPageHeading: false,
                    attributes: new AttributeCollection(),
                    content: new HtmlString(legendText));

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(mainContent);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        FieldsetOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateFieldsetAsync(It.IsAny<FieldsetOptions>())).Callback<FieldsetOptions>(o => actualOptions = o);

        var tagHelper = new FieldsetTagHelper(componentGeneratorMock.Object)
        {
            DescribedBy = describedBy,
            Role = role
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(describedBy, actualOptions!.DescribedBy);
        Assert.Equal(role, actualOptions.Role);
        Assert.Equal(mainContent, actualOptions.Html);
        Assert.NotNull(actualOptions.Legend);
        Assert.False(actualOptions.Legend!.IsPageHeading);
        Assert.Equal(legendText, actualOptions.Legend.Html);
        Assert.NotNull(actualOptions.Legend.Attributes);
        Assert.Empty(actualOptions.Legend.Attributes);
        Assert.Equal(customClass, actualOptions.Classes);
        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes, kvp =>
        {
            Assert.Equal("data-foo", kvp.Key);
            Assert.Equal(dataFooAttrValue, kvp.Value);
        });
    }
}
