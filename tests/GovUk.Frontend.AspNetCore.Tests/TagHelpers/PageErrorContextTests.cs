using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class PageErrorContextTests
{
    [Fact]
    public void AddError_AddsErrorToContext()
    {
        // Arrange
        var context = new PageErrorContext();
        var html = "Content";
        var href = "/foo";

        // Act
        context.AddError(new TemplateString(html), href);

        // Assert
        Assert.Collection(
            context.Errors,
            item =>
            {
                Assert.Equal(html, item.Html.ToHtmlString());
                Assert.Equal(href, item.Href);
            });
    }
}
