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

        AddI18nPluralAttributes(formGroupAttributes, "characters-under-limit", new PluralTexts(
            Other: options.CharactersUnderLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOther),
            Zero: options.CharactersUnderLimitText?.Zero ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextZero),
            One: options.CharactersUnderLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOne),
            Two: options.CharactersUnderLimitText?.Two ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextTwo),
            Few: options.CharactersUnderLimitText?.Few ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextFew),
            Many: options.CharactersUnderLimitText?.Many ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextMany)));

        AddI18nSingularAttribute(
            formGroupAttributes,
            "characters-at-limit",
            options.CharactersAtLimitText ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersAtLimitText));

        AddI18nPluralAttributes(formGroupAttributes, "characters-over-limit", new PluralTexts(
            Other: options.CharactersOverLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOther),
            Zero: options.CharactersOverLimitText?.Zero ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextZero),
            One: options.CharactersOverLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOne),
            Two: options.CharactersOverLimitText?.Two ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextTwo),
            Few: options.CharactersOverLimitText?.Few ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextFew),
            Many: options.CharactersOverLimitText?.Many ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextMany)));

        AddI18nPluralAttributes(formGroupAttributes, "words-under-limit", new PluralTexts(
            Other: options.WordsUnderLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOther),
            Zero: options.WordsUnderLimitText?.Zero ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextZero),
            One: options.WordsUnderLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOne),
            Two: options.WordsUnderLimitText?.Two ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextTwo),
            Few: options.WordsUnderLimitText?.Few ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextFew),
            Many: options.WordsUnderLimitText?.Many ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextMany)));

        AddI18nSingularAttribute(
            formGroupAttributes,
            "words-at-limit",
            options.WordsAtLimitText ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsAtLimitText));

        AddI18nPluralAttributes(formGroupAttributes, "words-over-limit", new PluralTexts(
            Other: options.WordsOverLimitText?.Other ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOther),
            Zero: options.WordsOverLimitText?.Zero ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextZero),
            One: options.WordsOverLimitText?.One ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOne),
            Two: options.WordsOverLimitText?.Two ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextTwo),
            Few: options.WordsOverLimitText?.Few ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextFew),
            Many: options.WordsOverLimitText?.Many ?? LocalizedText(GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextMany)));

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

    /// <summary>
    /// The text for a message that varies by plural category, in the order govuk-frontend writes them.
    /// </summary>
    /// <remarks>
    /// The categories are CLDR's, which is what <c>Intl.PluralRules</c> selects between in the browser.
    /// </remarks>
    private readonly record struct PluralTexts(
        string? Other,
        string? Zero,
        string? One,
        string? Two,
        string? Few,
        string? Many);

    /// <summary>
    /// Writes a <c>data-i18n</c> attribute for each plural category that has content.
    /// </summary>
    /// <remarks>
    /// Categories without content are left out rather than filled in from <see cref="PluralTexts.Other"/>,
    /// matching govuk-frontend, whose macro writes exactly the categories it is given. That means a
    /// category a translation omits falls back to govuk-frontend's own English default in the browser,
    /// not to the translation's <c>other</c>.
    /// </remarks>
    private static void AddI18nPluralAttributes(AttributeCollection attributes, string key, PluralTexts texts)
    {
        Add("other", texts.Other);
        Add("zero", texts.Zero);
        Add("one", texts.One);
        Add("two", texts.Two);
        Add("few", texts.Few);
        Add("many", texts.Many);

        void Add(string category, string? text)
        {
            if (!text.IsEmpty())
            {
                attributes.Set($"data-i18n.{key}.{category}", text);
            }
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
