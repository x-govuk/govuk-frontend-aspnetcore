namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateSelectAsync(SelectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id.ToTemplateString() ?? options.Name.ToTemplateString();
        var classNames = new TemplateString("govuk-select").AppendCssClasses(options.Classes.ToTemplateString());
        if (options.ErrorMessage is not null)
        {
            classNames = classNames.AppendCssClasses("govuk-select--error");
        }

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy.ToTemplateString() is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var hasBeforeInput = !string.IsNullOrEmpty(options.FormGroup?.BeforeInput?.Html) || !string.IsNullOrEmpty(options.FormGroup?.BeforeInput?.Text);
        var hasAfterInput = !string.IsNullOrEmpty(options.FormGroup?.AfterInput?.Html) || !string.IsNullOrEmpty(options.FormGroup?.AfterInput?.Text);

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes.ToTemplateString())
            .With(options.FormGroup?.Attributes));

        if (options.Label is not null)
        {
            var labelComponent = await GenerateLabelAsync(new LabelOptions
            {
                Html = options.Label.Html,
                Text = options.Label.Text,
                Classes = options.Label.Classes,
                IsPageHeading = options.Label.IsPageHeading,
                Attributes = options.Label.Attributes,
                For = id
            });

            formGroupDiv.InnerHtml.AppendHtml(labelComponent.GetContent());
        }

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
            select.Attributes.Set("name", options.Name.ToTemplateString() ?? TemplateString.Empty);

            if (options.Items is not null)
            {
                foreach (var item in options.Items)
                {
                    var option = new HtmlTag("option", attrs => attrs
                        .With("value", item.Value.ToTemplateString())
                        .WithBoolean("selected", item.Selected is true)
                        .WithBoolean("disabled", item.Disabled is true)
                        .With(item.Attributes));

                    var content = item.Text ?? string.Empty;
                    option.InnerHtml.AppendHtml(content);

                    select.InnerHtml.AppendHtml(option);
                }
            }

            return select;
        }
    }
}
