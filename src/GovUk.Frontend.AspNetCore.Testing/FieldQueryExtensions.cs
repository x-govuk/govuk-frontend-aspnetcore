using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

public static partial class ComponentQueryExtensions
{
    extension(IParentNode node)
    {
        /// <summary>
        /// Gets every form field, in document order, including those inside conditional reveals.
        /// </summary>
        public IReadOnlyList<FormField> Fields => FieldLookup.All(node);

        /// <summary>
        /// Gets the field of any kind whose label or legend is <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such field, or there's more than one.</exception>
        public FormField GetFieldByLabelOrLegend(string label) => FieldLookup.Find<FormField>(node, label, FieldKind.Field, required: true)!;

        /// <summary>
        /// Gets the field of any kind whose label or legend is <paramref name="label"/>, or <see langword="null"/> if
        /// there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such field.</exception>
        public FormField? FindFieldByLabelOrLegend(string label) => FieldLookup.Find<FormField>(node, label, FieldKind.Field, required: false);

        /// <summary>
        /// Gets the field of any kind whose <see cref="FormField.Name"/> is <paramref name="name"/>.
        /// </summary>
        /// <remarks>
        /// A date input's name is the prefix of its inputs' names, e.g. <c>DateOfBirth</c>, and radios and
        /// checkboxes are found by the name their inputs share.
        /// </remarks>
        /// <exception cref="ComponentLookupException">There is no such field, or there's more than one.</exception>
        public FormField GetFieldByName(string name) => FieldLookup.FindByName(node, name, required: true)!;

        /// <summary>
        /// Gets the field of any kind whose <see cref="FormField.Name"/> is <paramref name="name"/>, or
        /// <see langword="null"/> if there isn't one.
        /// </summary>
        /// <remarks>
        /// A date input's name is the prefix of its inputs' names, e.g. <c>DateOfBirth</c>, and radios and
        /// checkboxes are found by the name their inputs share.
        /// </remarks>
        /// <exception cref="ComponentLookupException">There's more than one such field.</exception>
        public FormField? FindFieldByName(string name) => FieldLookup.FindByName(node, name, required: false);

        /// <summary>
        /// Gets the text input labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such text input, or there's more than one.</exception>
        public TextInput GetTextInputByLabel(string label) =>
            FieldLookup.Find<TextInput>(node, label, FieldKind.TextInput, required: true)!;

        /// <summary>
        /// Gets the text input labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such text input.</exception>
        public TextInput? FindTextInputByLabel(string label) =>
            FieldLookup.Find<TextInput>(node, label, FieldKind.TextInput, required: false);

        /// <summary>
        /// Gets the textarea labelled <paramref name="label"/>, including a character count's.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such textarea, or there's more than one.</exception>
        public Textarea GetTextareaByLabel(string label) =>
            FieldLookup.Find<Textarea>(node, label, FieldKind.Textarea, required: true)!;

        /// <summary>
        /// Gets the textarea labelled <paramref name="label"/>, including a character count's, or
        /// <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such textarea.</exception>
        public Textarea? FindTextareaByLabel(string label) =>
            FieldLookup.Find<Textarea>(node, label, FieldKind.Textarea, required: false);

        /// <summary>
        /// Gets the character count labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such character count, or there's more than one.</exception>
        public CharacterCount GetCharacterCountByLabel(string label) =>
            FieldLookup.Find<CharacterCount>(node, label, FieldKind.CharacterCount, required: true)!;

        /// <summary>
        /// Gets the character count labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such character count.</exception>
        public CharacterCount? FindCharacterCountByLabel(string label) =>
            FieldLookup.Find<CharacterCount>(node, label, FieldKind.CharacterCount, required: false);

        /// <summary>
        /// Gets the select labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such select, or there's more than one.</exception>
        public Select GetSelectByLabel(string label) => FieldLookup.Find<Select>(node, label, FieldKind.Select, required: true)!;

        /// <summary>
        /// Gets the select labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such select.</exception>
        public Select? FindSelectByLabel(string label) => FieldLookup.Find<Select>(node, label, FieldKind.Select, required: false);

