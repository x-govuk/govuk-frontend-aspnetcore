using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A textarea component, or a <see cref="CharacterCount"/>.
/// </summary>
public class Textarea : FormField
{
    private protected Textarea(IHtmlTextAreaElement textarea, IElement formGroup)
        : base(formGroup, GetLabel(formGroup, textarea), formGroup)
    {
        TextareaElement = textarea;
    }

    internal static Textarea Create(IHtmlTextAreaElement textarea, IElement formGroup) =>
        formGroup.ClassList.Contains(CharacterCount.FormGroupClass)
            ? new CharacterCount(textarea, formGroup)
            : new Textarea(textarea, formGroup);

    /// <summary>
    /// Gets the <c>&lt;textarea&gt;</c> element.
    /// </summary>
    public IHtmlTextAreaElement TextareaElement { get; }

    /// <summary>
    /// Gets the textarea's <c>name</c>.
    /// </summary>
    public override string? Name => TextareaElement.GetAttribute("name");

    /// <summary>
    /// Gets the textarea's current value.
    /// </summary>
    public string Value => TextareaElement.Value;

    internal override string Kind => "textarea";
}

/// <summary>
/// A character count component: a textarea with a limit on its length.
/// </summary>
public sealed class CharacterCount : Textarea
{
    internal const string FormGroupClass = "govuk-character-count";

    internal CharacterCount(IHtmlTextAreaElement textarea, IElement formGroup)
        : base(textarea, formGroup)
    {
        MaxLength = GetInt(formGroup, "data-maxlength");
        MaxWords = GetInt(formGroup, "data-maxwords");
        Threshold = GetInt(formGroup, "data-threshold");
    }

    /// <summary>
    /// Gets the maximum number of characters, or <see langword="null"/> if the limit is in words or set by JavaScript.
    /// </summary>
    public int? MaxLength { get; }

    /// <summary>
    /// Gets the maximum number of words, or <see langword="null"/> if the limit is in characters or set by JavaScript.
    /// </summary>
    public int? MaxWords { get; }

    /// <summary>
    /// Gets the percentage of the limit at which the count message is shown, or <see langword="null"/> if it's always
    /// shown.
    /// </summary>
    public int? Threshold { get; }

    internal override string Kind => "character count";

    private static int? GetInt(IElement element, string attributeName) =>
        int.TryParse(element.GetAttribute(attributeName), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
}
