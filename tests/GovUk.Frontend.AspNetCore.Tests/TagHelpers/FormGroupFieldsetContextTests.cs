using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class FormGroupFieldsetContextTests
{
    [Fact]
    public void SetLegend_SetsLegendOnContext()
    {
        // Arrange
        var context = new TestContext([], aspFor: null);

        // Act
        context.SetLegend(isPageHeading: true, null, new HtmlString("Legend"));

        // Assert
        Assert.True(context.Legend?.IsPageHeading);
        Assert.Equal("Legend", context.Legend?.Content?.ToString());
    }

    [Fact]
    public void SetLegend_AlreadySet_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new TestContext([], aspFor: null);

        context.SetLegend(false, null, new HtmlString("Existing legend"));

        // Act
        var ex = Record.Exception(() => context.SetLegend(isPageHeading: true, null, new HtmlString("Legend")));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Only one <test-fieldset-legend> element is permitted within each <test-fieldset>.", ex.Message);
    }

    private class TestContext(AttributeDictionary attributes, ModelExpression? aspFor) : FormGroupFieldsetContext(fieldsetTagName: "test-fieldset", legendTagName: "test-fieldset-legend", attributes, aspFor)
    {
    }
}
