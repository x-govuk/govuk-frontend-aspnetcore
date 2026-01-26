namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    internal const string DefaultErrorSummaryTitleHtml = "There is a problem";

    public virtual ValueTask<GovUkComponent> GenerateErrorSummaryAsync(ErrorSummaryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var titleContent = HtmlOrText(options.TitleHtml, options.TitleText);

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-error-summary", options.Classes)
            .With("data-disable-auto-focus", GetDisableAutoFocusValue(options.DisableAutoFocus))
            .With(options.Attributes)
            .With("data-module", "govuk-error-summary"));

        var alertDiv = new HtmlTag("div", attrs => attrs
            .With("role", "alert"));

        var titleTag = new HtmlTag("h2", attrs => attrs
            .WithClasses("govuk-error-summary__title")
            .With(options.TitleAttributes));
        titleTag.InnerHtml.AppendHtml(titleContent);
        alertDiv.InnerHtml.AppendHtml(titleTag);

        var bodyTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-error-summary__body"));

        if (options.DescriptionHtml?.IsEmpty() == false || options.DescriptionText?.IsEmpty() == false)
        {
            var descriptionContent = HtmlOrText(options.DescriptionHtml, options.DescriptionText);
            var descriptionTag = new HtmlTag("p", attrs => attrs
                .With(options.DescriptionAttributes));
            descriptionTag.InnerHtml.AppendHtml(descriptionContent);
            bodyTag.InnerHtml.AppendHtml(descriptionTag);
        }

        if (options.ErrorList is not null)
        {
            var listTag = new HtmlTag("ul", attrs => attrs
                .WithClasses("govuk-list", "govuk-error-summary__list"));

            foreach (var item in options.ErrorList)
            {
                if (item is null)
                {
                    continue;
                }

                var itemTag = new HtmlTag("li", attrs => attrs
                    .With(item.ItemAttributes));

                var itemContent = HtmlOrText(item.Html, item.Text);

                if (item.Href?.IsEmpty() is false)
                {
                    var linkTag = new HtmlTag("a", attrs => attrs
                        .With("href", item.Href)
                        .With(item.Attributes))
                    {
                        itemContent
                    };

                    itemTag.InnerHtml.AppendHtml(linkTag);
                }
                else
                {
                    itemTag.InnerHtml.AppendHtml(itemContent);
                }

                listTag.InnerHtml.AppendHtml(itemTag);
            }

            bodyTag.InnerHtml.AppendHtml(listTag);
        }

        alertDiv.InnerHtml.AppendHtml(bodyTag);
        outerTag.InnerHtml.AppendHtml(alertDiv);

        return GenerateFromHtmlTagAsync(outerTag);

        static TemplateString? GetDisableAutoFocusValue(bool? disableAutoFocus)
        {
            if (disableAutoFocus is true)
            {
                return "true";
            }

            if (disableAutoFocus is false)
            {
                return "false";
            }

            return null;
        }
    }
}
