using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GeneratePhaseBannerAsync(PhaseBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var content = HtmlOrText(options.Html, options.Text);

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-phase-banner", options.Classes)
            .With(options.Attributes));

        var contentTag = new HtmlTag("p", attrs => attrs
            .WithClasses("govuk-phase-banner__content"));

        // Generate the tag component using the existing GenerateTagAsync method
        if (options.Tag != null)
        {
            var tagOptions = new TagOptions
            {
                Text = options.Tag.Text,
                Html = options.Tag.Html,
                Classes = options.Tag.Classes != null
                    ? new TemplateString("govuk-phase-banner__content__tag " + options.Tag.Classes)
                    : "govuk-phase-banner__content__tag",
                Attributes = options.Tag.Attributes
            };

            var tagComponent = await GenerateTagAsync(tagOptions);
            var tagHtml = new HtmlString(tagComponent.GetHtml());
            contentTag.InnerHtml.AppendHtml(tagHtml);
        }
        else
        {
            // If no tag options provided, render an empty tag
            var emptyTagOptions = new TagOptions
            {
                Classes = "govuk-phase-banner__content__tag"
            };
            var tagComponent = await GenerateTagAsync(emptyTagOptions);
            var tagHtml = new HtmlString(tagComponent.GetHtml());
            contentTag.InnerHtml.AppendHtml(tagHtml);
        }

        var textSpan = new HtmlTag("span", attrs => attrs
            .WithClasses("govuk-phase-banner__text"));
        textSpan.InnerHtml.AppendHtml(content);

        contentTag.InnerHtml.AppendHtml(textSpan);

        outerTag.InnerHtml.AppendHtml(contentTag);

        return await GenerateFromHtmlTagAsync(outerTag);
    }
}
