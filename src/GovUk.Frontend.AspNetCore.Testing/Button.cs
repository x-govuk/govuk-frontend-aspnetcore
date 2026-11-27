using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A button component: a <c>&lt;button&gt;</c>, an <c>&lt;input&gt;</c> or a link styled as a button.
/// </summary>
/// <remarks>
/// To submit a form with a button, use <see cref="Form.GetButtonByText"/> and <see cref="FormButton.CreateSubmission"/>.
/// </remarks>
public sealed class Button
{
    internal const string Selector = ".govuk-button";

    internal static readonly Lookup.Subject Subject = new("button", "buttons", "texts");

    private const string VariantClassPrefix = "govuk-button--";
    private const string StartVariant = "start";

    internal Button(IElement element)
    {
        Element = element;
        Text = element is IHtmlInputElement input ? input.Value : TextExtraction.GetText(element);
        Href = element is IHtmlAnchorElement ? element.GetAttribute("href") : null;
        Variant = element.ClassList
            .Where(c => c.StartsWith(VariantClassPrefix, StringComparison.Ordinal))
            .Select(c => c[VariantClassPrefix.Length..])
            .FirstOrDefault(v => v != StartVariant);
        IsStartButton = element.ClassList.Contains(VariantClassPrefix + StartVariant);
        IsDisabled = element.HasAttribute("disabled") || element.GetAttribute("aria-disabled") == "true";
    }

    /// <summary>
    /// Gets the button's element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the button's text, e.g. <c>Continue</c>; for an <c>&lt;input&gt;</c>, its <c>value</c>.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the <c>href</c> of a link styled as a button, or <see langword="null"/> if the button isn't a link.
    /// </summary>
    public string? Href { get; }

    /// <summary>
    /// Gets the button's variant from its <c>govuk-button--{variant}</c> class, e.g. <c>secondary</c>,
    /// <c>warning</c> or <c>inverse</c>, or <see langword="null"/> for the default button.
    /// </summary>
    /// <remarks>
    /// A start button is given by <see cref="IsStartButton"/> instead.
    /// </remarks>
    public string? Variant { get; }

    /// <summary>
    /// Gets whether the button is a start button.
    /// </summary>
    public bool IsStartButton { get; }

    /// <summary>
    /// Gets whether the button is disabled, with the <c>disabled</c> attribute or <c>aria-disabled="true"</c>.
    /// </summary>
    public bool IsDisabled { get; }

    /// <inheritdoc/>
    public override string ToString() => Variant is null ? $"Button: {Text}" : $"Button: {Text} ({Variant})";
}
