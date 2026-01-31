using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesContextTests
{
    [Fact]
    public void AddItem_AddsItemToItems()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        // Act
        context.AddItem(item);

        // Assert
        var contextItem = Assert.Single(context.Items);
        Assert.Same(item, contextItem);
    }

    [Fact]
    public void AddItem_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.AddItem(item));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-item> must be inside <govuk-checkboxes-fieldset>.", ex.Message);
    }

    [Fact]
    public void OpenFieldset_AlreadyOpen_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection());

        // Act
        var ex = Record.Exception(() => context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-fieldset> cannot be nested inside another <govuk-checkboxes-fieldset>.", ex.Message);
    }

    [Fact]
    public void OpenFieldset_AlreadyGotFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal<object>("Only one <govuk-checkboxes-fieldset> element is permitted within each <govuk-checkboxes>.", ex.Message);
    }

    [Fact]
    public void OpenFieldset_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        context.AddItem(item);

        // Act
        var ex = Record.Exception(() => context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-fieldset> must be the only direct child of the <govuk-checkboxes>.", ex.Message);
    }

    [Fact]
    public void OpenFieldset_AlreadyGotHint_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        context.SetHint(attributes: new AttributeCollection(), html: new TemplateString("Hint"), tagName: "govuk-checkboxes-hint");

        // Act
        var ex = Record.Exception(() => context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-fieldset> must be the only direct child of the <govuk-checkboxes>.", ex.Message);
    }

    [Fact]
    public void OpenFieldset_AlreadyGotErrorMessage_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);
        context.SetErrorMessage(visuallyHiddenText: null, attributes: new AttributeCollection(), html: new TemplateString("Error"), tagName: "govuk-checkboxes-error-message");

        // Act
        var ex = Record.Exception(() => context.OpenFieldset(new CheckboxesFieldsetContext(describedBy: null, @for: null), new AttributeCollection()));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-fieldset> must be the only direct child of the <govuk-checkboxes>.", ex.Message);
    }

    [Fact]
    public void CloseFieldset_FieldsetNotOpened_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        // Act
        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        var ex = Record.Exception(() => context.CloseFieldset());

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("Fieldset has not been opened.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        context.AddItem(item);

        // Act
        var ex = Record.Exception(
            () => context.SetErrorMessage(visuallyHiddenText: null, attributes: new AttributeCollection(), html: new TemplateString("Error"), tagName: "govuk-checkboxes-error-message"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-error-message> must be specified before <govuk-checkboxes-item>.", ex.Message);
    }

    [Fact]
    public void SetErrorMessage_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(
            () => context.SetErrorMessage(visuallyHiddenText: null, attributes: new AttributeCollection(), html: new TemplateString("Error"), tagName: "govuk-checkboxes-error-message"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-error-message> must be inside <govuk-checkboxes-fieldset>.", ex.Message);
    }

    [Fact]
    public void SetHint_AlreadyGotItem_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        context.AddItem(item);

        // Act
        var ex = Record.Exception(() => context.SetHint(attributes: new AttributeCollection(), html: new TemplateString("Hint"), tagName: "govuk-checkboxes-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-hint> must be specified before <govuk-checkboxes-item>.", ex.Message);
    }

    [Fact]
    public void SetHint_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var item = new CheckboxesOptionsItem()
        {
            Html = new TemplateString("Item 1"),
            Value = new TemplateString("item1")
        };

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.SetHint(attributes: new AttributeCollection(), html: new TemplateString("Hint"), tagName: "govuk-checkboxes-hint"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-hint> must be inside <govuk-checkboxes-fieldset>.", ex.Message);
    }

    [Fact]
    public void SetLabel_ThrowsNotSupportedException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        // Act
        var ex = Record.Exception(() => context.SetLabel(isPageHeading: false, attributes: new AttributeCollection(), html: null, tagName: "govuk-checkboxes-label"));

        // Assert
        Assert.IsType<NotSupportedException>(ex);
    }

    [Fact]
    public void SetBeforeInputs_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.SetBeforeInputs(new TemplateString("Before"), tagName: "govuk-checkboxes-before-inputs"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-before-inputs> must be inside <govuk-checkboxes-fieldset>.", ex.Message);
    }

    [Fact]
    public void SetAfterInputs_OutsideOfFieldset_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = new CheckboxesContext(name: null, @for: null);

        var fieldsetContext = new CheckboxesFieldsetContext(describedBy: null, @for: null);
        context.OpenFieldset(fieldsetContext, new AttributeCollection());
        context.CloseFieldset();

        // Act
        var ex = Record.Exception(() => context.SetAfterInputs(new TemplateString("After"), tagName: "govuk-checkboxes-after-inputs"));

        // Assert
        Assert.IsType<InvalidOperationException>(ex);
        Assert.Equal("<govuk-checkboxes-after-inputs> must be inside <govuk-checkboxes-fieldset>.", ex.Message);
    }
}
