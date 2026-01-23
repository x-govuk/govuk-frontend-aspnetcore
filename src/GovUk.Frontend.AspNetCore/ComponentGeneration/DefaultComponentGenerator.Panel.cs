namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GeneratePanelAsync(PanelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var headingLevel = options.HeadingLevel ?? 1;
        var titleContent = HtmlOrText(options.TitleHtml, options.TitleText);

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-panel", "govuk-panel--confirmation", options.Classes)
            .With(options.Attributes));

        var headingTag = new HtmlTag($"h{headingLevel}", attrs => attrs
            .WithClasses("govuk-panel__title")
            .With(options.TitleAttributes));
        headingTag.InnerHtml.AppendHtml(titleContent);
        outerTag.InnerHtml.AppendHtml(headingTag);

        if (!options.Html.IsEmpty() || !options.Text.IsEmpty())
        {
            var bodyContent = HtmlOrText(options.Html, options.Text);
            var bodyTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-panel__body")
                .With(options.BodyAttributes));
            bodyTag.InnerHtml.AppendHtml(bodyContent);
            outerTag.InnerHtml.AppendHtml(bodyTag);
        }

        return GenerateFromHtmlTagAsync(outerTag);
    }
}
