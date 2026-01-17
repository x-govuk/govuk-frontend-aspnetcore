using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateFooterAsync(FooterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var footerTag = new HtmlTag("footer", attrs => attrs
            .WithClasses("govuk-footer", options.Classes)
            .With(options.Attributes));

        var containerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-width-container", options.ContainerClasses));

        // Add rebrand logo if enabled
        if (options.Rebrand == true)
        {
            containerTag.InnerHtml.AppendHtml(GenerateLogo(new LogoOptions
            {
                Classes = "govuk-footer__crown",
                Rebrand = true,
                UseLogotype = false
            }));
        }

        // Add navigation section if present
        if (options.Navigation is not null && options.Navigation.Count > 0)
        {
            var navigationTag = GenerateNavigationSection(options.Navigation);
            containerTag.InnerHtml.AppendHtml(navigationTag);
            containerTag.InnerHtml.AppendHtml(new HtmlTag("hr", attrs => attrs.WithClasses("govuk-footer__section-break"))
            {
                TagRenderMode = TagRenderMode.SelfClosing
            });
        }

        // Add meta section
        var metaTag = GenerateMetaSection(options.Meta, options.ContentLicence, options.Copyright);
        containerTag.InnerHtml.AppendHtml(metaTag);

        footerTag.InnerHtml.AppendHtml(containerTag);

        return GenerateFromHtmlTagAsync(footerTag);

        // Local helper functions
        HtmlTag GenerateNavigationSection(IReadOnlyCollection<FooterOptionsNavigation> navigation)
        {
            var navContainerTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-footer__navigation"));

            foreach (var nav in navigation)
            {
                var width = nav.Width?.ToHtmlString() ?? "full";
                var sectionTag = new HtmlTag("div", attrs => attrs
                    .WithClasses($"govuk-footer__section", $"govuk-grid-column-{width}")
                    .With(nav.Attributes));

                var headingTag = new HtmlTag("h2", attrs => attrs
                    .WithClasses("govuk-footer__heading", "govuk-heading-m")
                    .With(nav.TitleAttributes));
                headingTag.InnerHtml.AppendHtml(nav.Title?.ToHtmlString() ?? string.Empty);
                sectionTag.InnerHtml.AppendHtml(headingTag);

                if (nav.Items is not null && nav.Items.Count > 0)
                {
                    var listClasses = nav.Columns > 0 ? $"govuk-footer__list--columns-{nav.Columns}" : null;
                    var ulTag = new HtmlTag("ul", attrs => attrs
                        .WithClasses("govuk-footer__list", listClasses)
                        .With(nav.ItemsAttributes));

                    foreach (var item in nav.Items)
                    {
                        if (item.Href is not null && (item.Text is not null || item.Html is not null))
                        {
                            var liTag = new HtmlTag("li", attrs => attrs
                                .WithClasses("govuk-footer__list-item")
                                .With(item.ItemAttributes));

                            var aTag = new HtmlTag("a", attrs => attrs
                                .WithClasses("govuk-footer__link")
                                .With("href", item.Href)
                                .With(item.Attributes));
                            aTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));

                            liTag.InnerHtml.AppendHtml(aTag);
                            ulTag.InnerHtml.AppendHtml(liTag);
                        }
                    }

                    sectionTag.InnerHtml.AppendHtml(ulTag);
                }

                navContainerTag.InnerHtml.AppendHtml(sectionTag);
            }

            return navContainerTag;
        }

        HtmlTag GenerateMetaSection(FooterOptionsMeta? meta, FooterOptionsContentLicence? contentLicence, FooterOptionsCopyright? copyright)
        {
            var metaTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-footer__meta")
                .With(meta?.Attributes));

            var metaItemGrowTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-footer__meta-item", "govuk-footer__meta-item--grow"));

            // Meta content
            if (meta is not null)
            {
                var visuallyHiddenTitle = meta.VisuallyHiddenTitle?.ToHtmlString() ?? "Support links";
                var h2Tag = new HtmlTag("h2", attrs => attrs
                    .WithClasses("govuk-visually-hidden")
                    .With(meta.ContentAttributes));
                h2Tag.InnerHtml.AppendHtml(visuallyHiddenTitle);
                metaItemGrowTag.InnerHtml.AppendHtml(h2Tag);

                if (meta.Items is not null && meta.Items.Count > 0)
                {
                    var ulTag = new HtmlTag("ul", attrs => attrs
                        .WithClasses("govuk-footer__inline-list")
                        .With(meta.ItemsAttributes));

                    foreach (var item in meta.Items)
                    {
                        var liTag = new HtmlTag("li", attrs => attrs
                            .WithClasses("govuk-footer__inline-list-item")
                            .With(item.ItemAttributes));

                        var aTag = new HtmlTag("a", attrs => attrs
                            .WithClasses("govuk-footer__link")
                            .With("href", item.Href)
                            .With(item.Attributes));
                        aTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));

                        liTag.InnerHtml.AppendHtml(aTag);
                        ulTag.InnerHtml.AppendHtml(liTag);
                    }

                    metaItemGrowTag.InnerHtml.AppendHtml(ulTag);
                }

                if (meta.Text is not null || meta.Html is not null)
                {
                    var customDivTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-footer__meta-custom"));
                    customDivTag.InnerHtml.AppendHtml(HtmlOrText(meta.Html, meta.Text));
                    metaItemGrowTag.InnerHtml.AppendHtml(customDivTag);
                }
            }

            // OGL licence logo SVG
            metaItemGrowTag.InnerHtml.AppendHtml(GenerateOglLicenceLogo());

            // Content licence
            var licenceSpanTag = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-footer__licence-description")
                .With(contentLicence?.Attributes));

            if (contentLicence?.Html is not null || contentLicence?.Text is not null)
            {
                licenceSpanTag.InnerHtml.AppendHtml(HtmlOrText(contentLicence.Html, contentLicence.Text));
            }
            else
            {
                licenceSpanTag.InnerHtml.AppendHtml("All content is available under the ");
                var licenceLink = new HtmlTag("a", attrs => attrs
                    .WithClasses("govuk-footer__link")
                    .With("href", "https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/")
                    .With("rel", "license"));
                licenceLink.InnerHtml.AppendHtml("Open Government Licence v3.0");
                licenceSpanTag.InnerHtml.AppendHtml(licenceLink);
                licenceSpanTag.InnerHtml.AppendHtml(", except where otherwise stated");
            }

            metaItemGrowTag.InnerHtml.AppendHtml(licenceSpanTag);
            metaTag.InnerHtml.AppendHtml(metaItemGrowTag);

            // Copyright
            var copyrightItemTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-footer__meta-item")
                .With(copyright?.Attributes));

            var copyrightLink = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-footer__link", "govuk-footer__copyright-logo")
                .With("href", "https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/"));

            if (copyright?.Html is not null || copyright?.Text is not null)
            {
                copyrightLink.InnerHtml.AppendHtml(HtmlOrText(copyright.Html, copyright.Text));
            }
            else
            {
                copyrightLink.InnerHtml.AppendHtml("&#xA9; Crown copyright");
            }

            copyrightItemTag.InnerHtml.AppendHtml(copyrightLink);
            metaTag.InnerHtml.AppendHtml(copyrightItemTag);

            return metaTag;
        }

        IHtmlContent GenerateOglLicenceLogo()
        {
            // SVG for OGL licence logo
            var svgTag = new HtmlTag("svg", attrs => attrs
                .With("aria-hidden", "true")
                .WithBoolean("focusable")  // focusable="false" in the template
                .WithClasses("govuk-footer__licence-logo")
                .With("xmlns", "http://www.w3.org/2000/svg")
                .With("viewBox", "0 0 483.2 195.7")
                .With("height", "17")
                .With("width", "41"));

            // Set focusable to false
            svgTag.Attributes.Set("focusable", "false");

            var pathTag = new HtmlTag("path", attrs => attrs
                .With("fill", "currentColor")
                .With("d", "M421.5 142.8V.1l-50.7 32.3v161.1h112.4v-50.7zm-122.3-9.6A47.12 47.12 0 0 1 221 97.8c0-26 21.1-47.1 47.1-47.1 16.7 0 31.4 8.7 39.7 21.8l42.7-27.2A97.63 97.63 0 0 0 268.1 0c-36.5 0-68.3 20.1-85.1 49.7A98 98 0 0 0 97.8 0C43.9 0 0 43.9 0 97.8s43.9 97.8 97.8 97.8c36.5 0 68.3-20.1 85.1-49.7a97.76 97.76 0 0 0 149.6 25.4l19.4 22.2h3v-87.8h-80l24.3 27.5zM97.8 145c-26 0-47.1-21.1-47.1-47.1s21.1-47.1 47.1-47.1 47.2 21 47.2 47S123.8 145 97.8 145"))
            {
                TagRenderMode = TagRenderMode.SelfClosing
            };

            svgTag.InnerHtml.AppendHtml(pathTag);

            return svgTag;
        }
    }
}
