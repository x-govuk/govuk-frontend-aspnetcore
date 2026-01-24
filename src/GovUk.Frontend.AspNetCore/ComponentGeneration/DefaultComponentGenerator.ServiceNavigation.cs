using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateServiceNavigationAsync(ServiceNavigationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var menuButtonText = options.MenuButtonText ?? "Menu";
        var navigationId = options.NavigationId ?? "navigation";

        var innerContent = CreateInnerContent();

        var containerTag = CreateContainerTag();

        return GenerateFromHtmlTagAsync(containerTag);

        HtmlTag CreateInnerContent()
        {
            var widthContainerTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-width-container"));

            if (!(options.Slots?.Start).IsEmpty())
            {
                widthContainerTag.InnerHtml.AppendHtml(options.Slots.Start.GetRawHtml());
            }

            var serviceNavigationContainer = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-service-navigation__container"));

            if (!options.ServiceName.IsEmpty())
            {
                var serviceNameSpan = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-service-navigation__service-name"));

                if (!options.ServiceUrl.IsEmpty())
                {
                    var linkTag = new HtmlTag("a", attrs => attrs
                        .WithClasses("govuk-service-navigation__link")
                        .With("href", options.ServiceUrl));
                    linkTag.InnerHtml.AppendHtml(options.ServiceName);
                    serviceNameSpan.InnerHtml.AppendHtml(linkTag);
                }
                else
                {
                    var textSpan = new HtmlTag("span", attrs => attrs
                        .WithClasses("govuk-service-navigation__text"));
                    textSpan.InnerHtml.AppendHtml(options.ServiceName);
                    serviceNameSpan.InnerHtml.AppendHtml(textSpan);
                }

                serviceNavigationContainer.InnerHtml.AppendHtml(serviceNameSpan);
            }

            var navigationItems = options.Navigation;
            var collapseNavigationOnMobile = options.CollapseNavigationOnMobile ?? navigationItems?.Count > 1;

            if (navigationItems?.Count > 0 ||
                !(options.Slots?.NavigationStart).IsEmpty() ||
                !(options.Slots?.NavigationEnd).IsEmpty())
            {
                var navTag = CreateNavigationTag(
                    collapseNavigationOnMobile,
                    navigationItems);

                serviceNavigationContainer.InnerHtml.AppendHtml(navTag);
            }

            widthContainerTag.InnerHtml.AppendHtml(serviceNavigationContainer);

            if (!(options.Slots?.End).IsEmpty())
            {
                widthContainerTag.InnerHtml.AppendHtml(options.Slots.End.GetRawHtml());
            }

            return widthContainerTag;
        }

        HtmlTag CreateNavigationTag(
            bool collapseNavigationOnMobile,
            IReadOnlyCollection<ServiceNavigationOptionsNavigationItem?>? navigationItems)
        {
            var ariaLabel = TemplateString.Coalesce(options.NavigationLabel, menuButtonText);

            var navTag = new HtmlTag("nav", attrs => attrs
                .WithClasses("govuk-service-navigation__wrapper", options.NavigationClasses)
                .With("aria-label", ariaLabel)
                .With(options.NavigationAttributes));

            if (collapseNavigationOnMobile)
            {
                var buttonTag = new HtmlTag("button", attrs =>
                {
                    attrs
                        .WithClasses("govuk-service-navigation__toggle", "govuk-js-service-navigation-toggle")
                        .With("type", "button")
                        .With("aria-controls", navigationId)
                        .WithBoolean("hidden")
                        .With("aria-hidden", "true");

                    if (!options.MenuButtonLabel.IsEmpty() && options.MenuButtonLabel != menuButtonText)
                    {
                        attrs.With("aria-label", options.MenuButtonLabel);
                    }
                });

                buttonTag.InnerHtml.AppendHtml(menuButtonText);
                navTag.InnerHtml.AppendHtml(buttonTag);
            }

            var ulTag = new HtmlTag("ul", attrs => attrs
                .WithClasses("govuk-service-navigation__list")
                .With("id", navigationId));

            if (!(options.Slots?.NavigationStart).IsEmpty())
            {
                ulTag.InnerHtml.AppendHtml(options.Slots.NavigationStart.GetRawHtml());
            }

            if (navigationItems is not null)
            {
                foreach (var item in navigationItems)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    var liTag = CreateNavigationItem(item);
                    ulTag.InnerHtml.AppendHtml(liTag);
                }
            }

            if (!(options.Slots?.NavigationEnd).IsEmpty())
            {
                ulTag.InnerHtml.AppendHtml(options.Slots.NavigationEnd.GetRawHtml());
            }

            navTag.InnerHtml.AppendHtml(ulTag);

            return navTag;
        }

        HtmlTag CreateNavigationItem(ServiceNavigationOptionsNavigationItem item)
        {
            var isActive = item.Active is true || item.Current is true;

            var liTag = new HtmlTag("li", attrs => attrs
                .WithClasses(
                    "govuk-service-navigation__item",
                    isActive ? "govuk-service-navigation__item--active" : null));

            var linkInnerContent = CreateLinkInnerContent(item);

            if (!item.Href.IsEmpty())
            {
                var aTag = new HtmlTag("a", attrs =>
                {
                    attrs
                        .WithClasses("govuk-service-navigation__link")
                        .With("href", item.Href)
                        .With(item.Attributes);

                    if (isActive)
                    {
                        var ariaCurrent = item.Current is true ? "page" : "true";
                        attrs.With("aria-current", ariaCurrent);
                    }
                });

                aTag.InnerHtml.AppendHtml(linkInnerContent);
                liTag.InnerHtml.AppendHtml(aTag);
            }
            else if (!item.Html.IsEmpty() || !item.Text.IsEmpty())
            {
                var spanTag = new HtmlTag("span", attrs =>
                {
                    attrs.WithClasses("govuk-service-navigation__text");

                    if (isActive)
                    {
                        var ariaCurrent = item.Current is true ? "page" : "true";
                        attrs.With("aria-current", ariaCurrent);
                    }
                });

                spanTag.InnerHtml.AppendHtml(linkInnerContent);
                liTag.InnerHtml.AppendHtml(spanTag);
            }

            return liTag;
        }

        IHtmlContent CreateLinkInnerContent(ServiceNavigationOptionsNavigationItem item)
        {
            var content = HtmlOrText(item.Html, item.Text);

            if (item.Active is true || item.Current is true)
            {
                var strongTag = new HtmlTag("strong", attrs => attrs
                    .WithClasses("govuk-service-navigation__active-fallback"));
                strongTag.InnerHtml.AppendHtml(content);
                return strongTag;
            }

            return content;
        }

        HtmlTag CreateContainerTag()
        {
            if (!options.ServiceName.IsEmpty() ||
                !(options.Slots?.Start).IsEmpty() ||
                !(options.Slots?.End).IsEmpty())
            {
                var ariaLabel = TemplateString.Coalesce(options.AriaLabel, "Service information");

                var sectionTag = new HtmlTag("section", attrs => attrs
                    .WithClasses("govuk-service-navigation", options.Classes)
                    .With("data-module", "govuk-service-navigation")
                    .With("aria-label", ariaLabel)
                    .With(options.Attributes));

                sectionTag.InnerHtml.AppendHtml(innerContent);
                return sectionTag;
            }
            else
            {
                var divTag = new HtmlTag("div", attrs => attrs
                    .WithClasses("govuk-service-navigation", options.Classes)
                    .With("data-module", "govuk-service-navigation")
                    .With(options.Attributes));

                divTag.InnerHtml.AppendHtml(innerContent);
                return divTag;
            }
        }
    }
}
