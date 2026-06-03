using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class GeneratedErrorSummaryTagHelperTests : TagHelperTestBase<GeneratedErrorSummaryTagHelper>
{
    [Xunit.Theory]
    [InlineData(null, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task ProcessAsync_RendersWhenExpected(
        bool? prependErrorSummary,
        bool expectErrorSummary)
    {
        // Arrange
        var options = Options.Create(new GovUkFrontendOptions());

        var errorHtml = "Error message";
        var errorHref = "#Field";

        var context = CreateTagHelperContext(tagName: "form");

        var output = CreateTagHelperOutput(tagName: "form",
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var (componentGenerator, getActualOptions) = CreateComponentGenerator<ErrorSummaryOptions>(nameof(IComponentGenerator.GenerateErrorSummaryAsync));

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetPageErrorContext();
        containerErrorContext.AddError(errorHtml, errorHref);

        var tagHelper = new GeneratedErrorSummaryTagHelper(componentGenerator, options)
        {
            PrependErrorSummary = prependErrorSummary,
            ViewContext = viewContext
        };

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var html = output.RenderToElement();
        Assert.Equal(expectErrorSummary ? 1 : 0, html.ChildElementCount);

        if (expectErrorSummary)
        {
            var actualOptions = getActualOptions();
            Assert.NotNull(actualOptions.ErrorList);
            Assert.True(containerErrorContext.ErrorSummaryHasBeenRendered);

            Assert.Collection(
                actualOptions.ErrorList,
                error =>
                {
                    Assert.NotNull(error);
                    Assert.Equal(errorHref, error.Href);
                    Assert.Equal(errorHtml, error.Html);
                });
        }
        else
        {
            Assert.False(containerErrorContext.ErrorSummaryHasBeenRendered);
        }
    }
}
