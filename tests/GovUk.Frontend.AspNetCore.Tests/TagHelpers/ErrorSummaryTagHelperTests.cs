using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class ErrorSummaryTagHelperTests : TagHelperTestBase<ErrorSummaryTagHelper>
{
    [Fact]
    public async Task ProcessAsync_InvokesComponentGeneratorWithExpectedOptions()
    {
        // Arrange
        var firstErrorHtml = "First error";
        var firstErrorHref = "#FirstError";
        var secondErrorHtml = "Second error";
        var secondErrorHref = "#SecondError";
        var disableAutoFocus = true;

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)];

                errorSummaryContext.SetTitle([], new TemplateString("Title"), ErrorSummaryTitleTagHelper.TagName);
                errorSummaryContext.SetDescription([], new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName);

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        firstErrorHref,
                        new TemplateString(firstErrorHtml),
                        [],
                        []),
                    ErrorSummaryItemTagHelper.TagName);

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        secondErrorHref,
                        new TemplateString(secondErrorHtml),
                        [],
                        []),
                    ErrorSummaryItemTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            DisableAutoFocus = disableAutoFocus,
            ViewContext = viewContext
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(firstErrorHref, error.Href);
                Assert.Equal(firstErrorHtml, error.Html?.ToHtmlString());
            },
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(secondErrorHref, error.Href);
                Assert.Equal(secondErrorHtml, error.Html?.ToHtmlString());
            });
        Assert.Equal(disableAutoFocus, actualOptions.DisableAutoFocus);
        Assert.True(containerErrorContext.ErrorSummaryHasBeenRendered);
    }

    [Fact]
    public async Task ProcessAsync_NoTitleSpecified_UsesDefaultTitle()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)];

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        "#Href",
                        new TemplateString("Content"),
                        [],
                        []),
                    ErrorSummaryItemTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("There is a problem", getActualOptions().TitleText);
    }

    [Fact]
    public async Task ProcessAsync_NoTitleDescriptionOrItems_RendersNothing()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, _) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var html = output.ToHtmlString();
        Assert.Empty(html);
    }

    [Fact]
    public async Task ProcessAsync_HasExplicitItemsDoesNotGetErrorsFromContainerErrorContext()
    {
        // Arrange
        var containerErrorContextErrorHtml = "First error";
        var containerErrorContextErrorHref = "#FirstError";
        var itemErrorHtml = "Item error";
        var itemErrorHref = "#ItemError";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)];

                errorSummaryContext.SetTitle([], new TemplateString("Title"), ErrorSummaryTitleTagHelper.TagName);
                errorSummaryContext.SetDescription([], new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName);

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        itemErrorHref,
                        new TemplateString(itemErrorHtml),
                        [],
                        []),
                    ErrorSummaryItemTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();
        containerErrorContext.AddError(new TemplateString(containerErrorContextErrorHtml), containerErrorContextErrorHref);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = viewContext
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(itemErrorHref, error.Href);
                Assert.Equal(itemErrorHtml, error.Html?.ToHtmlString());
            });
        Assert.True(containerErrorContext.ErrorSummaryHasBeenRendered);
    }

    [Fact]
    public async Task ProcessAsync_DoesNotHaveExplicitItemsGetsErrorsFromContainerErrorContext()
    {
        // Arrange
        var containerErrorContextErrorHtml = "First error";
        var containerErrorContextErrorHref = "#FirstError";

        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)];

                errorSummaryContext.SetTitle([], new TemplateString("Title"), ErrorSummaryTitleTagHelper.TagName);
                errorSummaryContext.SetDescription([], new TemplateString("Description"), ErrorSummaryDescriptionTagHelper.TagName);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();
        containerErrorContext.AddError(new TemplateString(containerErrorContextErrorHtml), containerErrorContextErrorHref);

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = viewContext
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(containerErrorContextErrorHref, error.Href);
                Assert.Equal(containerErrorContextErrorHtml, error.Html?.ToHtmlString());
            });
        Assert.True(containerErrorContext.ErrorSummaryHasBeenRendered);
    }

    [Fact]
    public async Task ProcessAsync_DoesNotHaveTitleOrDescriptionOrItemsRendersNothing()
    {
        // Arrange
        var context = CreateTagHelperContext();

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();

        var (componentGenerator, _) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = viewContext
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(output.TagName);
        Assert.False(containerErrorContext.ErrorSummaryHasBeenRendered);
    }
}
