using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GeneratePaginationAsync(PaginationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var blockLevel = (options.Items is null || options.Items.Count == 0) && (options.Next is not null || options.Previous is not null);

        var navTag = new HtmlTag("nav", attrs => attrs
            .WithClasses("govuk-pagination", blockLevel ? "govuk-pagination--block" : null, options.Classes)
            .With("aria-label", options.LandmarkLabel ?? "Pagination")
            .With(options.Attributes));

        if (!(options.Previous?.Href).IsEmpty())
        {
            var previousLinkContent = GetLinkContent(options.Previous.Html, options.Previous.Text, "Previous", " page");
            var previousTag = GeneratePrevLink(options.Previous, previousLinkContent, blockLevel);
            navTag.InnerHtml.AppendHtml(previousTag);
        }

        if (options.Items is not null && options.Items.Count > 0)
        {
            var ulTag = new HtmlTag("ul", attrs => attrs
                .WithClasses("govuk-pagination__list"));

            foreach (var item in options.Items)
            {
                if (item is null || (item.Ellipsis is not true && item.Href.IsEmpty()))
                {
                    continue;
                }

                var liTag = GeneratePageItem(item);
                ulTag.InnerHtml.AppendHtml(liTag);
            }

            navTag.InnerHtml.AppendHtml(ulTag);
        }

        if (!(options.Next?.Href).IsEmpty())
        {
            var nextLinkContent = GetLinkContent(options.Next.Html, options.Next.Text, "Next", " page");
            var nextTag = GenerateNextLink(options.Next, nextLinkContent, blockLevel);
            navTag.InnerHtml.AppendHtml(nextTag);
        }

        return GenerateFromHtmlTagAsync(navTag);

        IHtmlContent GetLinkContent(TemplateString? html, TemplateString? text, string defaultText, string visuallyHiddenSuffix)
        {
            if (!html.IsEmpty() || !text.IsEmpty())
            {
                return HtmlOrText(html, text);
            }

            var content = new HtmlContentBuilder();
            content.Append(defaultText);
            var spanTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-visually-hidden"))
            {
                visuallyHiddenSuffix
            };
            content.AppendHtml(spanTag);
            return content;
        }

        HtmlTag GeneratePrevLink(PaginationOptionsPrevious link, IHtmlContent content, bool isBlockLevel)
        {
            var divTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-pagination__prev")
                .With(link.ContainerAttributes));

            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-link", "govuk-pagination__link")
                .With("href", link.Href)
                .With("rel", "prev")
                .With(link.Attributes));

            // Add arrow before content for "prev" or when block level
            if (isBlockLevel || true) // always true for prev
            {
                var arrowSvg = GenerateArrowSvg(true);
                aTag.InnerHtml.AppendHtml(arrowSvg);
            }

            var spanTag = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-pagination__link-title", isBlockLevel && link.LabelText is null ? "govuk-pagination__link-title--decorated" : null));
            spanTag.InnerHtml.AppendHtml(content);
            aTag.InnerHtml.AppendHtml(spanTag);

            if (link.LabelText is not null && isBlockLevel)
            {
                var colonTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-visually-hidden"))
                {
                    ":"
                };
                aTag.InnerHtml.AppendHtml(colonTag);
                var labelTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-pagination__link-label"));
                labelTag.InnerHtml.AppendHtml(link.LabelText);
                aTag.InnerHtml.AppendHtml(labelTag);
            }

            divTag.InnerHtml.AppendHtml(aTag);
            return divTag;
        }

        HtmlTag GenerateNextLink(PaginationOptionsNext link, IHtmlContent content, bool isBlockLevel)
        {
            var divTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-pagination__next")
                .With(link.ContainerAttributes));

            var aTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-link", "govuk-pagination__link")
                .With("href", link.Href)
                .With("rel", "next")
                .With(link.Attributes));

            if (isBlockLevel)
            {
                var arrowSvg = GenerateArrowSvg(false);
                aTag.InnerHtml.AppendHtml(arrowSvg);
            }

            var spanTag = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-pagination__link-title", isBlockLevel && link.LabelText is null ? "govuk-pagination__link-title--decorated" : null));
            spanTag.InnerHtml.AppendHtml(content);
            aTag.InnerHtml.AppendHtml(spanTag);

            if (link.LabelText is not null && isBlockLevel)
            {
                var colonTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-visually-hidden"))
                {
                    ":"
                };
                aTag.InnerHtml.AppendHtml(colonTag);
                var labelTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-pagination__link-label"));
                labelTag.InnerHtml.AppendHtml(link.LabelText);
                aTag.InnerHtml.AppendHtml(labelTag);
            }

            if (!isBlockLevel)
            {
                var arrowSvg = GenerateArrowSvg(false);
                aTag.InnerHtml.AppendHtml(arrowSvg);
            }

            divTag.InnerHtml.AppendHtml(aTag);
            return divTag;
        }

        HtmlTag GenerateArrowSvg(bool isPrev)
        {
            var svgClass = isPrev ? "govuk-pagination__icon--prev" : "govuk-pagination__icon--next";
            var svgTag = new HtmlTag("svg", attrs => attrs
                .WithClasses("govuk-pagination__icon", svgClass)
                .With("xmlns", "http://www.w3.org/2000/svg")
                .With("height", "13")
                .With("width", "15")
                .With("aria-hidden", "true")
                .With("focusable", "false")
                .With("viewBox", "0 0 15 13"));

            var pathD = isPrev
                ? "m6.5938-0.0078125-6.7266 6.7266 6.7441 6.4062 1.377-1.449-4.1856-3.9768h12.896v-2h-12.984l4.2931-4.293-1.414-1.414z"
                : "m8.107-0.0078125-1.4136 1.414 4.2926 4.293h-12.986v2h12.896l-4.1855 3.9766 1.377 1.4492 6.7441-6.4062-6.7246-6.7266z";

            var pathTag = new HtmlTag("path", attrs => attrs.With("d", pathD))
            {
                TagRenderMode = TagRenderMode.SelfClosing
            };
            svgTag.InnerHtml.AppendHtml(pathTag);

            return svgTag;
        }

        HtmlTag GeneratePageItem(PaginationOptionsItem item)
        {
            var liTag = new HtmlTag("li", attrs => attrs
                .WithClasses(
                    "govuk-pagination__item",
                    item.Current is true ? "govuk-pagination__item--current" : null,
                    item.Ellipsis is true ? "govuk-pagination__item--ellipsis" : null));

            if (item.Ellipsis is true)
            {
                liTag.InnerHtml.Append("⋯");
            }
            else
            {
                var aTag = new HtmlTag("a", attrs =>
                {
                    var ariaLabel = item.VisuallyHiddenText
                        ?? (item.Number is not null ? new TemplateString($"Page {item.Number}") : null);

                    attrs
                        .WithClasses("govuk-link", "govuk-pagination__link")
                        .With("href", item.Href)
                        .With("aria-label", ariaLabel);

                    if (item.Current == true)
                    {
                        attrs.With("aria-current", "page");
                    }

                    attrs.With(item.Attributes);
                });

                aTag.InnerHtml.AppendHtml(item.Number ?? TemplateString.Empty);
                liTag.InnerHtml.AppendHtml(aTag);
            }

            return liTag;
        }
    }
}
