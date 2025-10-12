using System.Linq.Expressions;
using System.Text.Encodings.Web;
using AngleSharp.Dom;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.ModelBinding;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Xunit.Sdk;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public abstract class TagHelperTestBase(string tagName, string? parentTagName = null)
{
    protected string TagName { get; } = tagName;

    protected string? ParentTagName { get; } = parentTagName;

    protected TagHelperContext CreateTagHelperContext(
        string? tagName = null,
        string? className = null,
        IDictionary<string, string?>? attributes = null,
        params object[] contexts)
    {
        var tagHelperAttributes = new TagHelperAttributeList();

        if (attributes is not null)
        {
            foreach (var attr in attributes)
            {
                tagHelperAttributes.Add(
                    new TagHelperAttribute(
                        attr.Key,
                        attr.Value,
                        attr.Value is not null ? HtmlAttributeValueStyle.DoubleQuotes : HtmlAttributeValueStyle.Minimized));
            }
        }

        if (className is not null)
        {
            tagHelperAttributes.Add("class", className);
        }

        var items = contexts.ToDictionary(object (c) => c.GetType(), c => c);

        return new TagHelperContext(
            tagName ?? TagName,
            tagHelperAttributes,
            items,
            uniqueId: "test");
    }

    protected TagHelperOutput CreateTagHelperOutput(
        string? tagName = null,
        string? className = null,
        IDictionary<string, string?>? attributes = null,
        Func<bool, HtmlEncoder, Task<TagHelperContent>>? getChildContentAsync = null)
    {
        var tagHelperAttributes = new TagHelperAttributeList();

        if (attributes is not null)
        {
            foreach (var attr in attributes)
            {
                tagHelperAttributes.Add(
                    new TagHelperAttribute(
                        attr.Key,
                        attr.Value,
                        attr.Value is not null ? HtmlAttributeValueStyle.DoubleQuotes : HtmlAttributeValueStyle.Minimized));
            }
        }

        if (className is not null)
        {
            tagHelperAttributes.Add("class", className);
        }

        return new TagHelperOutput(
            tagName ?? TagName,
            tagHelperAttributes,
            getChildContentAsync: getChildContentAsync ?? ((usedCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            }));
    }

    protected ViewContext CreateViewContext() =>
        new() { HttpContext = new DefaultHttpContext() };

    protected IOptions<GovUkFrontendOptions> CreateOptions(Action<GovUkFrontendOptions>? configure = null)
    {
        var options = new GovUkFrontendOptions();
        configure?.Invoke(options);
        return Options.Create(options);
    }

    protected IDictionary<string, string?> CreateDummyDataAttributes() =>
        new Dictionary<string, string?>()
        {
            { "data-foo", Random.Shared.Next().ToString() }
        };

    protected string CreateDummyClassName() => $"class-{Random.Shared.Next()}";

    protected void AssertContainsAttributes(
        IDictionary<string, string?> expectedAttributes,
        AttributeCollection? actualAttributes,
        params string[] except) =>
        AssertContainsAttributes(new AttributeCollection(expectedAttributes), actualAttributes, except);

    protected void AssertContainsAttributes(
        AttributeCollection expectedAttributes,
        AttributeCollection? actualAttributes,
        params string[] except)
    {
        Assert.NotNull(actualAttributes);

        foreach (var attr in expectedAttributes)
        {
            if (except.Contains(attr.Key))
            {
                continue;
            }

            Assert.Contains(actualAttributes, a => a.Key == attr.Key && a.Value == attr.Value);
        }
    }

    protected void AssertContainsAttributes(IDictionary<string, string?> expectedAttributes, AttributeCollection? actualAttributes) =>
        AssertContainsAttributes(new AttributeCollection(expectedAttributes), actualAttributes);

    protected void AssertContainsAttributes(AttributeCollection expectedAttributes, AttributeCollection? actualAttributes)
    {
        Assert.NotNull(actualAttributes);

        foreach (var attr in expectedAttributes)
        {
            Assert.Contains(actualAttributes, a => a.Key == attr.Key && a.Value == attr.Value);
        }
    }

    protected (IComponentGenerator ComponentGenerator, Func<TOptions> GetActualOptions) CreateComponentGenerator<TOptions>(
        string generateMethodName)
        where TOptions : class
    {
        var componentGenerator = TestUtils.CreateComponentGeneratorMock();

        Expression<Func<DefaultComponentGenerator, Task<IHtmlContent>>> CreateExpression()
        {
            var generatorParameter = Expression.Parameter(typeof(DefaultComponentGenerator));

            var method = typeof(DefaultComponentGenerator).GetMethod(generateMethodName)!;

            var optionsArg = Expression.Call(
                instance: null,
                typeof(It).GetMethod("IsAny")!.MakeGenericMethod(typeof(TOptions)));

            return (Expression<Func<DefaultComponentGenerator, Task<IHtmlContent>>>)Expression.Lambda(
                Expression.Call(
                    generatorParameter,
                    method,
                    optionsArg),
                generatorParameter);
        }

        TOptions? actualOptions = null;
        componentGenerator.Setup(CreateExpression()).Callback<TOptions>(o => actualOptions = o);

        return (componentGenerator.Object, () => actualOptions ?? throw new XunitException("ComponentGenerator method was not invoked."));
    }

    protected void AddDateInputParseException(ViewContext viewContext, string displayName, DateInputParseErrors parseErrors)
    {
        ArgumentNullException.ThrowIfNull(viewContext);
        ArgumentNullException.ThrowIfNull(displayName);

        var messageTemplate = DateInputModelBinder.GetModelStateErrorMessageTemplate(parseErrors);
        var exception = new DateInputParseException(messageTemplate, displayName, parseErrors);

        AddDateInputParseException(viewContext, displayName, exception);
    }

    protected void AddDateInputParseException(ViewContext viewContext, string displayName, DateInputParseException exception)
    {
        ArgumentNullException.ThrowIfNull(viewContext);
        ArgumentNullException.ThrowIfNull(exception);

        var modelState = viewContext.ModelState;

        // Ensure ModelStateDictionary has an entry for property
        modelState.SetModelValue(displayName, rawValue: null, attemptedValue: null);

        var modelError = new ModelError(exception, exception.Message);
        modelState[displayName]!.Errors.Add(modelError);
    }
}
