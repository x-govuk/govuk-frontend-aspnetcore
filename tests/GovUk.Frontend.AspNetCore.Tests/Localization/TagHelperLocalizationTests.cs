using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;
using GovUk.Frontend.AspNetCore.TagHelpers;
using GovUk.Frontend.AspNetCore.Tests.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.Tests.Localization;

public class TagHelperLocalizationTests : TagHelperTestBase<ErrorSummaryTagHelper>
{
    [Xunit.Fact]
    public async Task TitleTagHelper_UsesTheLocalizedErrorPrefix()
    {
        // Arrange
        var options = Options.Create(new GovUkFrontendOptions { PrependErrorToTitle = true });
        var localizer = DelegateLocalizer.ForName(GovUkFrontendResourceNames.TitleErrorPrefix, "Gwall:");

        var (context, output, viewContext) = CreateTitleTagHelperState();

        var tagHelper = new TitleTagHelper(options, localizer) { ViewContext = viewContext };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.StartsWith("Gwall: ", output.RenderToElement().InnerHtml, StringComparison.Ordinal);
    }

    [Xunit.Fact]
    public async Task TitleTagHelper_ErrorPrefixAttribute_TakesPrecedenceOverTheLocalizedContent()
    {
        // Arrange
        var options = Options.Create(new GovUkFrontendOptions { PrependErrorToTitle = true });
        var localizer = DelegateLocalizer.ForName(GovUkFrontendResourceNames.TitleErrorPrefix, "Gwall:");

        var (context, output, viewContext) = CreateTitleTagHelperState();

        var tagHelper = new TitleTagHelper(options, localizer)
        {
            ErrorPrefix = "Problem:",
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.StartsWith("Problem: ", output.RenderToElement().InnerHtml, StringComparison.Ordinal);
    }

    [Xunit.Fact]
    public async Task ErrorSummaryTagHelper_UsesTheLocalizedTitleInTheTextSlot()
    {
        // Arrange
        var context = CreateTagHelperContext(tagName: "govuk-error-summary");

        var output = CreateTagHelperOutput(
            tagName: "govuk-error-summary",
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)]!;
                errorSummaryContext.AddItem(new ErrorSummaryContextItem(Href: null, Html: new TemplateString("An error"), Attributes: new(), ItemAttributes: new()), ErrorSummaryItemTagHelper.TagName);
                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var localizer = DelegateLocalizer.ForName(GovUkFrontendResourceNames.ErrorSummaryTitleText, "Mae problem wedi codi");

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, localizer)
        {
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();

        // The localized title goes in the text slot so that it's HTML-encoded.
        Assert.Equal("Mae problem wedi codi", actualOptions.TitleText);
        Assert.Null(actualOptions.TitleHtml);
    }

    [Xunit.Fact]
    public async Task ErrorSummaryTagHelper_WithNoLocalizedContent_UsesTheBuiltInTitle()
    {
        // Arrange
        var context = CreateTagHelperContext(tagName: "govuk-error-summary");

        var output = CreateTagHelperOutput(
            tagName: "govuk-error-summary",
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)]!;
                errorSummaryContext.AddItem(new ErrorSummaryContextItem(Href: null, Html: new TemplateString("An error"), Attributes: new(), ItemAttributes: new()), ErrorSummaryItemTagHelper.TagName);
                return Task.FromResult<TagHelperContent>(new DefaultTagHelperContent());
            });

        var (componentGenerator, getActualOptions) =
            CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var tagHelper = new ErrorSummaryTagHelper(componentGenerator, NullGovUkFrontendLocalizer.Instance)
        {
            ViewContext = TestUtils.CreateViewContext()
        };
        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var actualOptions = getActualOptions();

        Assert.Equal("There is a problem", actualOptions.TitleText);
        Assert.Null(actualOptions.TitleHtml);
    }

    private (TagHelperContext Context, TagHelperOutput Output, Microsoft.AspNetCore.Mvc.Rendering.ViewContext ViewContext)
        CreateTitleTagHelperState()
    {
        var context = CreateTagHelperContext(tagName: "title");

        var output = CreateTagHelperOutput(
            tagName: "title",
            getChildContentAsync: (useCachedResult, encoder) =>
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        var viewContext = TestUtils.CreateViewContext();
        viewContext.HttpContext.GetPageErrorContext().ErrorSummaryHasBeenRendered = true;

        return (context, output, viewContext);
    }
}
