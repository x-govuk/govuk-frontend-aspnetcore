using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A form field component: a text input, textarea, character count, select, password input, file upload, date
/// input, radios or checkboxes.
/// </summary>
/// <remarks>
/// A field's <see cref="Label"/> is the text of its <c>&lt;label&gt;</c> or, for date inputs, radios and checkboxes,
/// the <c>&lt;legend&gt;</c> of its fieldset.
/// </remarks>
public abstract class FormField
{
    private const string FormGroupClass = "govuk-form-group";

    private protected FormField(IElement element, IElement? labelElement, IElement container)
    {
        Element = element;
        LabelElement = labelElement;
        Label = TextExtraction.GetTextOrNull(labelElement);

        // The hint and error message are direct children of the fieldset, or of the form group when there isn't one,
        // so hints and errors of fields in a conditional reveal aren't picked up.
        HintElement = container.Children.FirstOrDefault(c =>
            c.ClassList.Contains("govuk-hint") && !c.ClassList.Contains("govuk-character-count__message"));
        Hint = TextExtraction.GetTextOrNull(HintElement);

        ErrorMessageElement = container.Children.FirstOrDefault(c => c.ClassList.Contains("govuk-error-message"));
        ErrorMessage = TextExtraction.GetTextOrNull(ErrorMessageElement);
    }

    /// <summary>
    /// Gets the field's form group element, or the field's own element if it isn't in a form group.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the field's <c>&lt;label&gt;</c> or <c>&lt;legend&gt;</c> element, or <see langword="null"/> if it doesn't
    /// have one.
    /// </summary>
    public IElement? LabelElement { get; }

    /// <summary>
    /// Gets the text of the field's label or legend, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    /// Gets the name the field is submitted with, or <see langword="null"/> if its inputs don't share one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the <c>name</c> of the field's input, textarea or select. For radios and checkboxes it's the
    /// <c>name</c> their inputs share.
    /// </para>
    /// <para>
    /// For a date input it's the prefix of its inputs' names, e.g. <c>DateOfBirth</c> for <c>DateOfBirth.Day</c>,
    /// <c>DateOfBirth.Month</c> and <c>DateOfBirth.Year</c>, which is the name model binding binds the date to.
    /// </para>
    /// <para>
    /// Radios, checkboxes and date inputs whose inputs have names of their own, e.g. checkboxes named
    /// <c>EmailUpdates</c> and <c>SmsUpdates</c>, don't have a single name; use <see cref="Names"/> for those.
    /// </para>
    /// </remarks>
    public abstract string? Name { get; }

    /// <summary>
    /// Gets every name the field can be found by with
    /// <see cref="ComponentQueryExtensions.GetFieldByName"/>: its <see cref="Name"/>, if it has one, then the
    /// names of its inputs, without duplicates.
    /// </summary>
    /// <remarks>
    /// For a text input, textarea, character count, select, password input or file upload this is just its
    /// <see cref="Name"/>. For checkboxes named <c>EmailUpdates</c> and <c>SmsUpdates</c> it's both names, and for
    /// a date input it's e.g. <c>DateOfBirth</c>, <c>DateOfBirth.Day</c>, <c>DateOfBirth.Month</c> and
    /// <c>DateOfBirth.Year</c>. Fields inside conditional reveals are fields of their own, so their names aren't
    /// included.
    /// </remarks>
    public virtual IReadOnlyList<string> Names => Name is { Length: > 0 } name ? [name] : [];

    /// <summary>
    /// Gets the field's hint element, or <see langword="null"/> if it doesn't have a hint.
    /// </summary>
    public IElement? HintElement { get; }

    /// <summary>
    /// Gets the text of the field's hint, or <see langword="null"/> if it doesn't have a hint.
    /// </summary>
    public string? Hint { get; }

    /// <summary>
    /// Gets the field's error message element, or <see langword="null"/> if it doesn't have an error.
    /// </summary>
    public IElement? ErrorMessageElement { get; }

    /// <summary>
    /// Gets the text of the field's error message, or <see langword="null"/> if it doesn't have an error.
    /// </summary>
    /// <remarks>
    /// The visually hidden prefix that tells screen reader users it's an error (<c>Error:</c> by default) is left out.
    /// </remarks>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets a description of the kind of field, e.g. <c>text input</c>.
    /// </summary>
    internal abstract string Kind { get; }

    /// <inheritdoc/>
    public override string ToString() => Label is null ? $"Unlabelled {Kind}" : $"{Lookup.Capitalize(Kind)}: {Label}";

    internal static IElement GetFormGroup(IElement control) =>
        control.Closest($".{FormGroupClass}") ?? control.ParentElement ?? control;

    /// <summary>
    /// Gets the <c>&lt;label&gt;</c> for <paramref name="control"/>, looking in its form group.
    /// </summary>
    internal static IElement? GetLabel(IElement formGroup, IElement control)
    {
        var id = control.Id;

        return string.IsNullOrEmpty(id)
            ? null
            : formGroup.QuerySelectorAll("label").FirstOrDefault(l => l.GetAttribute("for") == id);
    }

    /// <summary>
    /// Gets the fieldset that groups a component such as radios, if it's in one, and the container of its hint and
    /// error message.
    /// </summary>
    internal static (IElement? Fieldset, IElement Container) GetFieldset(IElement formGroup, IElement componentRoot)
    {
        var parent = componentRoot.ParentElement;

        return parent is not null && parent.LocalName == "fieldset" && parent.ParentElement == formGroup
            ? (parent, parent)
            : (null, formGroup);
    }

    /// <summary>
    /// Gets the name that every one of <paramref name="names"/> is, or <see langword="null"/> if there are none or
    /// they differ.
    /// </summary>
    internal static string? GetSharedName(IEnumerable<string?> names)
    {
        string? shared = null;

        foreach (var name in names)
        {
            if (string.IsNullOrEmpty(name) || (shared is not null && shared != name))
            {
                return null;
            }

            shared = name;
        }

        return shared;
    }

    /// <summary>
    /// Gets <paramref name="name"/>, if there is one, then <paramref name="inputNames"/>, leaving out empty names and
    /// duplicates.
    /// </summary>
    private protected static IReadOnlyList<string> GetNames(string? name, IEnumerable<string?> inputNames) =>
        inputNames.Prepend(name).OfType<string>().Where(n => n.Length > 0).Distinct(StringComparer.Ordinal).ToArray();

    internal static IElement? GetLegend(IElement? fieldset) => fieldset?.Children.FirstOrDefault(c => c.LocalName == "legend");
}
