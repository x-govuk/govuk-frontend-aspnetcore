using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesItemTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Checked = true,
            Disabled = true,
            Id = "id",
            Name = "name",
            Value = "value"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.True(checkboxesItem.Checked);
                Assert.True(checkboxesItem.Disabled);
                Assert.Equal("id", checkboxesItem.Id);
                Assert.Equal("name", checkboxesItem.Name);
                Assert.Equal("value", checkboxesItem.Value);
            });
    }

    [Fact]
    public async Task ProcessAsync_NoValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper();

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'value' attribute must be specified.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_NoName_ThrowsInvalidOperationException()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Value = "value"
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("The 'name' attribute must be specified on each item when not specified on the parent <govuk-checkboxes>.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_NoNameButParentHasName_DoesNotThrowInvalidOperationException()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "parent", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Value = "value"
        };

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ProcessAsync_WithNullOrEmptySimpleModelExpression_IsNotChecked(string? modelValue)
    {
        // Arrange
        var model = new Model()
        {
            Foo = modelValue
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), model)
            .GetExplorerForProperty(nameof(Model.Foo));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.Foo);

        var checkboxesContext = new CheckboxesContext(name: "test", @for: new ModelExpression(modelExpression, modelExplorer));

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Checked = null,
            Value = "",
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.False(checkboxesItem.Checked);
            });
    }

    [Theory]
    [InlineData("bar", true)]
    [InlineData("baz", false)]
    public async Task ProcessAsync_WithSimpleModelExpression_DeducesCheckedFromModelExpression(string modelValue, bool expectedChecked)
    {
        // Arrange
        var model = new Model()
        {
            Foo = modelValue
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), model)
            .GetExplorerForProperty(nameof(Model.Foo));
        var viewContext = new ViewContext();
        var modelExpression = nameof(Model.Foo);

        var checkboxesContext = new CheckboxesContext(name: "test", @for: new ModelExpression(modelExpression, modelExplorer));

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Checked = null,
            Value = "bar",
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal(expectedChecked, checkboxesItem.Checked);
            });
    }

    [Theory]
    [InlineData(new[] { 2, 3 }, "3", true)]
    [InlineData(new[] { 2, 3 }, "4", false)]
    public async Task ProcessAsync_WithCollectionModelExpression_DeducesCheckedFromModelExpression(
        int[] modelValues,
        string itemValue,
        bool expectedChecked)
    {
        // Arrange
        var model = new ModelWithCollectionProperty()
        {
            CollectionProperty = modelValues
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(ModelWithCollectionProperty), model)
            .GetExplorerForProperty(nameof(ModelWithCollectionProperty.CollectionProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(ModelWithCollectionProperty.CollectionProperty);

        var checkboxesContext = new CheckboxesContext(name: "test", @for: new ModelExpression(modelExpression, modelExplorer));

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            ViewContext = viewContext,
            Value = itemValue
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal(expectedChecked, checkboxesItem.Checked);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithNullCollectionModelExpression_ExecutesSuccessfully()
    {
        // Arrange
        var model = new ModelWithCollectionProperty()
        {
            CollectionProperty = null
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(ModelWithCollectionProperty), model)
            .GetExplorerForProperty(nameof(ModelWithCollectionProperty.CollectionProperty));
        var viewContext = new ViewContext();
        var modelExpression = nameof(ModelWithCollectionProperty.CollectionProperty);

        var checkboxesContext = new CheckboxesContext(name: "test", @for: new ModelExpression(modelExpression, modelExplorer));

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            ViewContext = viewContext,
            Value = "2"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.False(checkboxesItem.Checked);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithHint_SetsHintOnContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<CheckboxesItemContext>();
                var hintOptions = new HintOptions { Html = new TemplateString("Hint") };
                itemContext.SetHint(hintOptions, "govuk-checkboxes-item-hint");

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Name = "name",
            Value = "value"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal("Hint", checkboxesItem.Hint?.Html?.ToHtmlString());
            });
    }

    [Fact]
    public async Task ProcessAsync_WithoutHint_DoesNotSetHintOnContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Name = "name",
            Value = "value"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Null(checkboxesItem.Hint);
            });
    }

    [Fact]
    public async Task ProcessAsync_WithConditional_SetsConditionalOnContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var itemContext = context.GetContextItem<CheckboxesItemContext>();
                var conditionalOptions = new CheckboxesOptionsItemConditional { Html = new TemplateString("Conditional") };
                itemContext.SetConditional(conditionalOptions, "govuk-checkboxes-item-conditional");

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Name = "name",
            Value = "value"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Equal("Conditional", checkboxesItem.Conditional?.Html?.ToHtmlString());
            });
    }

    [Fact]
    public async Task ProcessAsync_WithoutConditional_DoesNotSetConditionalOnContext()
    {
        // Arrange
        var checkboxesContext = new CheckboxesContext(name: "test", @for: null);

        var context = new TagHelperContext(
            tagName: "govuk-checkboxes-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(CheckboxesContext), checkboxesContext },
                { typeof(CheckboxesItemContext), new CheckboxesItemContext() }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesItemTagHelper()
        {
            Name = "name",
            Value = "value"
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            checkboxesContext.Items,
            item =>
            {
                var checkboxesItem = Assert.IsType<CheckboxesOptionsItem>(item);
                Assert.Null(checkboxesItem.Conditional);
            });
    }

    private class Model
    {
        public string? Foo { get; set; }
    }

    private class ModelWithBooleanProperty
    {
        public bool BooleanProperty { get; set; }
    }

    private class ModelWithCollectionProperty
    {
        public IEnumerable<int>? CollectionProperty { get; set; }
    }
}
