using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class CheckboxesTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.SetHint(attributes: null, content: new HtmlString("The hint"));

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testcheckboxes"
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-hint"" id=""my-id-hint"">The hint</div>
    <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" aria-describedby=""my-id-hint"" disabled=""disabled"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""second"" name=""testcheckboxes"" type=""checkbox"" value=""second"" aria-describedby=""my-id-hint"" checked=""checked"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""second"">Second</label>
        </div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithError_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.SetErrorMessage(visuallyHiddenText: null, attributes: null, content: new HtmlString("A error"));

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testcheckboxes",
            ViewContext = TestUtils.CreateViewContext()
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group govuk-form-group--error"">
    <p class=""govuk-error-message"" id=""my-id-error""><span class=""govuk-visually-hidden"">Error:</span>A error</p>
    <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" aria-describedby=""my-id-error"" disabled=""disabled"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""second"" name=""testcheckboxes"" type=""checkbox"" value=""second"" aria-describedby=""my-id-error"" checked=""checked"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""second"">Second</label>
        </div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithItemHint_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Hint = new HintOptions()
                    {
                        Html = new TemplateString("First item hint")
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testcheckboxes"
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" aria-describedby=""first-item-hint"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
            <div class=""govuk-checkboxes__hint govuk-hint"" id=""first-item-hint"">First item hint</div>
        </div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithUncheckedItemConditional_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Conditional = new CheckboxesOptionsItemConditional()
                    {
                        Html = new TemplateString("Item 1 conditional")
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testcheckboxes"
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" data-aria-controls=""conditional-first"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-checkboxes__conditional--hidden govuk-checkboxes__conditional"" id=""conditional-first"">Item 1 conditional</div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithCheckedItemConditional_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("First"),
                    Conditional = new CheckboxesOptionsItemConditional()
                    {
                        Html = new TemplateString("Item 1 conditional")
                    },
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testcheckboxes"
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
        <div class=""govuk-checkboxes__item"">
            <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" checked=""checked"" data-aria-controls=""conditional-first"" />
            <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-checkboxes__conditional"" id=""conditional-first"">Item 1 conditional</div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithFieldset_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-checkboxes",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-checkboxes",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var checkboxesContext = context.GetContextItem<CheckboxesContext>();

                checkboxesContext.OpenFieldset();
                var checkboxesFieldsetContext = new CheckboxesFieldsetContext(attributes: null, aspFor: null);
                checkboxesFieldsetContext.SetLegend(isPageHeading: false, attributes: null, content: new HtmlString("Legend"));

                checkboxesContext.SetHint(attributes: null, content: new HtmlString("The hint"));

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = new TemplateString("first"),
                    Value = new TemplateString("first")
                });

                checkboxesContext.AddItem(new CheckboxesOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = new TemplateString("second"),
                    Value = new TemplateString("second")
                });

                checkboxesContext.CloseFieldset(checkboxesFieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var tagHelper = new CheckboxesTagHelper(new DefaultComponentGenerator(), new DefaultModelHelper())
        {
            DescribedBy = "describedby",
            IdPrefix = "my-id",
            Name = "testcheckboxes"
        };

        // Act
        tagHelper.Init(context);
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <fieldset aria-describedby=""describedby my-id-hint"" class=""govuk-fieldset"">
        <legend class=""govuk-fieldset__legend"">Legend</legend>
        <div class=""govuk-hint"" id=""my-id-hint"">The hint</div>
        <div class=""govuk-checkboxes"" data-module=""govuk-checkboxes"">
            <div class=""govuk-checkboxes__item"">
                <input class=""govuk-checkboxes__input"" id=""first"" name=""testcheckboxes"" type=""checkbox"" value=""first"" disabled=""disabled"" />
                <label class=""govuk-checkboxes__label govuk-label"" for=""first"">First</label>
            </div>
            <div class=""govuk-checkboxes__item"">
                <input class=""govuk-checkboxes__input"" id=""second"" name=""testcheckboxes"" type=""checkbox"" value=""second"" checked=""checked"" />
                <label class=""govuk-checkboxes__label govuk-label"" for=""second"">Second</label>
            </div>
        </div>
    </fieldset>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }
}
