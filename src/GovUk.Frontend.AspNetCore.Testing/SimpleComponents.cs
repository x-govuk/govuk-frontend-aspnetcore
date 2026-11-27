using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A tag component.
/// </summary>
public sealed class Tag
{
    internal const string Selector = ".govuk-tag";

    private const string ColourClassPrefix = "govuk-tag--";

    internal Tag(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element);
        Colour = element.ClassList
            .FirstOrDefault(c => c.StartsWith(ColourClassPrefix, StringComparison.Ordinal))?[ColourClassPrefix.Length..];
    }

    /// <summary>
    /// Gets the tag's element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the tag's text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the tag's colour from its <c>govuk-tag--{colour}</c> class, e.g. <c>blue</c>, or <see langword="null"/>
    /// if it has the default colour.
    /// </summary>
    public string? Colour { get; }

    /// <inheritdoc/>
    public override string ToString() => Colour is null ? Text : $"{Text} ({Colour})";
}

/// <summary>
/// A panel component.
/// </summary>
public sealed class Panel
{
    internal const string Selector = ".govuk-panel";

    internal Panel(IElement element)
    {
        Element = element;
        Title = TextExtraction.GetText(Lookup.Child(element, "govuk-panel__title"));
        Body = TextExtraction.GetTextOrNull(Lookup.Child(element, "govuk-panel__body"));
    }

    /// <summary>
    /// Gets the panel's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the panel's title, e.g. <c>Application complete</c>.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the text of the panel's body, or <see langword="null"/> if it doesn't have one.
    /// </summary>
    public string? Body { get; }

    /// <inheritdoc/>
    public override string ToString() => Body is null ? $"Panel: {Title}" : $"Panel: {Title}: {Body}";
}

/// <summary>
/// A back link component.
/// </summary>
public sealed class BackLink
{
    internal const string Selector = "a.govuk-back-link";

    internal BackLink(IElement element)
    {
        Element = element;
        Text = TextExtraction.GetText(element);
        Href = element.GetAttribute("href");
    }

    /// <summary>
    /// Gets the link's <c>&lt;a&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the link's text, e.g. <c>Back</c>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the link's <c>href</c>.
    /// </summary>
    public string? Href { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Back link: {Href}";
}

/// <summary>
/// A hidden <c>&lt;input&gt;</c>.
/// </summary>
public sealed class HiddenInput
{
    internal const string Selector = "input[type=hidden]";

    internal static readonly Lookup.Subject Subject = new("hidden input", "hidden inputs", "names");

    internal HiddenInput(IElement element)
    {
        Element = element;
        Name = element.GetAttribute("name") ?? "";
        Value = element.GetAttribute("value") ?? "";
    }

    /// <summary>
    /// Gets the <c>&lt;input&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the input's name, or an empty string if it doesn't have one.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the input's value, or an empty string if it doesn't have one.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}={Value}";
}

/// <summary>
/// A warning text component.
/// </summary>
public sealed class WarningText
{
    internal const string Selector = ".govuk-warning-text";

    internal WarningText(IElement element)
    {
        Element = element;

        var text = Lookup.Child(element, "govuk-warning-text__text");
        Text = TextExtraction.GetText(text);
        IconFallbackText = text is null ? null : TextExtraction.GetVisuallyHiddenText(text);
    }

    /// <summary>
    /// Gets the component's root element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the warning, e.g. <c>You can be fined up to £5,000 if you do not register.</c>, without the icon or the
    /// visually hidden text read out in its place.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the visually hidden text read out in place of the icon, e.g. <c>Warning</c>, or <see langword="null"/> if
    /// there isn't any.
    /// </summary>
    public string? IconFallbackText { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Warning text: {Text}";
}

/// <summary>
/// An inset text component.
/// </summary>
public sealed class InsetText
{
    internal const string Selector = ".govuk-inset-text";

    internal InsetText(IElement element)
    {
        Element = element;
        TextLines = TextExtraction.GetLines(element);
        Text = string.Join(" ", TextLines);
    }

    /// <summary>
    /// Gets the component's element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the component's text as a single line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the lines of the component's text, as split by line breaks and block-level elements.
    /// </summary>
    public IReadOnlyList<string> TextLines { get; }

    /// <inheritdoc/>
    public override string ToString() => $"Inset text: {Text}";
}
