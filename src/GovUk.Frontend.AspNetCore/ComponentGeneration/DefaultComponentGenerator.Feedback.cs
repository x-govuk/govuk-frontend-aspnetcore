namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateFeedbackAsync(FeedbackOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var headingLevel = options.HeadingLevel ?? 2;

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-feedback", "govuk-width-container", options.Classes)
            .With(options.Attributes));

        var rowTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-row"));
        var columnTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-column-two-thirds"));

        var headingTag = new HtmlTag($"h{headingLevel}", attrs => attrs
            .WithClasses("govuk-feedback__title")
            .With(options.TitleAttributes));
        headingTag.InnerHtml.AppendHtml(HtmlOrText(options.TitleHtml, options.TitleText));
        columnTag.InnerHtml.AppendHtml(headingTag);

        if (!options.Html.IsEmpty() || !options.Text.IsEmpty())
        {
            var bodyTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-feedback__body")
                .With(options.BodyAttributes));

            if (!options.Html.IsEmpty())
            {
                bodyTag.InnerHtml.AppendHtml(options.Html);
            }
            else
            {
                var paragraphTag = new HtmlTag("p", attrs => attrs.WithClasses("govuk-body"));
                paragraphTag.InnerHtml.Append(options.Text!);
                bodyTag.InnerHtml.AppendHtml(paragraphTag);
            }

            columnTag.InnerHtml.AppendHtml(bodyTag);
        }

        rowTag.InnerHtml.AppendHtml(columnTag);
        outerTag.InnerHtml.AppendHtml(rowTag);

        return GenerateFromHtmlTagAsync(outerTag);
    }
}
