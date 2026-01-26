namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateFieldsetAsync(FieldsetOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var content = HtmlOrText(options.Html, null);

        var tag = new HtmlTag("fieldset", attrs => attrs
            .WithClasses("govuk-fieldset", options.Classes)
            .With("role", options.Role)
            .With("aria-describedby", options.DescribedBy)
            .With(options.Attributes));

        if (options.Legend?.Html is not null || options.Legend?.Text is not null)
        {
            var legendContent = HtmlOrText(options.Legend.Html, options.Legend.Text);

            var legend = new HtmlTag("legend", attrs => attrs
                .WithClasses("govuk-fieldset__legend", options.Legend.Classes)
                .With(options.Legend.Attributes));

            if (options.Legend.IsPageHeading == true)
            {
                var h1 = new HtmlTag("h1", attrs => attrs
                    .WithClasses("govuk-fieldset__heading"))
                {
                    legendContent
                };

                legend.InnerHtml.AppendHtml(h1);
            }
            else
            {
                legend.InnerHtml.AppendHtml(legendContent);
            }

            tag.InnerHtml.AppendHtml(legend);
        }

        tag.InnerHtml.AppendHtml(content);

        return GenerateFromHtmlTagAsync(tag);
    }
}
