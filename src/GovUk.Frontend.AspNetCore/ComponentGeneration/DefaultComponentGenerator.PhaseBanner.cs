using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GeneratePhaseBannerAsync(PhaseBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        HtmlTag GenerateTag(TagOptions? tagOptions)
        {
            var tagContent = tagOptions != null ? HtmlOrText(tagOptions.Html, tagOptions.Text) : HtmlString.Empty;

            var tag = new HtmlTag("strong", attrs => attrs
                .WithClasses("govuk-tag", "govuk-phase-banner__content__tag", tagOptions?.Classes)
                .With(tagOptions?.Attributes));

            tag.InnerHtml.AppendHtml(tagContent);

            return tag;
        }

        var content = HtmlOrText(options.Html, options.Text);

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-phase-banner", options.Classes)
            .With(options.Attributes));

        var contentTag = new HtmlTag("p", attrs => attrs
            .WithClasses("govuk-phase-banner__content"));

        var tagElement = GenerateTag(options.Tag);
        contentTag.InnerHtml.AppendHtml(tagElement);

        var textSpan = new HtmlTag("span", attrs => attrs
            .WithClasses("govuk-phase-banner__text"));
        textSpan.InnerHtml.AppendHtml(content);

        contentTag.InnerHtml.AppendHtml(textSpan);

        outerTag.InnerHtml.AppendHtml(contentTag);

        return GenerateFromHtmlTagAsync(outerTag);
    }
}
