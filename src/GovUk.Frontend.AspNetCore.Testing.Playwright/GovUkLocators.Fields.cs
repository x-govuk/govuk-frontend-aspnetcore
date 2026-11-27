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

    private static readonly string[] _datePartNames = ["day", "month", "year"];

    private const string FillableControlSelector =
        "input.govuk-input:not(.govuk-date-input__input), textarea.govuk-textarea";

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
            new(GetFieldset(LocatorScope.For(page), legend, DateInputSelector).Locator(DateInputSelector));

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>.
        /// </summary>
        public RadiosLocator GetRadiosByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(page), legend, RadiosSelector).Locator(RadiosSelector));

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>.
        /// </summary>
        public CheckboxesLocator GetCheckboxesByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(page), legend, CheckboxesSelector).Locator(CheckboxesSelector));

        /// <summary>
        /// Locates the field of any kind that has the name <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// A field is found by the name it's submitted with, which for a date input is the prefix of its inputs' names,
        /// e.g. <c>DateOfBirth</c> for <c>DateOfBirth.Day</c> or <c>dob</c> for <c>dob-day</c>, or by the name of any of
        /// its inputs, e.g. <c>EmailUpdates</c> for checkboxes whose items have names of their own.
        /// </remarks>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        public FieldLocator GetFieldByName(string name) => GetFieldByName(LocatorScope.For(page), name);

        /// <summary>
        /// Locates the field of the kind <typeparamref name="TLocator"/> that has the name <paramref name="name"/>, e.g.
        /// <c>GetFieldByName&lt;RadiosLocator&gt;("Contact")</c>.
        /// </summary>
        /// <remarks>
        /// A field is found by name as <c>GetFieldByName(string)</c> finds it; fields of other kinds aren't matched.
        /// </remarks>
        /// <typeparam name="TLocator">
        /// The kind of field: <see cref="FieldLocator"/> for any kind, or one of <see cref="TextInputLocator"/>,
        /// <see cref="TextareaLocator"/>, <see cref="CharacterCountLocator"/>, <see cref="SelectLocator"/>,
        /// <see cref="PasswordInputLocator"/>, <see cref="FileUploadLocator"/>, <see cref="DateInputLocator"/>,
        /// <see cref="RadiosLocator"/> or <see cref="CheckboxesLocator"/>.
        /// </typeparam>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <exception cref="ArgumentException"><typeparamref name="TLocator"/> isn't one of those kinds.</exception>
        public TLocator GetFieldByName<TLocator>(string name)
            where TLocator : FieldLocator =>
            GetFieldByName<TLocator>(LocatorScope.For(page), name);
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
            new(GetFieldset(LocatorScope.For(locator), legend, DateInputSelector).Locator(DateInputSelector));

        /// <summary>
        /// Gets the radios within this locator whose legend is <paramref name="legend"/>.
        /// </summary>
        public RadiosLocator GetRadiosByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(locator), legend, RadiosSelector).Locator(RadiosSelector));

        /// <summary>
        /// Gets the checkboxes within this locator whose legend is <paramref name="legend"/>.
        /// </summary>
        public CheckboxesLocator GetCheckboxesByLegend(string legend) =>
            new(GetFieldset(LocatorScope.For(locator), legend, CheckboxesSelector).Locator(CheckboxesSelector));

        /// <summary>
        /// Locates the field of any kind that has the name <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// A field is found by the name it's submitted with, which for a date input is the prefix of its inputs' names,
        /// e.g. <c>DateOfBirth</c> for <c>DateOfBirth.Day</c> or <c>dob</c> for <c>dob-day</c>, or by the name of any of
        /// its inputs, e.g. <c>EmailUpdates</c> for checkboxes whose items have names of their own.
        /// </remarks>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        public FieldLocator GetFieldByName(string name) => GetFieldByName(LocatorScope.For(locator), name);

        /// <summary>
        /// Locates the field of the kind <typeparamref name="TLocator"/> that has the name <paramref name="name"/>, e.g.
        /// <c>GetFieldByName&lt;RadiosLocator&gt;("Contact")</c>.
        /// </summary>
        /// <remarks>
        /// A field is found by name as <c>GetFieldByName(string)</c> finds it; fields of other kinds aren't matched.
        /// </remarks>
        /// <typeparam name="TLocator">
        /// The kind of field: <see cref="FieldLocator"/> for any kind, or one of <see cref="TextInputLocator"/>,
        /// <see cref="TextareaLocator"/>, <see cref="CharacterCountLocator"/>, <see cref="SelectLocator"/>,
        /// <see cref="PasswordInputLocator"/>, <see cref="FileUploadLocator"/>, <see cref="DateInputLocator"/>,
        /// <see cref="RadiosLocator"/> or <see cref="CheckboxesLocator"/>.
        /// </typeparam>
        /// <param name="name">The field's name, e.g. <c>Email</c>.</param>
        /// <exception cref="ArgumentException"><typeparamref name="TLocator"/> isn't one of those kinds.</exception>
        public TLocator GetFieldByName<TLocator>(string name)
            where TLocator : FieldLocator =>
            GetFieldByName<TLocator>(LocatorScope.For(locator), name);
    }

    private static FieldLocator GetFieldByLabelOrLegend(LocatorScope scope, string labelOrLegend)
    {
        var control = GetControl(scope, labelOrLegend, LabelledControlSelector);
        var fieldset = GetFieldset(scope, labelOrLegend, $"{DateInputSelector}, {RadiosSelector}, {CheckboxesSelector}");

        return new FieldLocator(control.Or(fieldset).Locator(Selectors.ClosestFormGroup));
    }

    private static FieldLocator GetFieldByName(LocatorScope scope, string name)
    {
        var control = GetNamedControl(scope, name, LabelledControlSelector);
        var container = GetNamedDateInput(scope, name)
            .Or(GetNamedChoices(scope, name, "govuk-radios"))
            .Or(GetNamedChoices(scope, name, "govuk-checkboxes"));

        return new FieldLocator(control.Or(container).Locator(Selectors.ClosestFormGroup));
    }

    private static TLocator GetFieldByName<TLocator>(LocatorScope scope, string name)
        where TLocator : FieldLocator
    {
        ArgumentNullException.ThrowIfNull(name);

        var type = typeof(TLocator);

        FieldLocator field =
            type == typeof(FieldLocator) ? GetFieldByName(scope, name)
            : type == typeof(TextInputLocator) ? new TextInputLocator(GetNamedControl(scope, name, TextInputSelector))
            : type == typeof(TextareaLocator) ? new TextareaLocator(GetNamedControl(scope, name, TextareaSelector))
            : type == typeof(CharacterCountLocator) ? new CharacterCountLocator(GetNamedControl(scope, name, CharacterCountSelector))
            : type == typeof(SelectLocator) ? new SelectLocator(GetNamedControl(scope, name, SelectSelector))
            : type == typeof(PasswordInputLocator) ? new PasswordInputLocator(GetNamedControl(scope, name, PasswordInputSelector))
            : type == typeof(FileUploadLocator) ? new FileUploadLocator(GetNamedControl(scope, name, FileUploadSelector))
            : type == typeof(DateInputLocator) ? new DateInputLocator(GetNamedDateInput(scope, name))
            : type == typeof(RadiosLocator) ? new RadiosLocator(GetNamedChoices(scope, name, "govuk-radios"))
            : type == typeof(CheckboxesLocator) ? new CheckboxesLocator(GetNamedChoices(scope, name, "govuk-checkboxes"))
            : throw new ArgumentException($"Fields can't be located as {type.Name}.", nameof(TLocator));

        return (TLocator)field;
    }

    private static ILocator GetNamedControl(LocatorScope scope, string name, string selector)
    {
        ArgumentNullException.ThrowIfNull(name);

        return scope.Locator(selector).And(scope.Locator($"[name={Selectors.AttributeValue(name)}]"));
    }

    // A date input is named by the prefix of its inputs' names, e.g. DateOfBirth for DateOfBirth.Day, or by an input's
    // own name.
    private static ILocator GetNamedDateInput(LocatorScope scope, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        var names = new[] { name }
            .Concat(_datePartNames.SelectMany(part => new[] { $"{name}.{part}", $"{name}-{part}" }))
            .Select(n => $"[name={Selectors.AttributeValue(n)} i]");

        return scope.Locator(".govuk-date-input").Filter(new()
        {
            Has = scope.Page.Locator($".govuk-date-input__input:is({string.Join(", ", names)})")
        });
    }

    // Radios and checkboxes are named by their items' inputs, which may have names of their own. They may not be in a
    // fieldset, so they're found by their own container.
    private static ILocator GetNamedChoices(LocatorScope scope, string name, string block)
    {
        ArgumentNullException.ThrowIfNull(name);

        return scope.Locator($".{block}").Filter(new()
        {
            Has = scope.Page.Locator($":scope > .{block}__item > .{block}__input[name={Selectors.AttributeValue(name)}]")
        });
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
