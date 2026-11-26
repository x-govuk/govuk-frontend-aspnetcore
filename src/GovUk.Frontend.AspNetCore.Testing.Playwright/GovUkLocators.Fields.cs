using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

public static partial class GovUkLocators
{
    private const string TextInputSelector =
        "input.govuk-input:not(.govuk-date-input__input):not(.govuk-password-input__input)";

    private const string TextareaSelector = "textarea.govuk-textarea";
    private const string CharacterCountSelector = ".govuk-character-count textarea.govuk-textarea";
    private const string SelectSelector = "select.govuk-select";
    private const string PasswordInputSelector = "input.govuk-password-input__input";
    private const string FileUploadSelector = "input.govuk-file-upload";

    private const string LabelledControlSelector = "input.govuk-input:not(.govuk-date-input__input), " +
        $"{TextareaSelector}, {SelectSelector}, {FileUploadSelector}";

    private const string DateInputSelector = ":scope > .govuk-date-input";
    private const string RadiosSelector = ":scope > .govuk-radios";
    private const string CheckboxesSelector = ":scope > .govuk-checkboxes";

    extension(IPage page)
    {
        /// <summary>
        /// Gets the field of any kind whose label or legend is <paramref name="labelOrLegend"/>.
        /// </summary>
        public FieldLocator GetFieldByLabelOrLegend(string labelOrLegend) =>
            GetFieldByLabelOrLegend(LocatorScope.For(page), labelOrLegend);

        /// <summary>
        /// Gets the text input labelled <paramref name="label"/>.
        /// </summary>
        public TextInputLocator GetTextInputByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, TextInputSelector));

        /// <summary>
        /// Gets the textarea labelled <paramref name="label"/>, including a character count's.
        /// </summary>
        public TextareaLocator GetTextareaByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, TextareaSelector));

        /// <summary>
        /// Gets the character count labelled <paramref name="label"/>.
        /// </summary>
        public CharacterCountLocator GetCharacterCountByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, CharacterCountSelector));

        /// <summary>
        /// Gets the select labelled <paramref name="label"/>.
        /// </summary>
        public SelectLocator GetSelectByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, SelectSelector));

        /// <summary>
        /// Gets the password input labelled <paramref name="label"/>.
        /// </summary>
        public PasswordInputLocator GetPasswordInputByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, PasswordInputSelector));

        /// <summary>
        /// Gets the file upload labelled <paramref name="label"/>.
        /// </summary>
        public FileUploadLocator GetFileUploadByLabel(string label) =>
            new(GetControl(LocatorScope.For(page), label, FileUploadSelector));

        /// <summary>
        /// Gets the date input whose legend is <paramref name="legend"/>.
        /// </summary>
        public DateInputLocator GetDateInputByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(page), legend, DateInputSelector));

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>.
        /// </summary>
        public RadiosLocator GetRadiosByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(page), legend, RadiosSelector));

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>.
        /// </summary>
        public CheckboxesLocator GetCheckboxesByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(page), legend, CheckboxesSelector));
    }

    extension(ILocator locator)
    {
        /// <summary>
        /// Gets the field of any kind within this locator whose label or legend is <paramref name="labelOrLegend"/>.
        /// </summary>
        public FieldLocator GetFieldByLabelOrLegend(string labelOrLegend) =>
            GetFieldByLabelOrLegend(LocatorScope.For(locator), labelOrLegend);

        /// <summary>
        /// Gets the text input within this locator labelled <paramref name="label"/>.
        /// </summary>
        public TextInputLocator GetTextInputByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, TextInputSelector));

        /// <summary>
        /// Gets the textarea within this locator labelled <paramref name="label"/>, including a character count's.
        /// </summary>
        public TextareaLocator GetTextareaByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, TextareaSelector));

        /// <summary>
        /// Gets the character count within this locator labelled <paramref name="label"/>.
        /// </summary>
        public CharacterCountLocator GetCharacterCountByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, CharacterCountSelector));

        /// <summary>
        /// Gets the select within this locator labelled <paramref name="label"/>.
        /// </summary>
        public SelectLocator GetSelectByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, SelectSelector));

        /// <summary>
        /// Gets the password input within this locator labelled <paramref name="label"/>.
        /// </summary>
        public PasswordInputLocator GetPasswordInputByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, PasswordInputSelector));

        /// <summary>
        /// Gets the file upload within this locator labelled <paramref name="label"/>.
        /// </summary>
        public FileUploadLocator GetFileUploadByLabel(string label) =>
            new(GetControl(LocatorScope.For(locator), label, FileUploadSelector));

        /// <summary>
        /// Gets the date input within this locator whose legend is <paramref name="legend"/>.
        /// </summary>
        public DateInputLocator GetDateInputByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(locator), legend, DateInputSelector));

        /// <summary>
        /// Gets the radios within this locator whose legend is <paramref name="legend"/>.
        /// </summary>
        public RadiosLocator GetRadiosByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(locator), legend, RadiosSelector));

        /// <summary>
        /// Gets the checkboxes within this locator whose legend is <paramref name="legend"/>.
        /// </summary>
        public CheckboxesLocator GetCheckboxesByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(locator), legend, CheckboxesSelector));
    }

    private static FieldLocator GetFieldByLabelOrLegend(LocatorScope scope, string labelOrLegend)
    {
        var control = GetControl(scope, labelOrLegend, LabelledControlSelector);
        var fieldset = GetFieldset(scope, labelOrLegend, $"{DateInputSelector}, {RadiosSelector}, {CheckboxesSelector}");

        return new FieldLocator(control.Or(fieldset).Locator(Selectors.ClosestFormGroup));
    }

    /// <summary>
    /// Gets the form control labelled <paramref name="label"/> that matches <paramref name="selector"/>.
    /// </summary>
    private static ILocator GetControl(LocatorScope scope, string label, string selector) =>
        scope.GetByLabel(label).And(scope.Locator(selector));

    /// <summary>
    /// Gets the fieldset whose legend is <paramref name="legend"/> and that has a child matching
    /// <paramref name="childSelector"/>, e.g. radios.
    /// </summary>
    private static ILocator GetFieldset(LocatorScope scope, string legend, string childSelector) =>
        scope.GetByRole(AriaRole.Group, legend)
            .And(scope.Locator("fieldset"))
            .Filter(new() { Has = scope.Page.Locator(childSelector) });
}
