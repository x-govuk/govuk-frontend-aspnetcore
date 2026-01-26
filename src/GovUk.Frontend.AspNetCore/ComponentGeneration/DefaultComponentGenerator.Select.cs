namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateSelectInputAsync(SelectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;
        var classNames = new TemplateString("govuk-select").AppendCssClasses(options.Classes);
        if (options.ErrorMessage is not null)
        {
            classNames = classNames.AppendCssClasses("govuk-select--error");
        }

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var hasBeforeInput = !(options.FormGroup?.BeforeInput?.Html).IsEmpty() || !(options.FormGroup?.BeforeInput?.Text).IsEmpty();
        var hasAfterInput = !(options.FormGroup?.AfterInput?.Html).IsEmpty() || !(options.FormGroup?.AfterInput?.Text).IsEmpty();

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        var labelComponent = await GenerateLabelAsync(new LabelOptions
        {
            Html = options.Label?.Html,
            Text = options.Label?.Text,
            Classes = options.Label?.Classes,
            IsPageHeading = options.Label?.IsPageHeading,
            Attributes = options.Label?.Attributes,
            For = id
        });

        formGroupDiv.InnerHtml.AppendHtml(labelComponent.GetContent());

        if (options.Hint is not null)
        {
            var hintId = new TemplateString($"{id}-hint");
            describedByParts.Add(hintId);
            var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

            formGroupDiv.InnerHtml.AppendHtml(hintComponent.GetContent());
        }

        if (options.ErrorMessage is not null)
        {
            var errorId = new TemplateString($"{id}-error");
            describedByParts.Add(errorId);
            var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

            formGroupDiv.InnerHtml.AppendHtml(errorMessageComponent.GetContent());
        }

        if (hasBeforeInput)
        {
            var beforeContent = HtmlOrText(options.FormGroup!.BeforeInput!.Html, options.FormGroup.BeforeInput.Text);
            formGroupDiv.InnerHtml.AppendHtml(beforeContent);
        }

        var selectElement = CreateSelectElement();
        formGroupDiv.InnerHtml.AppendHtml(selectElement);

        if (hasAfterInput)
        {
            var afterContent = HtmlOrText(options.FormGroup!.AfterInput!.Html, options.FormGroup.AfterInput.Text);
            formGroupDiv.InnerHtml.AppendHtml(afterContent);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        HtmlTag CreateSelectElement()
        {
            var select = new HtmlTag("select", attrs => attrs
                .With("id", id)
                .WithClasses(classNames)
                .With("aria-describedby", TemplateString.Join(" ", describedByParts))
                .With(options.Attributes));

            // Always add name attribute, even if empty
            select.Attributes.Set("name", options.Name ?? TemplateString.Empty);

            // Add option elements
            if (options.Items is not null)
            {
                foreach (var item in options.Items)
                {
                    // Determine if this item should be selected
                    // If item.Selected is explicitly false, never select it
                    // Otherwise, select if item.Selected is true, or if the item's value/text matches the select's value
                    var isSelected = false;
                    
                    if (item.Selected is not false)
                    {
                        if (item.Selected is true)
                        {
                            isSelected = true;
                        }
                        else if (!options.Value.IsEmpty())
                        {
                            // If item has a value, compare with that; otherwise compare with text
                            var compareWith = item.Value ?? item.Text;
                            if (compareWith is not null && !compareWith.IsEmpty() && options.Value.ToHtmlString() == compareWith.ToHtmlString())
                            {
                                isSelected = true;
                            }
                        }
                    }

                    var option = new HtmlTag("option", attrs => attrs
                        .WithBoolean("disabled", item.Disabled is true)
                        .With(item.Attributes));

                    // Only set value attribute if item.Value is not null
                    if (item.Value is not null)
                    {
                        option.Attributes.Set("value", item.Value);
                    }

                    // Add selected attribute if selected
                    if (isSelected)
                    {
                        option.Attributes.Set("selected", TemplateString.Empty);
                    }

                    if (item.Text is not null && !item.Text.IsEmpty())
                    {
                        option.InnerHtml.AppendHtml(item.Text);
                    }

                    select.InnerHtml.AppendHtml(option);
                }
            }

            return select;
        }
    }
}
