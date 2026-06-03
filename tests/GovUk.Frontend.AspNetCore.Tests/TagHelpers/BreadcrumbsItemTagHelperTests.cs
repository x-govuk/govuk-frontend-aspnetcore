using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class BreadcrumbsItemTagHelperTests : TagHelperTestBase<BreadcrumbsItemTagHelper>
{
    [Fact]
    public async Task ProcessAsync_NoLink_AddsItemToContext()
    {
        // Arrange
        var content = "The item";

        var breadcrumbsContext = new BreadcrumbsContext();

        var context = CreateTagHelperContext(contexts: breadcrumbsContext);

        var output = CreateTagHelperOutput(
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new BreadcrumbsItemTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var lastItem = breadcrumbsContext.Items.Last();
        Assert.Null(lastItem.Href);
        Assert.Equal(content, lastItem.Html);
    }

    [Fact]
    public async Task ProcessAsync_WithLink_AddsItemToContext()
    {
        // Arrange
        var content = "The item";
        var href = "http://place.com";

        var breadcrumbsContext = new BreadcrumbsContext();

        var context = CreateTagHelperContext(contexts: breadcrumbsContext);

        var attributes = new TagHelperAttributeList();
        var output = new TagHelperOutput(
            "govuk-breadcrumbs-item",
            attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                attributes.Add("href", href);

                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(content);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new BreadcrumbsItemTagHelper();

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var lastItem = breadcrumbsContext.Items.Last();
        Assert.Equal(href, lastItem.Href);
        Assert.Equal(content, lastItem.Html);
    }
}
