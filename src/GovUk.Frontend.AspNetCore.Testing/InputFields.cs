using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A text input component.
/// </summary>
public sealed class TextInput : FormField
{
    internal TextInput(IHtmlInputElement input, IElement formGroup)
        : base(formGroup, GetLabel(formGroup, input), formGroup)
    {
        Input = input;

        var wrapper = input.ParentElement is { } parent && parent.ClassList.Contains("govuk-input__wrapper") ? parent : null;
        Prefix = TextExtraction.GetTextOrNull(Lookup.Child(wrapper, "govuk-input__prefix"));
        Suffix = TextExtraction.GetTextOrNull(Lookup.Child(wrapper, "govuk-input__suffix"));
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c> element.
    /// </summary>
    public IHtmlInputElement Input { get; }

    /// <summary>
    /// Gets the input's <c>name</c>.
    /// </summary>
    public string? Name => Input.GetAttribute("name");

    /// <summary>
    /// Gets the input's <c>type</c>, e.g. <c>text</c> or <c>email</c>.
    /// </summary>
    public string Type => Input.GetAttribute("type") ?? "text";

    /// <summary>
    /// Gets the input's current value.
    /// </summary>
    public string Value => Input.Value;

    /// <summary>
    /// Gets the text of the prefix shown before the input, e.g. <c>£</c>, or <see langword="null"/> if there isn't one.
    /// </summary>
    public string? Prefix { get; }

    /// <summary>
    /// Gets the text of the suffix shown after the input, e.g. <c>per item</c>, or <see langword="null"/> if there
    /// isn't one.
    /// </summary>
    public string? Suffix { get; }

    internal override string Kind => "text input";
}

/// <summary>
/// A password input component.
/// </summary>
public sealed class PasswordInput : FormField
{
    internal PasswordInput(IHtmlInputElement input, IElement formGroup)
        : base(formGroup, GetLabel(formGroup, input), formGroup)
    {
        Input = input;
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c> element.
    /// </summary>
    public IHtmlInputElement Input { get; }

    /// <summary>
    /// Gets the input's <c>name</c>.
    /// </summary>
    public string? Name => Input.GetAttribute("name");

    /// <summary>
    /// Gets the input's current value.
    /// </summary>
    public string Value => Input.Value;

    internal override string Kind => "password input";
}

/// <summary>
/// A file upload component.
/// </summary>
public sealed class FileUpload : FormField
{
    internal FileUpload(IHtmlInputElement input, IElement formGroup)
        : base(formGroup, GetLabel(formGroup, input), formGroup)
    {
        Input = input;
    }

    /// <summary>
    /// Gets the <c>&lt;input type="file"&gt;</c> element.
    /// </summary>
    public IHtmlInputElement Input { get; }

    /// <summary>
    /// Gets the input's <c>name</c>.
    /// </summary>
    public string? Name => Input.GetAttribute("name");

    /// <summary>
    /// Gets whether more than one file can be chosen.
    /// </summary>
    public bool Multiple => Input.HasAttribute("multiple");

    /// <summary>
    /// Gets the input's <c>accept</c> attribute, e.g. <c>.pdf,.jpg</c>, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Accept => Input.GetAttribute("accept");

    internal override string Kind => "file upload";
}
