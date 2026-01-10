namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateBreadcrumbsAsync(BreadcrumbsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var navTag = new HtmlTag("nav", attrs => attrs
            .WithClasses("govuk-breadcrumbs", options.CollapseOnMobile == true ? "govuk-breadcrumbs--collapse-on-mobile" : null, options.Classes)
            .With("aria-label", options.LabelText ?? "Breadcrumb")
            .With(options.Attributes));

        var olTag = new HtmlTag("ol", attrs => attrs
            .WithClasses("govuk-breadcrumbs__list"));

        if (options.Items is not null)
        {
            foreach (var item in options.Items)
            {
                var liTag = new HtmlTag("li", attrs =>
                {
                    attrs
                        .WithClasses("govuk-breadcrumbs__list-item")
                        .With(item.ItemAttributes);

                    if (item.Href is null)
                    {
                        attrs.With("aria-current", "page");
                    }
                });

                if (item.Href is not null)
                {
                    var aTag = new HtmlTag("a", attrs => attrs
                        .WithClasses("govuk-breadcrumbs__link")
                        .With("href", item.Href)
                        .With(item.Attributes));

                    aTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));

                    liTag.InnerHtml.AppendHtml(aTag);
                }
                else
                {
                    liTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));
                }

                olTag.InnerHtml.AppendHtml(liTag);
            }
        }

        navTag.InnerHtml.AppendHtml(olTag);

        return GenerateFromHtmlTagAsync(navTag);
    }
}
