using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class TextAreaTagHelperTests : TagHelperTestBase<TextAreaTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var id = "my-id";
        var describedBy = "describedby";
        var name = "my-name";
        var autocomplete = "none";
        var spellcheck = false;
        var rows = 10;
        var value = "test value";
        var disabled = true;
        var readOnly = true;
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
                var textAreaContext = context.GetContextItem<TextAreaContext>();

                textAreaContext.SetLabel(
                    isPageHeading: false,
                    attributes: [],
                    new TemplateString(labelContent),
                    TextAreaLabelTagHelper.TagName);

                textAreaContext.SetHint(
                    attributes: [],
                    new TemplateString(hintContent),
                    TextAreaHintTagHelper.TagName);

                textAreaContext.SetValue(new TemplateString(value), TextAreaValueTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TextareaOptions>(nameof(IComponentGenerator.GenerateTextareaAsync));

        var tagHelper = new TextAreaTagHelper(componentGenerator, modelHelperMock.Object)
        {
            Id = id,
            DescribedBy = describedBy,
            Name = name,
            AutoComplete = autocomplete,
            Spellcheck = spellcheck,
            Rows = rows,
            Disabled = disabled,
            ReadOnly = readOnly,
            LabelClass = labelClass,
            ViewContext = new ViewContext(),
            TextAreaAttributes = new Dictionary<string, string?>()
            {
                { "class", className },
                { "data-foo", dataFooAttrValue },
            }
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(id, actualOptions.Id);
        Assert.Equal(name, actualOptions.Name);
        Assert.Equal(rows, actualOptions.Rows);
        Assert.Equal(value, actualOptions.Value);
        Assert.Equal(disabled, actualOptions.Disabled);
        Assert.Equal(describedBy, actualOptions.DescribedBy);
        Assert.Equal(labelContent, actualOptions.Label?.Html?.ToHtmlString());
        Assert.Equal(hintContent, actualOptions.Hint?.Html?.ToHtmlString());
        Assert.Null(actualOptions.ErrorMessage);
        Assert.Equal(className, actualOptions.Classes);
        Assert.Equal(autocomplete, actualOptions.AutoComplete);
        Assert.Equal(spellcheck, actualOptions.Spellcheck);

        Assert.NotNull(actualOptions.Attributes);
        Assert.Collection(actualOptions.Attributes,
            kvp => Assert.Equal("readonly", kvp.Key),
            kvp =>
            {
                Assert.Equal("data-foo", kvp.Key);
                Assert.Equal(dataFooAttrValue, kvp.Value);
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

        var context = new TagHelperContext(
            tagName: "govuk-textarea",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-textarea",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var textAreaContext = context.GetContextItem<TextAreaContext>();

                textAreaContext.SetLabel(
                    isPageHeading: false,
                    [],
                    new HtmlString(labelHtml),
                    TextAreaLabelTagHelper.TagName);

                textAreaContext.SetErrorMessage(
                    visuallyHiddenText: new HtmlString(errorVht).ToHtmlString(),
                    attributes: new AttributeCollection { { "data-foo", errorDataFooAttribute } },
                    html: new HtmlString(errorHtml),
                    TextAreaErrorMessageTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelHelperMock = new Mock<IModelHelper>();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TextareaOptions>(nameof(IComponentGenerator.GenerateTextareaAsync));

        var tagHelper = new TextAreaTagHelper(componentGenerator, modelHelperMock.Object)
        {
            Id = id,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(errorVht, actualOptions.ErrorMessage.VisuallyHiddenText);
        Assert.Equal(errorHtml, actualOptions.ErrorMessage.Html?.ToHtmlString());
        Assert.Equal(errorDataFooAttribute, actualOptions.ErrorMessage.Attributes?["data-foo"]);
    }

    private class Model
    {
        public string? Foo { get; set; }
    }

    [Xunit.Fact]
    public async Task ProcessAsync_WithBeforeAndAfterInputContent_PassesItToTheComponentGenerator()
    {
        // The context stores this content as IHtmlContent, so a type test against a concrete type
        // here silently drops it -- which is how it got dropped for every component at once.

        // Arrange
        var beforeInputContent = "Before input";
        var afterInputContent = "After input";

        var context = CreateTagHelperContext(tagName: TextAreaTagHelper.TagName);

        var output = CreateTagHelperOutput(
            tagName: TextAreaTagHelper.TagName,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var textAreaContext = context.GetContextItem<TextAreaContext>();

                textAreaContext.SetLabel(isPageHeading: false, attributes: [], new TemplateString("The label"), TextAreaLabelTagHelper.TagName);
                textAreaContext.SetBeforeInput(new HtmlString(beforeInputContent), TextAreaBeforeInputTagHelper.TagName);
                textAreaContext.SetAfterInput(new HtmlString(afterInputContent), TextAreaAfterInputTagHelper.TagName);

                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<TextareaOptions>(nameof(IComponentGenerator.GenerateTextareaAsync));

        var tagHelper = new TextAreaTagHelper(componentGenerator, new DefaultModelHelper())
        {
            Name = "my-name",
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.Equal(beforeInputContent, actualOptions.FormGroup?.BeforeInput?.Html?.ToHtmlString());
        Assert.Equal(afterInputContent, actualOptions.FormGroup?.AfterInput?.Html?.ToHtmlString());
    }
}
