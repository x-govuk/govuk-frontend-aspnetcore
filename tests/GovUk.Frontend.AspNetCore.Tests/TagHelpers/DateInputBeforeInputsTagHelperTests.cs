using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputBeforeInputsTagHelperTests : TagHelperTestBase<DateInputBeforeInputsTagHelper>
{
    [Fact]
    public async Task ProcessAsync_SetsBeforeInputsOnContext()
    {
        // Arrange
        var content = "BeforeInputs";
        var className = CreateDummyClassName();
        var attributes = CreateDummyDataAttributes();
        var dateInputContext = new DateInputContext(haveExplicitValue: false, @for: null);

        var context = CreateTagHelperContext(className: className, attributes: attributes, contexts: dateInputContext);

        var output = CreateTagHelperOutput(
            className: className,
            attributes: attributes,
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.AppendHtml(new HtmlString(content));
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new DateInputBeforeInputsTagHelper(new NullLogger<DateInputBeforeInputsTagHelper>());

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.NotNull(dateInputContext.BeforeInputs);
        Assert.Equal(content, dateInputContext.BeforeInputs);
    }
}
