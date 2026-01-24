using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GeneratePasswordInputAsync(PasswordInputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses(
                "govuk-form-group",
                "govuk-password-input",
                options.ErrorMessage is not null ? "govuk-form-group--error" : null,
                options.FormGroup?.Classes)
            .With("data-module", "govuk-password-input")
            .With("data-i18n.show-password", options.ShowPasswordText)
            .With("data-i18n.hide-password", options.HidePasswordText)
            .With("data-i18n.show-password-aria-label", options.ShowPasswordAriaLabelText)
            .With("data-i18n.hide-password-aria-label", options.HidePasswordAriaLabelText)
            .With("data-i18n.password-shown-announcement", options.PasswordShownAnnouncementText)
            .With("data-i18n.password-hidden-announcement", options.PasswordHiddenAnnouncementText)
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

        var hasBeforeInput = !(options.FormGroup?.BeforeInput?.Html).IsEmpty() || !(options.FormGroup?.BeforeInput?.Text).IsEmpty();

        var wrapperDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-input__wrapper", "govuk-password-input__wrapper"));

        if (hasBeforeInput)
        {
            var beforeContent = HtmlOrText(options.FormGroup!.BeforeInput!.Html, options.FormGroup.BeforeInput.Text);
            wrapperDiv.InnerHtml.AppendHtml(beforeContent);
        }

        var inputClasses = new TemplateString("govuk-input")
            .AppendCssClasses("govuk-password-input__input")
            .AppendCssClasses("govuk-js-password-input-input")
            .AppendCssClasses(options.Classes);

        if (options.ErrorMessage is not null)
        {
            inputClasses = inputClasses.AppendCssClasses("govuk-input--error");
        }

        var inputElement = new HtmlTag("input", attrs => attrs
            .With("id", id)
            .With("type", "password")
            .WithClasses(inputClasses)
            .With("value", options.Value)
            .With("aria-describedby", TemplateString.Join(" ", describedByParts))
            .With("autocomplete", options.AutoComplete ?? "current-password")
            .With("autocapitalize", "none")
            .WithBoolean("disabled", options.Disabled is true)
            .With(options.Attributes)
            .With("spellcheck", "false"));

        // Always add name attribute, even if empty
        inputElement.Attributes.Set("name", options.Name ?? TemplateString.Empty);

        inputElement.TagRenderMode = TagRenderMode.SelfClosing;

        wrapperDiv.InnerHtml.AppendHtml(inputElement);

        var buttonClasses = new TemplateString("govuk-button")
            .AppendCssClasses("govuk-button--secondary")
            .AppendCssClasses("govuk-password-input__toggle")
            .AppendCssClasses("govuk-js-password-input-toggle")
            .AppendCssClasses(options.Button?.Classes);

        var buttonElement = new HtmlTag("button", attrs => attrs
            .With("type", "button")
            .WithClasses(buttonClasses)
            .With("data-module", "govuk-button")
            .With("aria-controls", id)
            .With("aria-label", options.ShowPasswordAriaLabelText ?? "Show password")
            .WithBoolean("hidden"));

        buttonElement.InnerHtml.Append((options.ShowPasswordText ?? "Show").ToHtmlString());

        wrapperDiv.InnerHtml.AppendHtml(buttonElement);

        if (options.FormGroup?.AfterInput is not null)
        {
            var afterContent = HtmlOrText(options.FormGroup.AfterInput.Html, options.FormGroup.AfterInput.Text);
            wrapperDiv.InnerHtml.AppendHtml(afterContent);
        }

        formGroupDiv.InnerHtml.AppendHtml(wrapperDiv);

        return await GenerateFromHtmlTagAsync(formGroupDiv);
    }
}
