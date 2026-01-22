using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateDateInputAsync(DateInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Determine if we have a fieldset
        var hasFieldset = options.Fieldset is not null;

        // Get the default items if not provided or empty
        var dateInputItems = options.Items is null || options.Items.Count == 0
            ?
            [
                new DateInputOptionsItem { Name = "day", Classes = "govuk-input--width-2" },
                new DateInputOptionsItem { Name = "month", Classes = "govuk-input--width-2" },
                new DateInputOptionsItem { Name = "year", Classes = "govuk-input--width-4" }
            ]
            : options.Items;

        // Build describedBy string for fieldset
        var describedByParts = new List<string>();
        if (!string.IsNullOrEmpty(options.Fieldset?.DescribedBy?.ToHtmlString()))
        {
            describedByParts.Add(options.Fieldset.DescribedBy.ToHtmlString());
        }

        // Build the form group div
        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        if (hasFieldset)
        {
            // Generate fieldset with legend and inner HTML
            var innerContent = await BuildInnerContentAsync();
            var fieldsetComponent = await GenerateFieldsetAsync(new FieldsetOptions
            {
                DescribedBy = describedByParts.Count > 0 ? string.Join(" ", describedByParts) : null,
                Classes = options.Fieldset!.Classes,
                Role = "group",
                Attributes = options.Fieldset.Attributes,
                Legend = options.Fieldset.Legend,
                Html = innerContent.ToHtmlString(_encoder)
            });

            formGroupDiv.InnerHtml.AppendHtml(fieldsetComponent.GetHtml());
        }
        else
        {
            var innerContent = await BuildInnerContentAsync();
            formGroupDiv.InnerHtml.AppendHtml(innerContent);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        // Build the inner HTML content
        async Task<IHtmlContent> BuildInnerContentAsync()
        {
            var innerHtmlBuilder = new HtmlContentBuilder();

            // Generate hint if present
            if (options.Hint is not null)
            {
                var hintId = options.Id?.ToHtmlString() + "-hint";
                describedByParts.Add(hintId);
                var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

                innerHtmlBuilder.AppendHtml(hintComponent.GetHtml());
            }

            // Generate error message if present
            if (options.ErrorMessage is not null)
            {
                var errorId = options.Id?.ToHtmlString() + "-error";
                describedByParts.Add(errorId);
                var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

                innerHtmlBuilder.AppendHtml(errorMessageComponent.GetHtml());
            }

            // Create the date input container div
            var dateInputDiv = new HtmlTag("div", attrs => attrs
                .With("id", options.Id)
                .WithClasses("govuk-date-input", options.Classes)
                .With(options.Attributes));

            // Add beforeInputs content if present
            if (options.FormGroup?.BeforeInputs is not null)
            {
                var beforeContent = HtmlOrText(options.FormGroup.BeforeInputs.Html, options.FormGroup.BeforeInputs.Text);
                if (!beforeContent.IsEmpty())
                {
                    dateInputDiv.InnerHtml.AppendHtml(beforeContent);
                }
            }

            // Generate each input item
            foreach (var item in dateInputItems)
            {
                var itemDiv = await CreateDateInputItemAsync(item, options.Id?.ToHtmlString(), options.NamePrefix?.ToHtmlString());
                dateInputDiv.InnerHtml.AppendHtml(itemDiv);
            }

            // Add afterInputs content if present
            if (options.FormGroup?.AfterInputs is not null)
            {
                var afterContent = HtmlOrText(options.FormGroup.AfterInputs.Html, options.FormGroup.AfterInputs.Text);
                if (!afterContent.IsEmpty())
                {
                    dateInputDiv.InnerHtml.AppendHtml(afterContent);
                }
            }

            innerHtmlBuilder.AppendHtml(dateInputDiv);
            return innerHtmlBuilder;
        }

        async Task<HtmlTag> CreateDateInputItemAsync(DateInputOptionsItem item, string? parentId, string? namePrefix)
        {
            // Create the item div wrapper
            var itemDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-date-input__item"));

            // Generate label text
            var labelText = item.Label?.ToHtmlString() ?? Capitalize(item.Name?.ToHtmlString() ?? "");
            var inputId = item.Id?.ToHtmlString() ?? (parentId + "-" + item.Name?.ToHtmlString());
            var inputName = (!string.IsNullOrEmpty(namePrefix) ? namePrefix + "-" : "") + item.Name?.ToHtmlString();
            var inputClasses = "govuk-date-input__input " + (item.Classes?.ToHtmlString() ?? "").TrimStart();

            // Generate the input component which includes label and input wrapped in a form-group
            var inputComponent = await GenerateInputAsync(new InputOptions
            {
                Label = new LabelOptions { Text = labelText, Classes = "govuk-date-input__label" },
                Id = inputId,
                Classes = inputClasses.TrimEnd(),
                Name = inputName,
                Value = item.Value,
                Type = "text",
                InputMode = item.InputMode ?? "numeric",
                AutoComplete = item.AutoComplete,
                Pattern = item.Pattern,
                Attributes = item.Attributes
            });

            itemDiv.InnerHtml.AppendHtml(inputComponent.GetHtml());

            return itemDiv;
        }
    }
}
