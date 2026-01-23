using GovUk.Frontend.AspNetCore.ComponentGeneration;
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

                errorSummaryContext.SetTitle([], "Title");
                errorSummaryContext.SetDescription([], "Description");

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        firstErrorHref,
                        firstErrorHtml,
                        [],
                        []));

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        secondErrorHref,
                        secondErrorHtml,
                        [],
                        []));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetContainerErrorContext();

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        ErrorSummaryOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateErrorSummaryAsync(It.IsAny<ErrorSummaryOptions>())).Callback<ErrorSummaryOptions>(o => actualOptions = o);

        var tagHelper = new ErrorSummaryTagHelper(componentGeneratorMock.Object)
        {
            DisableAutoFocus = disableAutoFocus,
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(firstErrorHref, error.Href);
                Assert.Equal(firstErrorHtml, error.Html);
            },
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(secondErrorHref, error.Href);
                Assert.Equal(secondErrorHtml, error.Html);
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
                        "Content",
                        [],
                        []));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        ErrorSummaryOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateErrorSummaryAsync(It.IsAny<ErrorSummaryOptions>())).Callback<ErrorSummaryOptions>(o => actualOptions = o);

        var tagHelper = new ErrorSummaryTagHelper(componentGeneratorMock.Object)
        {
            ViewContext = TestUtils.CreateViewContext()
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("There is a problem", actualOptions?.TitleHtml);
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

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator)
        {
            ViewContext = TestUtils.CreateViewContext()
        };

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

                errorSummaryContext.SetTitle([], "Title");
                errorSummaryContext.SetDescription([], "Description");

                errorSummaryContext.AddItem(
                    new ErrorSummaryContextItem(
                        itemErrorHref,
                        itemErrorHtml,
                        [],
                        []));

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetContainerErrorContext();
        containerErrorContext.AddError(containerErrorContextErrorHtml, containerErrorContextErrorHref);

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        ErrorSummaryOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateErrorSummaryAsync(It.IsAny<ErrorSummaryOptions>())).Callback<ErrorSummaryOptions>(o => actualOptions = o);

        var tagHelper = new ErrorSummaryTagHelper(componentGeneratorMock.Object)
        {
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(itemErrorHref, error.Href);
                Assert.Equal(itemErrorHtml, error.Html);
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

                errorSummaryContext.SetTitle([], "Title");
                errorSummaryContext.SetDescription([], "Description");

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetContainerErrorContext();
        containerErrorContext.AddError(containerErrorContextErrorHtml, containerErrorContextErrorHref);

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        ErrorSummaryOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateErrorSummaryAsync(It.IsAny<ErrorSummaryOptions>())).Callback<ErrorSummaryOptions>(o => actualOptions = o);

        var tagHelper = new ErrorSummaryTagHelper(componentGeneratorMock.Object)
        {
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(actualOptions);
        Assert.NotNull(actualOptions.ErrorList);
        Assert.Collection(
            actualOptions.ErrorList,
            error =>
            {
                Assert.NotNull(error);
                Assert.Equal(containerErrorContextErrorHref, error.Href);
                Assert.Equal(containerErrorContextErrorHtml, error.Html);
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
        var containerErrorContext = viewContext.HttpContext.GetContainerErrorContext();

        var (componentGenerator, _) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator)
        {
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Null(output.TagName);
        Assert.False(containerErrorContext.ErrorSummaryHasBeenRendered);
    }
}
