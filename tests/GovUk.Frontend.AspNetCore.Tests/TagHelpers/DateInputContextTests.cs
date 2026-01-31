using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class DateInputContextTests
{
    [Fact]
    public void SetBeforeInputs_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new DateInputContext(haveExplicitValue: false, @for: null);

        var fieldsetContext = new DateInputFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.SetBeforeInputs(new TemplateString("Before"), tagName: "govuk-date-input-before-inputs"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-date-input-before-inputs> must be inside <govuk-date-input-fieldset>.", ex.Message);
    }

    [Fact]
    public void SetAfterInputs_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new DateInputContext(haveExplicitValue: false, @for: null);

        var fieldsetContext = new DateInputFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.SetAfterInputs(new TemplateString("After"), tagName: "govuk-date-input-after-inputs"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-date-input-after-inputs> must be inside <govuk-date-input-fieldset>.", ex.Message);
    }
}