        /// <summary>
        /// Gets the password input labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such password input, or there's more than one.</exception>
        public PasswordInput GetPasswordInputByLabel(string label) =>
            FieldLookup.Find<PasswordInput>(node, label, FieldKind.PasswordInput, required: true)!;

        /// <summary>
        /// Gets the password input labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such password input.</exception>
        public PasswordInput? FindPasswordInputByLabel(string label) =>
            FieldLookup.Find<PasswordInput>(node, label, FieldKind.PasswordInput, required: false);

        /// <summary>
        /// Gets the file upload labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such file upload, or there's more than one.</exception>
        public FileUpload GetFileUploadByLabel(string label) =>
            FieldLookup.Find<FileUpload>(node, label, FieldKind.FileUpload, required: true)!;

        /// <summary>
        /// Gets the file upload labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such file upload.</exception>
        public FileUpload? FindFileUploadByLabel(string label) =>
            FieldLookup.Find<FileUpload>(node, label, FieldKind.FileUpload, required: false);

        /// <summary>
        /// Gets the date input whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such date input, or there's more than one.</exception>
        public DateInput GetDateInputByLegend(string legend) =>
            FieldLookup.Find<DateInput>(node, legend, FieldKind.DateInput, required: true)!;

        /// <summary>
        /// Gets the date input whose legend is <paramref name="legend"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such date input.</exception>
        public DateInput? FindDateInputByLegend(string legend) =>
            FieldLookup.Find<DateInput>(node, legend, FieldKind.DateInput, required: false);

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There are no such radios, or there's more than one set.</exception>
        public Radios GetRadiosByLegend(string legend) => FieldLookup.Find<Radios>(node, legend, FieldKind.Radios, required: true)!;

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>, or <see langword="null"/> if there aren't any.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such set of radios.</exception>
        public Radios? FindRadiosByLegend(string legend) => FieldLookup.Find<Radios>(node, legend, FieldKind.Radios, required: false);

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There are no such checkboxes, or there's more than one set.</exception>
        public Checkboxes GetCheckboxesByLegend(string legend) =>
            FieldLookup.Find<Checkboxes>(node, legend, FieldKind.Checkboxes, required: true)!;

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>, or <see langword="null"/> if there aren't any.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such set of checkboxes.</exception>
        public Checkboxes? FindCheckboxesByLegend(string legend) =>
            FieldLookup.Find<Checkboxes>(node, legend, FieldKind.Checkboxes, required: false);
    }
}

internal static class FieldLookup
{
    private const string Selector =
        "input.govuk-input, input.govuk-file-upload, textarea.govuk-textarea, select.govuk-select, " +
        $"{DateInput.Selector}, {Radios.Selector}, {Checkboxes.Selector}";

    public static IReadOnlyList<FormField> All(IParentNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        return node.QuerySelectorAll(Selector).Select(Create).OfType<FormField>().ToArray();
    }

    public static T? Find<T>(IParentNode node, string text, FieldKind kind, bool required)
        where T : FormField
    {
        ArgumentNullException.ThrowIfNull(text);

        return Find<T>(node, text, kind, f => f.Label, "labelled", "unlabelled", required);
    }

