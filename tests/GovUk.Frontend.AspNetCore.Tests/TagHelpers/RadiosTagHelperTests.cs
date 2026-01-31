using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testradios";
        var hintContent = "The hint";
        var className = "additional-class";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.SetHint(
                    attributes: new AttributeCollection(),
                    html: new TemplateString(hintContent),
                    tagName: RadiosTagHelper.HintTagName);

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                radiosContext.AddItem(new RadiosOptionsItem()
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
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        var name = "testradios";
        var errorContent = "An error";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.SetErrorMessage(
                    visuallyHiddenText: null,
                    attributes: new AttributeCollection(),
                    html: new TemplateString(errorContent),
                    tagName: RadiosTagHelper.ErrorMessageTagName);

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                radiosContext.AddItem(new RadiosOptionsItem()
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
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        Assert.NotNull(actualOptions.ErrorMessage);
        Assert.Equal(errorContent, actualOptions.ErrorMessage.Html);
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

    [Fact]
    public async Task ProcessAsync_WithItemHint_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testradios";
        var itemHintContent = "First item hint";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
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
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        var name = "testradios";
        var conditionalContent = "Item 1 conditional";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Conditional = new RadiosOptionsItemConditional()
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
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        var name = "testradios";
        var conditionalContent = "Item 1 conditional";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("First"),
                    Conditional = new RadiosOptionsItemConditional()
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
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        var name = "testradios";
        var hintContent = "The hint";
        var legendContent = "Legend";
        var describedBy = "described-by-value";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                var radiosFieldsetContext = new RadiosFieldsetContext(describedBy, @for: null);
                radiosContext.OpenFieldset(radiosFieldsetContext, new AttributeCollection());
                radiosFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new TemplateString(legendContent), RadiosFieldsetLegendTagHelper.TagName);

                radiosContext.SetHint(
                    attributes: new AttributeCollection(),
                    html: new TemplateString(hintContent),
                    tagName: RadiosTagHelper.HintTagName);

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                radiosContext.CloseFieldset();

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        Assert.NotNull(actualOptions.Hint);
        Assert.Equal(hintContent, actualOptions.Hint.Html);
        Assert.NotNull(actualOptions.Fieldset);
        Assert.NotNull(actualOptions.Fieldset.Legend);
        Assert.Equal(describedBy, actualOptions.Fieldset.DescribedBy);
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

    [Fact]
    public async Task ProcessAsync_WithItemAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testradios";
        var itemAttributeValue = "custom-value";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                var itemAttributes = new AttributeCollection();
                itemAttributes.Add("data-custom", itemAttributeValue);

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first"),
                    ItemAttributes = itemAttributes
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        Assert.Single(actualOptions.Items!);

        var item = actualOptions.Items!.ElementAt(0);
        Assert.NotNull(item);
        Assert.NotNull(item.ItemAttributes);
        Assert.Equal(itemAttributeValue, item.ItemAttributes["data-custom"]);
    }

    [Fact]
    public async Task ProcessAsync_WithConditionalAttributes_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var idPrefix = "my-id";
        var name = "testradios";
        var conditionalContent = "Item conditional";
        var conditionalAttributeValue = "custom-conditional-value";

        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                var conditionalAttributes = new AttributeCollection();
                conditionalAttributes.Add("data-conditional", conditionalAttributeValue);

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Conditional = new RadiosOptionsItemConditional()
                    {
                        Html = new TemplateString(conditionalContent),
                        Attributes = conditionalAttributes
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        RadiosOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateRadiosAsync(It.IsAny<RadiosOptions>()))
            .Callback<RadiosOptions>(o => actualOptions = o);

        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
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
        Assert.Single(actualOptions.Items!);

        var item = actualOptions.Items!.ElementAt(0);
        Assert.NotNull(item);
        Assert.NotNull(item.Conditional);
        Assert.Equal(conditionalContent, item.Conditional.Html);
        Assert.NotNull(item.Conditional.Attributes);
        Assert.Equal(conditionalAttributeValue, item.Conditional.Attributes["data-conditional"]);
    }
}
