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
        public FormField GetField(string label) => FieldLookup.Find<FormField>(node, label, FieldKind.Field, required: true)!;

        /// <summary>
        /// Gets the field of any kind whose label or legend is <paramref name="label"/>, or <see langword="null"/> if
        /// there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such field.</exception>
        public FormField? FindField(string label) => FieldLookup.Find<FormField>(node, label, FieldKind.Field, required: false);

        /// <summary>
        /// Gets the text input labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such text input, or there's more than one.</exception>
        public TextInput GetTextInput(string label) =>
            FieldLookup.Find<TextInput>(node, label, FieldKind.TextInput, required: true)!;

        /// <summary>
        /// Gets the text input labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such text input.</exception>
        public TextInput? FindTextInput(string label) =>
            FieldLookup.Find<TextInput>(node, label, FieldKind.TextInput, required: false);

        /// <summary>
        /// Gets the textarea labelled <paramref name="label"/>, including a character count's.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such textarea, or there's more than one.</exception>
        public Textarea GetTextarea(string label) =>
            FieldLookup.Find<Textarea>(node, label, FieldKind.Textarea, required: true)!;

        /// <summary>
        /// Gets the textarea labelled <paramref name="label"/>, including a character count's, or
        /// <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such textarea.</exception>
        public Textarea? FindTextarea(string label) =>
            FieldLookup.Find<Textarea>(node, label, FieldKind.Textarea, required: false);

        /// <summary>
        /// Gets the character count labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such character count, or there's more than one.</exception>
        public CharacterCount GetCharacterCount(string label) =>
            FieldLookup.Find<CharacterCount>(node, label, FieldKind.CharacterCount, required: true)!;

        /// <summary>
        /// Gets the character count labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such character count.</exception>
        public CharacterCount? FindCharacterCount(string label) =>
            FieldLookup.Find<CharacterCount>(node, label, FieldKind.CharacterCount, required: false);

        /// <summary>
        /// Gets the select labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such select, or there's more than one.</exception>
        public Select GetSelect(string label) => FieldLookup.Find<Select>(node, label, FieldKind.Select, required: true)!;

        /// <summary>
        /// Gets the select labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such select.</exception>
        public Select? FindSelect(string label) => FieldLookup.Find<Select>(node, label, FieldKind.Select, required: false);

        /// <summary>
        /// Gets the password input labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such password input, or there's more than one.</exception>
        public PasswordInput GetPasswordInput(string label) =>
            FieldLookup.Find<PasswordInput>(node, label, FieldKind.PasswordInput, required: true)!;

        /// <summary>
        /// Gets the password input labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such password input.</exception>
        public PasswordInput? FindPasswordInput(string label) =>
            FieldLookup.Find<PasswordInput>(node, label, FieldKind.PasswordInput, required: false);

        /// <summary>
        /// Gets the file upload labelled <paramref name="label"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such file upload, or there's more than one.</exception>
        public FileUpload GetFileUpload(string label) =>
            FieldLookup.Find<FileUpload>(node, label, FieldKind.FileUpload, required: true)!;

        /// <summary>
        /// Gets the file upload labelled <paramref name="label"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such file upload.</exception>
        public FileUpload? FindFileUpload(string label) =>
            FieldLookup.Find<FileUpload>(node, label, FieldKind.FileUpload, required: false);

        /// <summary>
        /// Gets the date input whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such date input, or there's more than one.</exception>
        public DateInput GetDateInput(string legend) =>
            FieldLookup.Find<DateInput>(node, legend, FieldKind.DateInput, required: true)!;

        /// <summary>
        /// Gets the date input whose legend is <paramref name="legend"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such date input.</exception>
        public DateInput? FindDateInput(string legend) =>
            FieldLookup.Find<DateInput>(node, legend, FieldKind.DateInput, required: false);

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There are no such radios, or there's more than one set.</exception>
        public Radios GetRadios(string legend) => FieldLookup.Find<Radios>(node, legend, FieldKind.Radios, required: true)!;

        /// <summary>
        /// Gets the radios whose legend is <paramref name="legend"/>, or <see langword="null"/> if there aren't any.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such set of radios.</exception>
        public Radios? FindRadios(string legend) => FieldLookup.Find<Radios>(node, legend, FieldKind.Radios, required: false);

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There are no such checkboxes, or there's more than one set.</exception>
        public Checkboxes GetCheckboxes(string legend) =>
            FieldLookup.Find<Checkboxes>(node, legend, FieldKind.Checkboxes, required: true)!;

        /// <summary>
        /// Gets the checkboxes whose legend is <paramref name="legend"/>, or <see langword="null"/> if there aren't any.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such set of checkboxes.</exception>
        public Checkboxes? FindCheckboxes(string legend) =>
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

    public static T? Find<T>(IParentNode node, string label, FieldKind kind, bool required)
        where T : FormField
    {
        ArgumentNullException.ThrowIfNull(label);

        var fields = All(node);
        var matches = fields.OfType<T>().Where(f => f.Label == label).ToArray();

        if (matches.Length == 1)
        {
            return matches[0];
        }

        var expected = $"{kind.Expected} labelled {LookupFailure.Quote(label)}";

        if (matches.Length == 0)
        {
            if (!required)
            {
                return null;
            }

            throw new ComponentLookupException(new LookupFailure(
                $"{Lookup.Capitalize(kind.Name)} not found.",
                expected,
                DescribeFields(fields),
                LookupFailure.FormatHtml((INode)node)));
        }

        throw new ComponentLookupException(new LookupFailure(
            $"More than one {kind.Countable} found.",
            expected,
            $"{matches.Length} {kind.CountablePlural} labelled {LookupFailure.Quote(label)}",
            LookupFailure.FormatHtml((INode)node)));
    }

    /// <summary>
    /// Describes the fields that were found, e.g. <c>fields labelled "Name" (text input), "Colour" (radios)</c>.
    /// </summary>
    private static string DescribeFields(IReadOnlyList<FormField> fields) =>
        fields.Count == 0
            ? "no fields"
            : "fields labelled " + string.Join(", ", fields.Select(f =>
                f.Label is null ? $"unlabelled {f.Kind}" : $"{LookupFailure.Quote(f.Label)} ({f.Kind})"));

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
internal sealed record FieldKind(string Name, string Countable, string CountablePlural, string Expected)
{
    public static readonly FieldKind Field = new("field", "fields");
    public static readonly FieldKind TextInput = new("text input", "text inputs");
    public static readonly FieldKind Textarea = new("textarea", "textareas");
    public static readonly FieldKind CharacterCount = new("character count", "character counts");
    public static readonly FieldKind Select = new("select", "selects");
    public static readonly FieldKind PasswordInput = new("password input", "password inputs");
    public static readonly FieldKind FileUpload = new("file upload", "file uploads");
    public static readonly FieldKind DateInput = new("date input", "date inputs");
    public static readonly FieldKind Radios = new("radios", "set of radios", "sets of radios", "radios");
    public static readonly FieldKind Checkboxes = new("checkboxes", "set of checkboxes", "sets of checkboxes", "checkboxes");

    private FieldKind(string name, string plural)
        : this(name, name, plural, $"a {name}")
    {
    }
}
