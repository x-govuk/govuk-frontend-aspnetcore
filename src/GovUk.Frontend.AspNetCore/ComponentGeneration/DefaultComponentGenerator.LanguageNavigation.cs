using GovUk.Frontend.AspNetCore.Localization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateLanguageNavigationAsync(LanguageNavigationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var ariaLabel = options.AriaLabel ??
            LocalizedText(GovUkFrontendResourceNames.LanguageNavigationAriaLabel) ??
            "Language";

        var navTag = new HtmlTag("nav", attrs => attrs
            .WithClasses("govuk-language-navigation", options.Classes)
            .With("aria-label", ariaLabel)
            .With(options.Attributes));

        var ulTag = new HtmlTag("ul", attrs => attrs
            .WithClasses("govuk-language-navigation__list"));

        if (options.Items is not null)
        {
            foreach (var item in options.Items)
            {
                if (item is null)
                {
                    continue;
                }

                var liTag = new HtmlTag("li", attrs => attrs
                    .WithClasses("govuk-language-navigation__list-item"));

                liTag.InnerHtml.AppendHtml(CreateItemContent(item));
                ulTag.InnerHtml.AppendHtml(liTag);
            }
        }

        navTag.InnerHtml.AppendHtml(ulTag);

        return GenerateFromHtmlTagAsync(navTag);

        HtmlTag CreateItemContent(LanguageNavigationOptionsItem item)
        {
            // An item without a link is the language the page is already in.
            var isCurrent = item.Current is true || item.Href.IsEmpty();

            if (isCurrent)
            {
                var textTag = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-language-navigation__text", item.Classes)
                    .With("aria-current", "true")
                    .With("lang", item.Lang)
                    .With("dir", item.Dir)
                    .With(item.Attributes));

                textTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));
                return textTag;
            }

            var linkTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-language-navigation__link", item.Classes)
                .With("href", item.Href)
                .With("rel", "alternate")
                .With("lang", item.Lang)
                .With("hreflang", TemplateString.Coalesce(item.HrefLang, item.Lang))
                .With("dir", item.Dir)
                .With(item.Attributes));

            linkTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));

            if (!item.LanguageDescriptionText.IsEmpty())
            {
                var descriptionTag = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-visually-hidden"));
                descriptionTag.InnerHtml.Append(" " + item.LanguageDescriptionText);
                linkTag.InnerHtml.AppendHtml(descriptionTag);
            }

            return linkTag;
        }
    }
}
