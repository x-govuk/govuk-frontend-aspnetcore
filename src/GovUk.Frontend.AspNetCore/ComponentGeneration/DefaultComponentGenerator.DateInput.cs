using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateDateInputAsync(DateInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var hasFieldset = options.Fieldset is not null;

        var dateInputItems = options.Items is null || options.Items.Count == 0 ?
            [
                new DateInputOptionsItem { Name = "day", Classes = "govuk-input--width-2" },
                new DateInputOptionsItem { Name = "month", Classes = "govuk-input--width-2" },
                new DateInputOptionsItem { Name = "year", Classes = "govuk-input--width-4" }
            ] : options.Items;

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
                Html = innerContent.ToTemplateString()
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

            foreach (var item in dateInputItems)
            {
                var itemDiv = await CreateDateInputItemAsync(item, options.Id, options.NamePrefix);
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

        async Task<HtmlTag> CreateDateInputItemAsync(DateInputOptionsItem item, TemplateString? parentId, TemplateString? namePrefix)
        {
            var itemDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-date-input__item"));

            var labelText = item.Label ?? Capitalize(item.Name);
            var inputId = item.Id ?? new TemplateString($"{parentId}-{item.Name}");
            var inputName = namePrefix.IsEmpty() ? item.Name : new TemplateString($"{namePrefix}-{item.Name}");
            var inputClasses = new TemplateString("govuk-date-input__input").AppendCssClasses(item.Classes);

            var inputComponent = await GenerateInputAsync(new InputOptions
            {
                Label = new LabelOptions { Text = labelText, Classes = "govuk-date-input__label" },
                Id = inputId,
                Classes = inputClasses,
                Name = inputName,
                Value = item.Value,
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
}
