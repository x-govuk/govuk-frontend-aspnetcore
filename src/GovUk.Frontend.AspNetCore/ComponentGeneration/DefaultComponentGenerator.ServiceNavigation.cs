using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateServiceNavigationAsync(ServiceNavigationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var menuButtonText = options.MenuButtonText?.ToString() ?? "Menu";
        var navigationId = options.NavigationId?.ToString() ?? "navigation";

        var innerContent = CreateInnerContent(options, menuButtonText, navigationId);

        var containerTag = CreateContainerTag(options, innerContent);

        return GenerateFromHtmlTagAsync(containerTag);

        HtmlTag CreateInnerContent(ServiceNavigationOptions options, string menuButtonText, string navigationId)
        {
            var widthContainerTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-width-container"));

            // Start slot
            if (options.Slots?.Start?.IsEmpty() == false)
            {
                widthContainerTag.InnerHtml.AppendHtml(new HtmlString(options.Slots.Start.ToHtmlString(raw: true)));
            }

            var serviceNavigationContainer = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-service-navigation__container"));

            // Service name
            if (options.ServiceName?.IsEmpty() == false)
            {
                var serviceNameSpan = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-service-navigation__service-name"));

                if (options.ServiceUrl?.IsEmpty() == false)
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

            // Navigation
            var navigationItems = options.Navigation;
            var collapseNavigationOnMobile = options.CollapseNavigationOnMobile ?? (navigationItems?.Count > 1);

            if ((navigationItems?.Count > 0) ||
                options.Slots?.NavigationStart?.IsEmpty() == false ||
                options.Slots?.NavigationEnd?.IsEmpty() == false)
            {
                var navTag = CreateNavigationTag(
                    options,
                    menuButtonText,
                    navigationId,
                    collapseNavigationOnMobile,
                    navigationItems);

                serviceNavigationContainer.InnerHtml.AppendHtml(navTag);
            }

            widthContainerTag.InnerHtml.AppendHtml(serviceNavigationContainer);

            // End slot
            if (options.Slots?.End?.IsEmpty() == false)
            {
                widthContainerTag.InnerHtml.AppendHtml(new HtmlString(options.Slots.End.ToHtmlString(raw: true)));
            }

            return widthContainerTag;
        }

        HtmlTag CreateNavigationTag(
            ServiceNavigationOptions options,
            string menuButtonText,
            string navigationId,
            bool collapseNavigationOnMobile,
            IReadOnlyCollection<ServiceNavigationOptionsNavigationItem>? navigationItems)
        {
            var ariaLabel = options.NavigationLabel?.ToString() ?? menuButtonText;

            var navTag = new HtmlTag("nav", attrs => attrs
                .WithClasses("govuk-service-navigation__wrapper", options.NavigationClasses)
                .With("aria-label", ariaLabel)
                .With(options.NavigationAttributes));

            // Menu button (for mobile)
            if (collapseNavigationOnMobile)
            {
                var buttonTag = new HtmlTag("button", attrs =>
                {
                    attrs
                        .WithClasses("govuk-service-navigation__toggle", "govuk-js-service-navigation-toggle")
                        .With("type", "button")
                        .With("aria-controls", navigationId)
                        .WithBoolean("hidden");

                    if (options.MenuButtonLabel?.IsEmpty() == false &&
                        options.MenuButtonLabel?.ToString() != menuButtonText)
                    {
                        attrs.With("aria-label", options.MenuButtonLabel);
                    }
                });

                buttonTag.InnerHtml.Append(menuButtonText);
                navTag.InnerHtml.AppendHtml(buttonTag);
            }

            // Navigation list
            var ulTag = new HtmlTag("ul", attrs => attrs
                .WithClasses("govuk-service-navigation__list")
                .With("id", navigationId));

            // Navigation start slot
            if (options.Slots?.NavigationStart?.IsEmpty() == false)
            {
                ulTag.InnerHtml.AppendHtml(new HtmlString(options.Slots.NavigationStart.ToHtmlString(raw: true)));
            }

            // Navigation items
            if (navigationItems is not null)
            {
                foreach (var item in navigationItems)
                {
                    var liTag = CreateNavigationItem(item);
                    ulTag.InnerHtml.AppendHtml(liTag);
                }
            }

            // Navigation end slot
            if (options.Slots?.NavigationEnd?.IsEmpty() == false)
            {
                ulTag.InnerHtml.AppendHtml(new HtmlString(options.Slots.NavigationEnd.ToHtmlString(raw: true)));
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

            if (item.Href?.IsEmpty() == false)
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
            else if (item.Html?.IsEmpty() == false || item.Text?.IsEmpty() == false)
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

            // Wrap active links in strong tags for accessibility
            if (item.Active is true || item.Current is true)
            {
                var strongTag = new HtmlTag("strong", attrs => attrs
                    .WithClasses("govuk-service-navigation__active-fallback"));
                strongTag.InnerHtml.AppendHtml(content);
                return strongTag;
            }

            return content;
        }

        HtmlTag CreateContainerTag(
            ServiceNavigationOptions options,
            HtmlTag innerContent)
        {
            // If a service name is included or slots are provided, use a <section> element
            // with an aria-label to create a containing landmark region.
            // Otherwise, the <nav> in the innerContent can do the job by itself.
            if (options.ServiceName?.IsEmpty() == false ||
                options.Slots?.Start?.IsEmpty() == false ||
                options.Slots?.End?.IsEmpty() == false)
            {
                var ariaLabel = options.AriaLabel?.ToString() ?? "Service information";

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
