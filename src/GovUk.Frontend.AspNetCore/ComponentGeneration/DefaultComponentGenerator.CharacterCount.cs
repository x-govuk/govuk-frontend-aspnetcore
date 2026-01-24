using System.Globalization;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;

        // Determine if we have a limit
        var hasNoLimit = !options.MaxWords.HasValue && !options.MaxLength.HasValue;
        var textareaDescriptionLength = options.MaxWords ?? options.MaxLength;
        var textareaDescriptionText = options.TextareaDescriptionText ??
            new TemplateString($"You can enter up to %{{count}} {(options.MaxWords.HasValue ? "words" : "characters")}");

        TemplateString textareaDescriptionTextNoLimit;
        if (!hasNoLimit && textareaDescriptionLength.HasValue)
        {
            var textareaDescriptionStr = textareaDescriptionText.ToHtmlString() ?? string.Empty;
            textareaDescriptionTextNoLimit = new TemplateString(textareaDescriptionStr.Replace("%{count}", textareaDescriptionLength.Value.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal));
        }
        else
        {
            textareaDescriptionTextNoLimit = TemplateString.Empty;
        }

        // Build the count message (hint) that will appear in formGroup.afterInput
        var countMessageId = new TemplateString($"{id}-info");
        var countMessageClasses = new TemplateString("govuk-character-count__message")
            .AppendCssClasses(options.CountMessage?.Classes);

        var countMessageHint = await GenerateHintAsync(new HintOptions
        {
            Text = textareaDescriptionTextNoLimit,
            Id = countMessageId,
            Classes = countMessageClasses,
            Attributes = options.CountMessage?.Attributes
        });

        // Prepare afterInput content (count message + any user-provided afterInput)
        var afterInputContent = new HtmlContentBuilder();
        afterInputContent.AppendHtml(countMessageHint.GetContent());

        if (options.FormGroup?.AfterInput is not null)
        {
            var afterInputHtml = HtmlOrText(options.FormGroup.AfterInput.Html, options.FormGroup.AfterInput.Text);
            afterInputContent.AppendHtml(afterInputHtml);
        }

        // Build attributes for the form group wrapper
        var formGroupAttributes = new AttributeCollection();

        // Add data-module attribute
        formGroupAttributes.Set("data-module", "govuk-character-count");

        // Add maxlength/maxwords/threshold data attributes
        if (options.MaxLength.HasValue)
        {
            formGroupAttributes.Set("data-maxlength", new TemplateString(options.MaxLength.Value.ToString(CultureInfo.InvariantCulture)));
        }

        if (options.MaxWords.HasValue)
        {
            formGroupAttributes.Set("data-maxwords", new TemplateString(options.MaxWords.Value.ToString(CultureInfo.InvariantCulture)));
        }

        if (options.Threshold.HasValue)
        {
            formGroupAttributes.Set("data-threshold", new TemplateString(options.Threshold.Value.ToString(CultureInfo.InvariantCulture)));
        }

        // Add i18n attributes for textarea-description (only if no limit and explicitly provided)
        if (hasNoLimit && options.TextareaDescriptionText?.IsEmpty() is false)
        {
            formGroupAttributes.Set("data-i18n.textarea-description.other", options.TextareaDescriptionText);
        }

        // Add i18n attributes for character/word limits
        AddI18nPluralAttributes(formGroupAttributes, "characters-under-limit", options.CharactersUnderLimitText?.Other, options.CharactersUnderLimitText?.One);
        AddI18nSingularAttribute(formGroupAttributes, "characters-at-limit", options.CharactersAtLimitText);
        AddI18nPluralAttributes(formGroupAttributes, "characters-over-limit", options.CharactersOverLimitText?.Other, options.CharactersOverLimitText?.One);
        AddI18nPluralAttributes(formGroupAttributes, "words-under-limit", options.WordsUnderLimitText?.Other, options.WordsUnderLimitText?.One);
        AddI18nSingularAttribute(formGroupAttributes, "words-at-limit", options.WordsAtLimitText);
        AddI18nPluralAttributes(formGroupAttributes, "words-over-limit", options.WordsOverLimitText?.Other, options.WordsOverLimitText?.One);

        // Merge with user-provided formGroup attributes
        if (options.FormGroup?.Attributes is not null)
        {
            foreach (var attr in options.FormGroup.Attributes)
            {
                if (attr.Value is not null)
                {
                    formGroupAttributes.Set(attr.Key, attr.Value);
                }
            }
        }

        // Prepare classes for the textarea
        var textareaClasses = new TemplateString("govuk-js-character-count")
            .AppendCssClasses(options.Classes);

        // Prepare formGroup classes for the wrapper
        var formGroupClasses = new TemplateString("govuk-character-count")
            .AppendCssClasses(options.FormGroup?.Classes);

        // Generate the textarea using GenerateTextareaAsync
        var textareaOptions = new TextareaOptions
        {
            Id = id,
            Name = options.Name,
            Rows = options.Rows,
            Spellcheck = options.Spellcheck,
            Value = options.Value,
            Classes = textareaClasses,
            Label = options.Label,
            Hint = options.Hint,
            ErrorMessage = options.ErrorMessage,
            Attributes = options.Attributes,
            DescribedBy = countMessageId,
            FormGroup = new TextareaOptionsFormGroup
            {
                Classes = formGroupClasses,
                Attributes = formGroupAttributes,
                BeforeInput = options.FormGroup?.BeforeInput is not null
                    ? new TextareaOptionsBeforeInput
                    {
                        Html = options.FormGroup.BeforeInput.Html,
                        Text = options.FormGroup.BeforeInput.Text
                    }
                    : null,
                AfterInput = new TextareaOptionsAfterInput
                {
                    Html = new TemplateString(afterInputContent)
                }
            }
        };

        return await GenerateTextareaAsync(textareaOptions);
    }

    private static void AddI18nPluralAttributes(AttributeCollection attributes, string key, TemplateString? other, TemplateString? one)
    {
        if (other is not null && !other.IsEmpty())
        {
            attributes.Set($"data-i18n.{key}.other", other);
        }

        if (one is not null && !one.IsEmpty())
        {
            attributes.Set($"data-i18n.{key}.one", one);
        }
    }

    private static void AddI18nSingularAttribute(AttributeCollection attributes, string key, TemplateString? message)
    {
        if (message is not null && !message.IsEmpty())
        {
            attributes.Set($"data-i18n.{key}", message);
        }
    }
}
