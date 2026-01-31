using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesAfterInputsTagHelperTests : TagHelperTestBase<CheckboxesAfterInputsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsAfterInputsOnContext()
    {
        // Arrange
        var content = "AfterInputs";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var checkboxesContext = new CheckboxesContext(name: null, @for: null);

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: checkboxesContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesAfterInputsTagHelper(new NullLogger<CheckboxesAfterInputsTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(checkboxesContext.AfterInputs);
        Assert.Equal(content, checkboxesContext.AfterInputs);
    }
}
