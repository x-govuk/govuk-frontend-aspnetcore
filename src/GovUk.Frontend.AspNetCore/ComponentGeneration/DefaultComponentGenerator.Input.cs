using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateInputAsync(InputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;
        var classNames = new TemplateString("govuk-input").AppendCssClasses(options.Classes);
        if (options.ErrorMessage is not null)
        {
            classNames = classNames.AppendCssClasses("govuk-input--error");
        }

        var describedByParts = new List<TemplateString>();
        if (options.DescribedBy is var describedBy && !describedBy.IsEmpty())
        {
            describedByParts.Add(describedBy);
        }

        var hasPrefix = !(options.Prefix?.Html).IsEmpty() || !(options.Prefix?.Text).IsEmpty();
        var hasSuffix = !(options.Suffix?.Html).IsEmpty() || !(options.Suffix?.Text).IsEmpty();
        var hasBeforeInput = !(options.FormGroup?.BeforeInput?.Html).IsEmpty() || !(options.FormGroup?.BeforeInput?.Text).IsEmpty();
        var hasAfterInput = !(options.FormGroup?.AfterInput?.Html).IsEmpty() || !(options.FormGroup?.AfterInput?.Text).IsEmpty();

        IHtmlContent CreateAffixItem(TemplateString? html, TemplateString? text, TemplateString? classes, AttributeCollection? attributes, string type)
        {
            var affixDiv = new HtmlTag("div", attrs => attrs
                .WithClasses(new TemplateString($"govuk-input__{type}"), classes)
                .With("aria-hidden", "true")
                .With(attributes));

            var content = HtmlOrText(html, text);
            affixDiv.InnerHtml.AppendHtml(content);

            return affixDiv;
        }

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

        if (hasPrefix || hasSuffix || hasBeforeInput || hasAfterInput)
        {
            var wrapperDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-input__wrapper", options.InputWrapper?.Classes)
                .With(options.InputWrapper?.Attributes));

            if (hasBeforeInput)
            {
                var beforeContent = HtmlOrText(options.FormGroup!.BeforeInput!.Html, options.FormGroup.BeforeInput.Text);
                wrapperDiv.InnerHtml.AppendHtml(beforeContent);
            }

            if (hasPrefix)
            {
                var prefixDiv = CreateAffixItem(options.Prefix!.Html, options.Prefix.Text, options.Prefix.Classes, options.Prefix.Attributes, "prefix");
                wrapperDiv.InnerHtml.AppendHtml(prefixDiv);
            }

            var inputElement = CreateInputElement();
            wrapperDiv.InnerHtml.AppendHtml(inputElement);

            if (hasSuffix)
            {
                var suffixDiv = CreateAffixItem(options.Suffix!.Html, options.Suffix.Text, options.Suffix.Classes, options.Suffix.Attributes, "suffix");
                wrapperDiv.InnerHtml.AppendHtml(suffixDiv);
            }

            if (hasAfterInput)
            {
                var afterContent = HtmlOrText(options.FormGroup!.AfterInput!.Html, options.FormGroup.AfterInput.Text);
                wrapperDiv.InnerHtml.AppendHtml(afterContent);
            }

            formGroupDiv.InnerHtml.AppendHtml(wrapperDiv);
        }
        else
        {
            var inputElement = CreateInputElement();
            formGroupDiv.InnerHtml.AppendHtml(inputElement);
        }

        return await GenerateFromHtmlTagAsync(formGroupDiv);

        HtmlTag CreateInputElement()
        {
            var input = new HtmlTag("input", attrs => attrs
                .With("id", id)
                .With("type", options.Type ?? "text")
                .WithClasses(classNames)
                .With("value", options.Value)
                .With("aria-describedby", TemplateString.Join(" ", describedByParts))
                .With("autocomplete", options.AutoComplete)
                .With("autocapitalize", options.AutoCapitalize)
                .With("pattern", options.Pattern)
                .With("inputmode", options.InputMode)
                .WithBoolean("disabled", options.Disabled is true)
                .With(options.Attributes));

            // Always add name attribute, even if empty
            input.Attributes.Set("name", options.Name ?? TemplateString.Empty);

            if (options.Spellcheck is true or false)
            {
                input.Attributes.Set("spellcheck", options.Spellcheck.Value ? "true" : "false");
            }

            input.TagRenderMode = TagRenderMode.SelfClosing;
            return input;
        }
    }
}
