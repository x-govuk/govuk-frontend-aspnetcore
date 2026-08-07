using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator : IComponentGenerator
{
    private ValueTask<GovUkComponent> GenerateFromHtmlTagAsync(HtmlTag tag) =>
        ValueTask.FromResult<GovUkComponent>(new HtmlTagGovUkComponent(tag));

    // The parameter types carry the distinction: Html is markup and is written as-is, Text is plain
    // and is encoded. Neither can be mistaken for the other, so nothing here has to decide.
    private IHtmlContent HtmlOrText(IHtmlContent? html, string? text, string? fallback = null)
    {
        if (!html.IsEmpty())
        {
            return html;
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            return new TemplateString(text);
        }

        return new TemplateString(fallback);
    }

    protected sealed class EmptyComponent : GovUkComponent
    {
        private readonly IHtmlContent _content = new HtmlString(string.Empty);

        private EmptyComponent() { }

        public static EmptyComponent Instance { get; } = new();

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            output.SuppressOutput();
        }

        public override IHtmlContent GetContent() => _content;
    }

    // Casing the encoded form would upper-case the '&' of an entity rather than the character it
    // stands for, so this works on the text.
    private static string Capitalize(TemplateString? input)
    {
        if (input.IsEmpty())
        {
            return string.Empty;
        }

        var text = input.ToText();

#pragma warning disable CA1308
        return char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant();
#pragma warning restore CA1308
    }

    /// <summary>
    /// Replaces every occurrence of <paramref name="placeholder"/> in <paramref name="value"/>.
    /// </summary>
    /// <remarks>
    /// Substituting into the text rather than the rendered HTML keeps the result's encoding, so it's
    /// still written with the caller's encoder.
    /// </remarks>
    private static TemplateString ReplacePlaceholder(TemplateString value, string placeholder, string replacement)
    {
        if (value.TryGetText(out var text))
        {
            return new TemplateString(text.Replace(placeholder, replacement, StringComparison.Ordinal));
        }

        return TemplateString.FromEncoded(value.Render().Replace(placeholder, replacement, StringComparison.Ordinal));
    }

    private class HtmlTagGovUkComponent : GovUkComponent
    {
        public HtmlTagGovUkComponent(HtmlTag tag)
        {
            ArgumentNullException.ThrowIfNull(tag);

            Tag = tag;
        }

        public HtmlTag Tag { get; }

        public override IHtmlContent GetContent() => Tag;

        public override void ApplyToTagHelper(TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            var tagMode = Tag.TagRenderMode switch
            {
                TagRenderMode.StartTag => TagMode.StartTagOnly,
                TagRenderMode.SelfClosing => TagMode.SelfClosing,
                TagRenderMode.Normal => TagMode.StartTagAndEndTag,
                _ => throw new InvalidOperationException($"Cannot apply an HtmlTag with TagRenderMode '{Tag.TagRenderMode}' to a tag helper.")
            };

            output.TagName = Tag.TagName;
            output.TagMode = tagMode;

            output.Attributes.Clear();

            foreach (var attribute in Tag.Attributes.ToTagHelperAttributes())
            {
                output.Attributes.Add(attribute);
            }

            output.Content.AppendHtml(Tag.InnerHtml);
        }
    }
}
