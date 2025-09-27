using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class GeneratedErrorSummaryTagHelperTests
{
    [Theory]
    [InlineData(false, null, false)]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, null, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public async Task ProcessAsync_RendersWhenExpected(
        bool prepentErrorSummaryToFormsOption,
        bool? prependErrorSummary,
        bool expectErrorSummary)
    {
        // Arrange
        var options = Options.Create(new GovUkFrontendOptions()
        {
#pragma warning disable GFA0005
            PrependErrorSummaryToForms = prepentErrorSummaryToFormsOption
#pragma warning restore GFA0005
        });

        var errorHtml = "Error message";
        var errorHref = "#Field";

        var context = new TagHelperContext(
            tagName: "form",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "form",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        ErrorSummaryOptions? actualOptions = null;
        componentGeneratorMock.Setup(mock => mock.GenerateErrorSummaryAsync(It.IsAny<ErrorSummaryOptions>())).Callback<ErrorSummaryOptions>(o => actualOptions = o);

        var viewContext = TestUtils.CreateViewContext();
        var containerErrorContext = viewContext.HttpContext.GetContainerErrorContext();
        containerErrorContext.AddError(errorHtml, errorHref);

        var tagHelper = new GeneratedErrorSummaryTagHelper(componentGeneratorMock.Object, options)
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
            Assert.NotNull(actualOptions);
            Assert.NotNull(actualOptions.ErrorList);
            Assert.True(containerErrorContext.ErrorSummaryHasBeenRendered);

            Assert.Collection(
                actualOptions.ErrorList,
                error =>
                {
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
