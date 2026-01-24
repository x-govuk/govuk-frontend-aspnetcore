using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator : IComponentGenerator
{
#pragma warning disable CA2012 // ValueTasks should not be composed - but this is a completed ValueTask wrapping an immutable singleton
    protected ValueTask<GovUkComponent> EmptyComponentTask { get; } = ValueTask.FromResult<GovUkComponent>(EmptyComponent.Instance);
#pragma warning restore CA2012

    private ValueTask<GovUkComponent> GenerateFromHtmlTagAsync(HtmlTag tag) =>
        ValueTask.FromResult<GovUkComponent>(new HtmlTagGovUkComponent(tag));

    private IHtmlContent HtmlOrText(TemplateString? html, TemplateString? text, string? fallback = null)
    {
        if (html?.IsEmpty() is false)
        {
            return html.GetRawHtml();
        }

        if (text?.IsEmpty() is false)
        {
            return text;
        }

        return new HtmlString(fallback);
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

    private static TemplateString Capitalize(TemplateString? input)
    {
        if (input.IsEmpty())
        {
            return TemplateString.Empty;
        }

        var encodedInput = input.ToHtmlString();

#pragma warning disable CA1308
        return TemplateString.FromEncoded(char.ToUpperInvariant(encodedInput[0]) + encodedInput[1..].ToLowerInvariant());
#pragma warning restore CA1308
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