    public static FormField? FindByName(IParentNode node, string name, bool required)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Find<FormField>(node, name, FieldKind.FieldByName, f => f.Name, "named", "unnamed", required);
    }

    private static T? Find<T>(
        IParentNode node,
        string text,
        FieldKind kind,
        Func<FormField, string?> getText,
        string describedAs,
        string describedAsWithout,
        bool required)
        where T : FormField
    {
        var fields = All(node);
        var matches = fields.OfType<T>().Where(f => getText(f) == text).ToArray();

        if (matches.Length == 1)
        {
            return matches[0];
        }

        var expected = $"{kind.Expected} {kind.MatchedBy} {LookupFailure.Quote(text)}";

        if (matches.Length == 0)
        {
            if (!required)
            {
                return null;
            }

            throw new ComponentLookupException(new LookupFailure(
                $"{Lookup.Capitalize(kind.Name)} not found.",
                expected,
                DescribeFields(fields, getText, describedAs, describedAsWithout),
                LookupFailure.FormatHtml((INode)node)));
        }

        throw new ComponentLookupException(new LookupFailure(
            $"More than one {kind.Countable} found.",
            expected,
            $"{matches.Length} {kind.CountablePlural} {kind.MatchedBy} {LookupFailure.Quote(text)}",
            LookupFailure.FormatHtml((INode)node)));
    }

    /// <summary>
    /// Describes the fields that were found, e.g. <c>fields labelled "Name" (text input), "Colour" (radios)</c>.
    /// </summary>
    private static string DescribeFields(
        IReadOnlyList<FormField> fields,
        Func<FormField, string?> getText,
        string describedAs,
        string describedAsWithout) =>
        fields.Count == 0
            ? "no fields"
            : $"fields {describedAs} " + string.Join(", ", fields.Select(f =>
                getText(f) is { } text ? $"{LookupFailure.Quote(text)} ({f.Kind})" : $"{describedAsWithout} {f.Kind}"));

    private static FormField? Create(IElement element)
    {
        var formGroup = FormField.GetFormGroup(element);

        switch (element)
        {
            case IHtmlInputElement input when input.ClassList.Contains("govuk-file-upload"):
                return new FileUpload(input, formGroup);

            case IHtmlInputElement input when input.ClassList.Contains("govuk-date-input__input"):
                // Part of a date input, which is found by its container.
                return null;

            case IHtmlInputElement input when input.ClassList.Contains("govuk-password-input__input"):
                return new PasswordInput(input, formGroup);

            case IHtmlInputElement input:
                return new TextInput(input, formGroup);

            case IHtmlTextAreaElement textarea:
                return Textarea.Create(textarea, formGroup);

            case IHtmlSelectElement select:
                return new Select(select, formGroup);
        }

        var (fieldset, container) = FormField.GetFieldset(formGroup, element);

        return element.ClassList.Contains("govuk-date-input") ? new DateInput(element, formGroup, fieldset, container)
            : element.ClassList.Contains("govuk-radios") ? new Radios(element, formGroup, fieldset, container)
            : element.ClassList.Contains("govuk-checkboxes") ? new Checkboxes(element, formGroup, fieldset, container)
            : null;
    }
}

/// <summary>
/// Describes a kind of field in failure messages.
/// </summary>
/// <param name="Name">What the field is called, e.g. <c>radios</c>.</param>
/// <param name="Countable">One of them, e.g. <c>set of radios</c>.</param>
/// <param name="CountablePlural">More than one of them, e.g. <c>sets of radios</c>.</param>
/// <param name="Expected">What's looked for, e.g. <c>radios</c> or <c>a text input</c>.</param>
/// <param name="MatchedBy">How the text looked for is matched, e.g. <c>labelled</c> or <c>with the legend</c>.</param>
internal sealed record FieldKind(string Name, string Countable, string CountablePlural, string Expected, string MatchedBy)
{
    private const string Labelled = "labelled";
    private const string WithTheLegend = "with the legend";

    public static FieldKind Field { get; } = new("field", "fields", "with the label or legend");
    public static FieldKind FieldByName { get; } = new("field", "fields", "named");
    public static FieldKind TextInput { get; } = new("text input", "text inputs", Labelled);
    public static FieldKind Textarea { get; } = new("textarea", "textareas", Labelled);
    public static FieldKind CharacterCount { get; } = new("character count", "character counts", Labelled);
    public static FieldKind Select { get; } = new("select", "selects", Labelled);
    public static FieldKind PasswordInput { get; } = new("password input", "password inputs", Labelled);
    public static FieldKind FileUpload { get; } = new("file upload", "file uploads", Labelled);
    public static FieldKind DateInput { get; } = new("date input", "date inputs", WithTheLegend);
    public static FieldKind Radios { get; } = new("radios", "set of radios", "sets of radios", "radios", WithTheLegend);
    public static FieldKind Checkboxes { get; } = new("checkboxes", "set of checkboxes", "sets of checkboxes", "checkboxes", WithTheLegend);

    private FieldKind(string name, string plural, string matchedBy)
        : this(name, name, plural, $"a {name}", matchedBy)
    {
    }
}
