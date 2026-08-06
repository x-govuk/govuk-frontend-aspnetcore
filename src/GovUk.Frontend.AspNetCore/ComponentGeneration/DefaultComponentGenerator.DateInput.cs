using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateDateInputAsync(DateInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var hasFieldset = options.Fieldset is not null;
        var values = options.Values;

        // The day/month/year fields default to standard items whose value is taken from the
        // `values` object, and are used only when an explicit `items` collection isn't supplied.
        var dateInputDay = options.Day ??
            new DateInputOptionsItem { Name = "day", Value = GetValue(values, "day"), Classes = "govuk-input--width-2" };
        var dateInputMonth = options.Month ??
            new DateInputOptionsItem { Name = "month", Value = GetValue(values, "month"), Classes = "govuk-input--width-2" };
        var dateInputYear = options.Year ??
            new DateInputOptionsItem { Name = "year", Value = GetValue(values, "year"), Classes = "govuk-input--width-4" };

        var dateInputItems = options.Items is { Count: > 0 } ?
            options.Items :
            [dateInputDay, dateInputMonth, dateInputYear];

        var describedByParts = new List<TemplateString>();
        if (options.Fieldset?.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        if (hasFieldset)
        {
            var innerContent = await BuildInnerContentAsync();
            var fieldsetComponent = await GenerateFieldsetAsync(new FieldsetOptions
            {
                DescribedBy = describedByParts.Count > 0 ? TemplateString.Join(" ", describedByParts) : null,
                Classes = options.Fieldset!.Classes,
                Role = "group",
                Attributes = options.Fieldset.Attributes,
                Legend = options.Fieldset.Legend,
                Html = innerContent.Snapshot()
            });

            formGroupDiv.InnerHtml.AppendHtml(fieldsetComponent);
        }
        else
        {
            var innerContent = await BuildInnerContentAsync();
            formGroupDiv.InnerHtml.AppendHtml(innerContent);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        async Task<IHtmlContent> BuildInnerContentAsync()
        {
            var innerHtmlBuilder = new HtmlContentBuilder();

            if (options.Hint is not null)
            {
                var hintId = new TemplateString($"{options.Id}-hint");
                describedByParts.Add(hintId);
                var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

                innerHtmlBuilder.AppendHtml(hintComponent);
            }

            if (options.ErrorMessage is not null)
            {
                var errorId = new TemplateString($"{options.Id}-error");
                describedByParts.Add(errorId);
                var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

                innerHtmlBuilder.AppendHtml(errorMessageComponent);
            }

            var dateInputDiv = new HtmlTag("div", attrs => attrs
                .With("id", options.Id)
                .WithClasses("govuk-date-input", options.Classes)
                .With(options.Attributes));

            if (options.FormGroup?.BeforeInputs is { } beforeInputs)
            {
                var beforeContent = HtmlOrText(beforeInputs.Html, beforeInputs.Text);
                if (!beforeContent.IsEmpty())
                {
                    dateInputDiv.InnerHtml.AppendHtml(beforeContent);
                }
            }

            var anyItemHasError = dateInputItems.Any(i => i.Error is true || ClassesContain(i.Classes, "govuk-input--error"));

            foreach (var item in dateInputItems)
            {
                var itemDiv = await CreateDateInputItemAsync(item, options.Id, options.NamePrefix, anyItemHasError);
                dateInputDiv.InnerHtml.AppendHtml(itemDiv);
            }

            if (options.FormGroup?.AfterInputs is { } afterInputs)
            {
                var afterContent = HtmlOrText(afterInputs.Html, afterInputs.Text);
                if (!afterContent.IsEmpty())
                {
                    dateInputDiv.InnerHtml.AppendHtml(afterContent);
                }
            }

            innerHtmlBuilder.AppendHtml(dateInputDiv);
            return innerHtmlBuilder;
        }

        async Task<HtmlTag> CreateDateInputItemAsync(DateInputOptionsItem item, TemplateString? parentId, TemplateString? namePrefix, bool anyItemHasError)
        {
            var itemDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-date-input__item"));

            // Resolve the item's name, value and width. The day/month/year fields are matched by
            // object identity (for the default items) or by their name, and the year field is wider.
            var itemName = item.Name;
            var itemValue = item.Value;
            var itemWidth = 2;

            if (ReferenceEquals(item, dateInputDay) || NameMatches(item.Name, "day", dateInputDay.Name))
            {
                itemName = item.Name ?? "day";
                itemValue = item.Value ?? dateInputDay.Value;
            }
            else if (ReferenceEquals(item, dateInputMonth) || NameMatches(item.Name, "month", dateInputMonth.Name))
            {
                itemName = item.Name ?? "month";
                itemValue = item.Value ?? dateInputMonth.Value;
            }
            else if (ReferenceEquals(item, dateInputYear) || NameMatches(item.Name, "year", dateInputYear.Name))
            {
                itemName = item.Name ?? "year";
                itemValue = item.Value ?? dateInputYear.Value;
                itemWidth = 4;
            }

            var itemHasErrorClass = ClassesContain(item.Classes, "govuk-input--error");
            var itemHasError = item.Error is true || itemHasErrorClass;

            // Add the error modifier when the item opts in (via `error` or its classes), or when the
            // component has an error message and no item has opted in itself.
            var errorClass = !itemHasErrorClass &&
                (itemHasError || (item.Error != false && options.ErrorMessage is not null && !anyItemHasError)) ?
                "govuk-input--error" : null;

            // Default the width modifier from the field when the item doesn't specify one.
            var widthClass = !ClassesContain(item.Classes, "govuk-input--width-") ?
                new TemplateString($"govuk-input--width-{itemWidth}") : null;

            var inputClasses = new TemplateString("govuk-date-input__input").AppendCssClasses(errorClass, widthClass, item.Classes);

            var labelText = item.Label ?? new TemplateString(Capitalize(itemName));
            var inputId = item.Id ?? new TemplateString($"{parentId}-{itemName}");
            var inputName = namePrefix.IsEmpty() ? itemName : new TemplateString($"{namePrefix}-{itemName}");
            var inputValue = itemValue ?? GetValue(values, inputName.ToText());

            var inputComponent = await GenerateInputAsync(new InputOptions
            {
                Label = new LabelOptions { Html = labelText, Classes = "govuk-date-input__label" },
                Id = inputId,
                Classes = inputClasses,
                Name = inputName,
                Value = inputValue,
                Type = "text",
                InputMode = item.InputMode ?? "numeric",
                AutoComplete = item.AutoComplete,
                Pattern = item.Pattern,
                Attributes = item.Attributes
            });

            itemDiv.InnerHtml.AppendHtml(inputComponent);

            return itemDiv;
        }
    }

    private static bool ClassesContain(TemplateString? classes, string token) =>
        !classes.IsEmpty() && classes.ToText().Contains(token, StringComparison.Ordinal);

    private static bool NameMatches(TemplateString? name, string defaultName, TemplateString? fieldName) =>
        !name.IsEmpty() && (name == defaultName || name == fieldName);

    private static TemplateString? GetValue(IReadOnlyDictionary<string, TemplateString?>? values, string? key) =>
        key is not null && values is not null && values.TryGetValue(key, out var value) ? value : null;
}
