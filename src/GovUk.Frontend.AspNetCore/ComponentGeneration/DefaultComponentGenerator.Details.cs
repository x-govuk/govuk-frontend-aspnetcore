namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateDetailsAsync(DetailsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var detailsTag = new HtmlTag("details", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-details", options.Classes)
            .With("open", options.Open == true ? "" : null)
            .With(options.Attributes));

        var summaryTag = new HtmlTag("summary", attrs => attrs
            .WithClasses("govuk-details__summary")
            .With(options.SummaryAttributes));

        var summaryTextTag = new HtmlTag("span", attrs => attrs
            .WithClasses("govuk-details__summary-text"));

        summaryTextTag.InnerHtml.AppendHtml(HtmlOrText(options.SummaryHtml, options.SummaryText));
        summaryTag.InnerHtml.AppendHtml(summaryTextTag);
        detailsTag.InnerHtml.AppendHtml(summaryTag);

        var textTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-details__text")
            .With(options.TextAttributes));

        textTag.InnerHtml.AppendHtml(HtmlOrText(options.Html, options.Text));
        detailsTag.InnerHtml.AppendHtml(textTag);

        return GenerateFromHtmlTagAsync(detailsTag);
    }
}
