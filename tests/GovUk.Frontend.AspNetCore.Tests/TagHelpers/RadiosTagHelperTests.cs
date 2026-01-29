using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.Tests.TagHelpers;

public class RadiosTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.SetHint(attributes: new AttributeCollection(), html: new TemplateString("The hint"), tagName: "govuk-radios-hint");

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = "first",
                    Value = "first"
                });

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = "second",
                    Value = "second"
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-hint"" id=""my-id-hint"">The hint</div>
    <div class=""govuk-radios"" data-module=""govuk-radios"">
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" disabled=""disabled"" aria-describedby=""my-id-hint"" />
            <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""second"" name=""testradios"" type=""radio"" value=""second"" checked=""checked"" aria-describedby=""my-id-hint"" />
            <label class=""govuk-radios__label govuk-label"" for=""second"">Second</label>
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
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.SetErrorMessage(visuallyHiddenText: null, attributes: new AttributeCollection(), html: new TemplateString("A error"), tagName: "govuk-radios-error-message");

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = "first",
                    Value = "first"
                });

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = "second",
                    Value = "second"
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios",
            ViewContext = TestUtils.CreateViewContext()
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group govuk-form-group--error"">
    <p class=""govuk-error-message"" id=""my-id-error""><span class=""govuk-visually-hidden"">Error:</span>A error</p>
    <div class=""govuk-radios"" data-module=""govuk-radios"">
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" disabled=""disabled"" aria-describedby=""my-id-error"" />
            <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""second"" name=""testradios"" type=""radio"" value=""second"" checked=""checked"" aria-describedby=""my-id-error"" />
            <label class=""govuk-radios__label govuk-label"" for=""second"">Second</label>
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
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Hint = new HintOptions()
                    {
                        Html = new TemplateString("First item hint")
                    },
                    Id = "first",
                    Value = "first"
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-radios"" data-module=""govuk-radios"">
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" aria-describedby=""first-item-hint"" />
            <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
            <div class=""govuk-radios__hint govuk-hint"" id=""first-item-hint"">First item hint</div>
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
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Html = new TemplateString("First"),
                    Conditional = new RadiosOptionsItemConditional()
                    {
                        Html = new TemplateString("Item 1 conditional")
                    },
                    Id = "first",
                    Value = "first"
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-radios"" data-module=""govuk-radios"">
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" data-aria-controls=""conditional-first"" />
            <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-radios__conditional--hidden govuk-radios__conditional"" id=""conditional-first"">Item 1 conditional</div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithCheckedItemConditional_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("First"),
                    Conditional = new RadiosOptionsItemConditional()
                    {
                        Html = new TemplateString("Item 1 conditional")
                    },
                    Id = "first",
                    Value = "first"
                });

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <div class=""govuk-radios"" data-module=""govuk-radios"">
        <div class=""govuk-radios__item"">
            <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" checked=""checked"" data-aria-controls=""conditional-first"" />
            <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
        </div>
        <div class=""govuk-radios__conditional"" id=""conditional-first"">Item 1 conditional</div>
    </div>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }

    [Fact]
    public async Task ProcessAsync_WithFieldset_GeneratesExpectedOutput()
    {
        // Arrange
        var context = new TagHelperContext(
            tagName: "govuk-radios",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");

        var output = new TagHelperOutput(
            "govuk-radios",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) =>
            {
                var radiosContext = context.GetContextItem<RadiosContext>();

                radiosContext.OpenFieldset();
                var radiosFieldsetContext = new RadiosFieldsetContext(describedBy: null, attributes: new AttributeCollection(), @for: null);
                radiosFieldsetContext.SetLegend(isPageHeading: false, attributes: new AttributeCollection(), html: new TemplateString("Legend"));

                radiosContext.SetHint(attributes: new AttributeCollection(), html: new TemplateString("The hint"), tagName: "govuk-radios-hint");

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = false,
                    Html = new TemplateString("First"),
                    Disabled = true,
                    Id = "first",
                    Value = "first"
                });

                radiosContext.AddItem(new RadiosOptionsItem()
                {
                    Checked = true,
                    Html = new TemplateString("Second"),
                    Disabled = false,
                    Id = "second",
                    Value = "second"
                });

                radiosContext.CloseFieldset(radiosFieldsetContext);

                var tagHelperContent = new DefaultTagHelperContent();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        var componentGeneratorMock = TestUtils.CreateComponentGeneratorMock();
        var tagHelper = new RadiosTagHelper(componentGeneratorMock.Object, new DefaultModelHelper())
        {
            IdPrefix = "my-id",
            Name = "testradios"
        };

        tagHelper.Init(context);

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var expectedHtml = @"
<div class=""govuk-form-group"">
    <fieldset aria-describedby=""describedby my-id-hint"" class=""govuk-fieldset"">
        <legend class=""govuk-fieldset__legend"">Legend</legend>
        <div class=""govuk-hint"" id=""my-id-hint"">The hint</div>
        <div class=""govuk-radios"" data-module=""govuk-radios"">
            <div class=""govuk-radios__item"">
                <input class=""govuk-radios__input"" id=""first"" name=""testradios"" type=""radio"" value=""first"" disabled=""disabled"" />
                <label class=""govuk-radios__label govuk-label"" for=""first"">First</label>
            </div>
            <div class=""govuk-radios__item"">
                <input class=""govuk-radios__input"" id=""second"" name=""testradios"" type=""radio"" value=""second"" checked=""checked"" />
                <label class=""govuk-radios__label govuk-label"" for=""second"">Second</label>
            </div>
        </div>
    </fieldset>
</div>";

        AssertEx.HtmlEqual(expectedHtml, output.ToHtmlString());
    }
}
