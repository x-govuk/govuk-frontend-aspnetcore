using System.Globalization;
using GovUk.Frontend.AspNetCore.Localization;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var id = options.Id ?? options.Name;

        var hasNoLimit = !options.MaxWords.HasValue && !options.MaxLength.HasValue;
        var textareaDescriptionLength = options.MaxWords ?? options.MaxLength;

        // %{count} is substituted below (when there's a limit) or by the JavaScript (when there isn't),
        // so it has to survive localization verbatim.
        var localizedTextareaDescriptionText = LocalizedText(options.MaxWords.HasValue
            ? GovUkFrontendResourceNames.CharacterCountTextareaDescriptionTextWords
            : GovUkFrontendResourceNames.CharacterCountTextareaDescriptionTextCharacters);

        var specifiedTextareaDescriptionText = options.TextareaDescriptionText ?? localizedTextareaDescriptionText;

        var textareaDescriptionText = specifiedTextareaDescriptionText ??
            $"You can enter up to %{{count}} {(options.MaxWords.HasValue ? "words" : "characters")}";

        var textareaDescriptionTextNoLimit = !hasNoLimit && textareaDescriptionLength.HasValue
            ? textareaDescriptionText.Replace(
                "%{count}",
                textareaDescriptionLength.Value.ToString(CultureInfo.InvariantCulture),
                StringComparison.Ordinal)
            : null;

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

        var afterInputContent = new HtmlContentBuilder();
        afterInputContent.AppendHtml(countMessageHint.GetContent());

        if (options.FormGroup?.AfterInput is not null)
        {
            var afterInputHtml = HtmlOrText(options.FormGroup.AfterInput.Html, options.FormGroup.AfterInput.Text);
            afterInputContent.AppendHtml(afterInputHtml);
        }

        var formGroupAttributes = new AttributeCollection()
        {
            { "data-module", "govuk-character-count" }
        };

        // Add maxlength/maxwords/threshold data attributes
        if (options.MaxLength.HasValue)
        {
            formGroupAttributes.Set("data-maxlength", options.MaxLength.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (options.MaxWords.HasValue)
        {
            formGroupAttributes.Set("data-maxwords", options.MaxWords.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (options.Threshold.HasValue)
        {
            formGroupAttributes.Set("data-threshold", options.Threshold.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (hasNoLimit && specifiedTextareaDescriptionText is not null && !specifiedTextareaDescriptionText.IsEmpty())
        {
            formGroupAttributes.Set("data-i18n.textarea-description.other", specifiedTextareaDescriptionText);
        }

        AddI18nPluralAttributes(
            formGroupAttributes,
            "characters-under-limit",
            options.CharactersUnderLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOther),
            options.CharactersUnderLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOne));

        AddI18nSingularAttribute(
            formGroupAttributes,
            "characters-at-limit",
            options.CharactersAtLimitText ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersAtLimitText));

        AddI18nPluralAttributes(
            formGroupAttributes,
            "characters-over-limit",
            options.CharactersOverLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOther),
            options.CharactersOverLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOne));

        AddI18nPluralAttributes(
            formGroupAttributes,
            "words-under-limit",
            options.WordsUnderLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOther),
            options.WordsUnderLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOne));

        AddI18nSingularAttribute(
            formGroupAttributes,
            "words-at-limit",
            options.WordsAtLimitText ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsAtLimitText));

        AddI18nPluralAttributes(
            formGroupAttributes,
            "words-over-limit",
            options.WordsOverLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOther),
            options.WordsOverLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOne));

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

        var textareaClasses = new TemplateString("govuk-js-character-count")
            .AppendCssClasses(options.Classes);

        var formGroupClasses = new TemplateString("govuk-character-count")
            .AppendCssClasses(options.FormGroup?.Classes);

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
