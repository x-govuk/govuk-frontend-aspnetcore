using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateTextareaAsync(TextareaOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;
        var classNames = new TemplateString("govuk-textarea").AppendCssClasses(options.Classes);
        if (options.ErrorMessage is not null)
        {
            classNames = classNames.AppendCssClasses("govuk-textarea--error");
        }

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var hasBeforeInput = !string.IsNullOrEmpty(options.FormGroup?.BeforeInput?.Html) || !string.IsNullOrEmpty(options.FormGroup?.BeforeInput?.Text);
        var hasAfterInput = !string.IsNullOrEmpty(options.FormGroup?.AfterInput?.Html) || !string.IsNullOrEmpty(options.FormGroup?.AfterInput?.Text);

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
            var beforeContent = HtmlOrTextString(options.FormGroup!.BeforeInput!.Html, options.FormGroup.BeforeInput.Text);
            formGroupDiv.InnerHtml.AppendHtml(beforeContent);
        }

        var textareaElement = CreateTextareaElement();
        formGroupDiv.InnerHtml.AppendHtml(textareaElement);

        if (hasAfterInput)
        {
            var afterContent = HtmlOrTextString(options.FormGroup!.AfterInput!.Html, options.FormGroup.AfterInput.Text);
            formGroupDiv.InnerHtml.AppendHtml(afterContent);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        IHtmlContent HtmlOrTextString(string? html, string? text)
        {
            if (!string.IsNullOrEmpty(html))
            {
                return new HtmlString(html);
            }

            if (!string.IsNullOrEmpty(text))
            {
                // Text needs to be encoded
                return new HtmlContentBuilder().Append(text);
            }

            return HtmlString.Empty;
        }

        HtmlTag CreateTextareaElement()
        {
            var textarea = new HtmlTag("textarea", attrs => attrs
                .With("id", id)
                .WithClasses(classNames)
                .With("rows", new TemplateString((options.Rows ?? 5).ToString(System.Globalization.CultureInfo.InvariantCulture)))
                .With("aria-describedby", TemplateString.Join(" ", describedByParts))
                .With("autocomplete", options.AutoComplete)
                .WithBoolean("disabled", options.Disabled is true)
                .With(options.Attributes));

            // Always add name attribute, even if empty
            textarea.Attributes.Set("name", options.Name ?? TemplateString.Empty);

            if (options.Spellcheck is true or false)
            {
                textarea.Attributes.Set("spellcheck", options.Spellcheck.Value ? "true" : "false");
            }

            if (options.Value is not null && !options.Value.IsEmpty())
            {
                textarea.InnerHtml.AppendHtml(options.Value);
            }

            return textarea;
        }
    }
}
