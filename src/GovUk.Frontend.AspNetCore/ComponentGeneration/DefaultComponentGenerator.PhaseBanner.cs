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

        if (options.Tag != null)
        {
            var tagOptions = options.Tag with
            {
                Classes = new TemplateString("govuk-phase-banner__content__tag").AppendCssClasses(options.Tag.Classes)
            };

            var tagComponent = await GenerateTagAsync(tagOptions);
            var tagHtml = tagComponent.GetContent();
            contentTag.InnerHtml.AppendHtml(tagHtml);
        }
        else
        {
            var emptyTagOptions = new TagOptions
            {
                Classes = "govuk-phase-banner__content__tag"
            };
            var tagComponent = await GenerateTagAsync(emptyTagOptions);
            var tagHtml = tagComponent.GetContent();
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
