using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateInputAsync(InputOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Build the ID and class names
        var id = options.Id?.ToHtmlString() ?? options.Name?.ToHtmlString();
        var classNames = "govuk-input";
        if (!string.IsNullOrEmpty(options.Classes?.ToHtmlString()))
        {
            classNames += " " + options.Classes.ToHtmlString();
        }
        if (options.ErrorMessage is not null)
        {
            classNames += " govuk-input--error";
        }

        // Build describedBy
        var describedByParts = new List<string>();
        if (!string.IsNullOrEmpty(options.DescribedBy?.ToHtmlString()))
        {
            describedByParts.Add(options.DescribedBy.ToHtmlString());
        }

        // Check for prefix, suffix, and before/after input
        var hasPrefix = options.Prefix is not null && (!string.IsNullOrEmpty(options.Prefix.Text) || !(options.Prefix.Html?.IsEmpty() ?? true));
        var hasSuffix = options.Suffix is not null && (!string.IsNullOrEmpty(options.Suffix.Text?.ToHtmlString()) || !(options.Suffix.Html?.IsEmpty() ?? true));
        var hasBeforeInput = options.FormGroup?.BeforeInput is not null && (!string.IsNullOrEmpty(options.FormGroup.BeforeInput.Text?.ToHtmlString()) || !(options.FormGroup.BeforeInput.Html?.IsEmpty() ?? true));
        var hasAfterInput = options.FormGroup?.AfterInput is not null && (!string.IsNullOrEmpty(options.FormGroup.AfterInput.Text?.ToHtmlString()) || !(options.FormGroup.AfterInput.Html?.IsEmpty() ?? true));

        // Local function to create affix item (prefix or suffix)
        HtmlTag CreateAffixItem(TemplateString? html, TemplateString? text, TemplateString? classes, AttributeCollection? attributes, string type)
        {
            var affixDiv = new HtmlTag("div", attrs => attrs
                .WithClasses($"govuk-input__{type}", classes)
                .With("aria-hidden", "true")
                .With(attributes));

            var content = HtmlOrText(html, text);
            affixDiv.InnerHtml.AppendHtml(content);

            return affixDiv;
        }

        // Create the form group div
        var formGroupDiv = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-form-group", options.ErrorMessage is not null ? "govuk-form-group--error" : null, options.FormGroup?.Classes)
            .With(options.FormGroup?.Attributes));

        // Generate label
        var labelComponent = await GenerateLabelAsync(new LabelOptions
        {
            Html = options.Label?.Html,
            Text = options.Label?.Text,
            Classes = options.Label?.Classes,
            IsPageHeading = options.Label?.IsPageHeading,
            Attributes = options.Label?.Attributes,
            For = id
        });

        formGroupDiv.InnerHtml.AppendHtml(labelComponent.GetHtml());

        // Generate hint if present
        if (options.Hint is not null)
        {
            var hintId = id + "-hint";
            describedByParts.Add(hintId);
            var hintComponent = await GenerateHintAsync(options.Hint with { Id = hintId });

            formGroupDiv.InnerHtml.AppendHtml(hintComponent.GetHtml());
        }

        // Generate error message if present
        if (options.ErrorMessage is not null)
        {
            var errorId = id + "-error";
            describedByParts.Add(errorId);
            var errorMessageComponent = await GenerateErrorMessageAsync(options.ErrorMessage with { Id = errorId });

            formGroupDiv.InnerHtml.AppendHtml(errorMessageComponent.GetHtml());
        }

        // Add input with optional wrapper
        if (hasPrefix || hasSuffix || hasBeforeInput || hasAfterInput)
        {
            var wrapperDiv = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-input__wrapper", options.InputWrapper?.Classes)
                .With(options.InputWrapper?.Attributes));

            // Add beforeInput content if present
            if (hasBeforeInput)
            {
                var beforeContent = HtmlOrText(options.FormGroup!.BeforeInput!.Html, options.FormGroup.BeforeInput.Text);
                wrapperDiv.InnerHtml.AppendHtml(beforeContent);
            }

            // Add prefix if present
            if (hasPrefix)
            {
                var prefixDiv = CreateAffixItem(options.Prefix!.Html, options.Prefix.Text, options.Prefix.Classes, options.Prefix.Attributes, "prefix");
                wrapperDiv.InnerHtml.AppendHtml(prefixDiv);
            }

            // Add input element
            var inputElement = CreateInputElement();
            wrapperDiv.InnerHtml.AppendHtml(inputElement);

            // Add suffix if present
            if (hasSuffix)
            {
                var suffixDiv = CreateAffixItem(options.Suffix!.Html, options.Suffix.Text, options.Suffix.Classes, options.Suffix.Attributes, "suffix");
                wrapperDiv.InnerHtml.AppendHtml(suffixDiv);
            }

            // Add afterInput content if present
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

        // Local function to create the input element
        HtmlTag CreateInputElement()
        {
            var input = new HtmlTag("input", attrs => attrs
                .With("id", id)
                .With("type", options.Type ?? "text")
                .WithClasses(classNames)
                .With("value", options.Value)
                .With("aria-describedby", describedByParts.Count > 0 ? string.Join(" ", describedByParts) : null)
                .With("autocomplete", options.AutoComplete)
                .With("autocapitalize", options.AutoCapitalize)
                .With("pattern", options.Pattern)
                .With("inputmode", options.InputMode)
                .With(options.Attributes));

            // Always add name attribute, even if empty
            input.Attributes.Set("name", options.Name?.ToHtmlString() ?? string.Empty);

            // Handle spellcheck - only add if explicitly set
            if (options.Spellcheck is true or false)
            {
                input.Attributes.Set("spellcheck", options.Spellcheck.Value ? "true" : "false");
            }

            // Handle disabled
            if (options.Disabled is true)
            {
                input.Attributes.AddBoolean("disabled");
            }

            input.TagRenderMode = TagRenderMode.SelfClosing;
            return input;
        }
    }
}
