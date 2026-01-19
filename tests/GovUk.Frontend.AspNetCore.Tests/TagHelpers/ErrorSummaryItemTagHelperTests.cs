using System.Text.Encodings.Web;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ErrorSummaryItemTagHelperTests : TagHelperTestBase<ErrorSummaryItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_AddsItemToContext()
    {
        // Arrange
        var errorMessage = "Error message";

        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(errorMessage);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var options = CreateOptions();

        var tagHelper = new ErrorSummaryItemTagHelper(options);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal(new TemplateString(HtmlEncoder.Default.Encode(errorMessage)), item.Html);
            });
        Assert.True(errorSummaryContext.HaveExplicitItems);
    }

    [Fact]
    public async Task ProcessAsync_NoContentOrFor_ThrowsInvalidOperationException()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var options = CreateOptions();

        var tagHelper = new ErrorSummaryItemTagHelper(options);

        // Act
        var ex = await Record.ExceptionAsync(() => tagHelper.ProcessAsync(context, output));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Content is required when the 'for' attribute is not specified.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_ForSpecified_UsesModelStateErrorMessageForContent()
    {
        // Arrange
        var errorMessage = "ModelState error message";

        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Field));

        var viewContext = new ViewContext();

        var options = CreateOptions();

        viewContext.ModelState.AddModelError(nameof(Model.Field), errorMessage);

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Field), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal(new TemplateString(HtmlEncoder.Default.Encode(errorMessage)), item.Html);
            });
    }

    [Fact]
    public async Task ProcessAsync_ForSpecifiedAndModelStateHasNoError_SetsHaveExplicitItemsOnContext()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Field));

        var viewContext = new ViewContext();

        var options = CreateOptions();

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Field), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.True(errorSummaryContext.HaveExplicitItems);
    }

    [Fact]
    public async Task ProcessAsync_BothContentAndAspNetForSpecified_UsesContent()
    {
        // Arrange
        var modelStateErrorMessage = "ModelState error message";
        var explicitErrorMessage = "Error message";

        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(explicitErrorMessage);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Field));

        var viewContext = new ViewContext();

        var options = CreateOptions();

        viewContext.ModelState.AddModelError(nameof(Model.Field), modelStateErrorMessage);

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Field), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal(new TemplateString(explicitErrorMessage), item.Html);
            });
    }

    [Fact]
    public async Task ProcessAsync_HrefAttributeSpecifiedOrDerived_SetsHrefOnItem()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: new TagHelperAttributeList()
            {
                { "href", "#TheField" }
            },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Error message");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var options = CreateOptions();

        var tagHelper = new ErrorSummaryItemTagHelper(options);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal("#TheField", item.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_AspForSpecified_GeneratesHrefFromModelExpression()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Field));

        var viewContext = new ViewContext();

        var options = CreateOptions();

        viewContext.ModelState.AddModelError(nameof(Model.Field), "ModelState error message");

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Field), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal("#Field", item.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_AspForSpecifiedForDateField_GeneratesHrefForDateComponentFromModelExpression()
    {
        // Arrange
        var viewContext = CreateViewContext();

        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var modelMetadataProvider = new TestModelMetadataProvider();

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Date));

        var options = CreateOptions();

        var modelName = nameof(Model.Date);
        AddDateInputParseException(viewContext, modelName, DateInputParseErrors.InvalidMonth | DateInputParseErrors.MissingYear);

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Date), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal("#Date_Month", item.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_BothHrefAndAspForSpecified_UsesHrefAttribute()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: new TagHelperAttributeList()
            {
                { "href", "#SomeHref" }
            },
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            })
        {
            TagMode = TagMode.SelfClosing
        };

        var modelExplorer = new EmptyModelMetadataProvider().GetModelExplorerForType(typeof(Model), new Model())
            .GetExplorerForProperty(nameof(Model.Field));

        var viewContext = new ViewContext();

        var options = CreateOptions();

        viewContext.ModelState.AddModelError(nameof(Model.Field), "ModelState error message");

        var tagHelper = new ErrorSummaryItemTagHelper(options)
        {
            For = new ModelExpression(nameof(Model.Field), modelExplorer),
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Equal("#SomeHref", item.Href);
            });
    }

    [Fact]
    public async Task ProcessAsync_NoHrefAttributeOrAspFor_SetsNullHrefOnItem()
    {
        // Arrange
        var errorSummaryContext = new ErrorSummaryContext();

        var context = new TagHelperContext(
            tagName: "govuk-error-summary-item",
            allAttributes: [],
            items: new Dictionary<object, object>()
            {
                { typeof(ErrorSummaryContext), errorSummaryContext }
            },
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-error-summary-item",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent("Error message");
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var options = CreateOptions();

        var tagHelper = new ErrorSummaryItemTagHelper(options);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Collection(
            errorSummaryContext.Items,
            item =>
            {
                Assert.Null(item.Href);
            });
    }

    private class Model
    {
        public DateOnly? Date { get; set; }
        public string? Field { get; set; }
    }
}
