using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var hintContent = "The hint";
        var className = "additional-class";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.SetHint(
                    attributes: new AttributeCollection(),
                    html: new TemplateString(hintContent),
                    tagName: CheckboxesTagHelper.HintTagName);

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        output.Attributes.Add("class", className);

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.Equal(hintContent, actualOptions.Hint?.Html);
        Assert.Null(actualOptions.ErrorMessage);
        Assert.Null(actualOptions.Fieldset);
        Assert.Equal(2, actualOptions.Items?.Count);
        Assert.Equal(className, actualOptions.FormGroup?.Classes);

        var firstItem = actualOptions.Items?.ElementAt(0);
        Assert.NotNull(firstItem);
        Assert.Equal("First", firstItem.Html);
        Assert.Equal("first", firstItem.Id);
        Assert.Equal("first", firstItem.Value);
        Assert.False(firstItem.Checked);
        Assert.True(firstItem.Disabled);

        var secondItem = actualOptions.Items?.ElementAt(1);
        Assert.NotNull(secondItem);
        Assert.Equal("Second", secondItem.Html);
        Assert.Equal("second", secondItem.Id);
        Assert.Equal("second", secondItem.Value);
        Assert.True(secondItem.Checked);
        Assert.False(secondItem.Disabled);
    }

    [Fact]
    public async Task ProcessAsync_WithErrorMessage_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var errorContent = "An error";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.SetErrorMessage(
                    visuallyHiddenText: null,
                    attributes: new AttributeCollection(),
                    html: new TemplateString(errorContent),
                    tagName: CheckboxesTagHelper.ErrorMessageTagName);

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.Null(actualOptions.Hint);
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(errorContent, actualOptions.ErrorMessage.Html);
        Assert.Null(actualOptions.Fieldset);
        Assert.Equal(2, actualOptions.Items?.Count);
    }

    [Fact]
    public async Task ProcessAsync_WithItemHint_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var itemHintContent = "First item hint";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Hint = new HintOptions()
                    {
                        Html = new TemplateString(itemHintContent)
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.Single(actualOptions.Items!);

        var item = actualOptions.Items!.ElementAt(0);
        Assert.NotNull(item);
        Assert.Equal("First", item.Html);
        Assert.Equal("first", item.Id);
        Assert.Equal("first", item.Value);
        Assert.NotNull(item.Hint);
        Assert.Equal(itemHintContent, item.Hint.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithUncheckedItemConditional_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var conditionalContent = "Item 1 conditional";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Conditional = new CheckboxesOptionsItemConditional()
                    {
                        Html = new TemplateString(conditionalContent)
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.Single(actualOptions.Items!);

        var item = actualOptions.Items!.ElementAt(0);
        Assert.NotNull(item);
        Assert.Equal("First", item.Html);
        Assert.Equal("first", item.Id);
        Assert.Equal("first", item.Value);
        Assert.True(item.Checked != true);
        Assert.NotNull(item.Conditional);
        Assert.Equal(conditionalContent, item.Conditional.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithCheckedItemConditional_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var conditionalContent = "Item 1 conditional";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("First"),
                    Conditional = new CheckboxesOptionsItemConditional()
                    {
                        Html = new TemplateString(conditionalContent)
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.Single(actualOptions.Items!);

        var item = actualOptions.Items!.ElementAt(0);
        Assert.NotNull(item);
        Assert.Equal("First", item.Html);
        Assert.Equal("first", item.Id);
        Assert.Equal("first", item.Value);
        Assert.True(item.Checked);
        Assert.NotNull(item.Conditional);
        Assert.Equal(conditionalContent, item.Conditional.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithFieldset_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testcheckboxes";
        var describedBy = "describedby";
        var hintContent = "The hint";
        var legendContent = "Legend";

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.OpenFieldset();
                var checkboxesFieldsetContext = new CheckboxesFieldsetContext(describedBy: null, attributes: new AttributeCollection(), @for: null);
                checkboxesFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new HtmlString(legendContent));

                checkboxesContext.SetHint(
                    attributes: new AttributeCollection(),
                    html: new TemplateString(hintContent),
                    tagName: CheckboxesTagHelper.HintTagName);

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                checkboxesContext.CloseFieldset(checkboxesFieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        CheckboxesOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateCheckboxesAsync(It.IsAny<CheckboxesOptions>()))
            .Callback<CheckboxesOptions>(o => actualOptions = o);

        var tagHelper = new CheckboxesTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            DescribedBy = describedBy,
            IdPrefix = idPrefix,
            Name = name,
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.Equal(idPrefix, actualOptions.IdPrefix);
        Assert.Equal(name, actualOptions.Name);
        Assert.NotNull(actualOptions.Hint);
        Assert.Equal(hintContent, actualOptions.Hint.Html);
        Assert.NotNull(actualOptions.Fieldset);
        Assert.NotNull(actualOptions.Fieldset.Legend);
        Assert.Equal(describedBy, actualOptions.DescribedBy);
        Assert.Equal(legendContent, actualOptions.Fieldset.Legend.Html);
        Assert.Equal(2, actualOptions.Items?.Count);

        var firstItem = actualOptions.Items?.ElementAt(0);
        Assert.NotNull(firstItem);
        Assert.Equal("First", firstItem.Html);
        Assert.Equal("first", firstItem.Id);
        Assert.Equal("first", firstItem.Value);
        Assert.False(firstItem.Checked);
        Assert.True(firstItem.Disabled);

        var secondItem = actualOptions.Items?.ElementAt(1);
        Assert.NotNull(secondItem);
        Assert.Equal("Second", secondItem.Html);
        Assert.Equal("second", secondItem.Id);
        Assert.Equal("second", secondItem.Value);
        Assert.True(secondItem.Checked);
        Assert.False(secondItem.Disabled);
    }
}
