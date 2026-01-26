namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var rebrand = options.Rebrand ?? false;
        var menuButtonText = options.MenuButtonText ?? "Menu";

        var headerTag = new HtmlTag("header", attrs => attrs
            .WithClasses("govuk-header", options.Classes)
            .With("data-module", "govuk-header")
            .With(options.Attributes));

        var containerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-header__container", options.ContainerClasses ?? "govuk-width-container")
            .With(options.ContainerAttributes));

        var logoDiv = CreateLogoSection();
        containerTag.InnerHtml.AppendHtml(logoDiv);

        if (options.ServiceName?.IsEmpty() == false || (options.Navigation?.Count ?? 0) > 0)
        {
            var contentDiv = CreateContentSection();
            containerTag.InnerHtml.AppendHtml(contentDiv);
        }

        headerTag.InnerHtml.AppendHtml(containerTag);

        return GenerateFromHtmlTagAsync(headerTag);

        HtmlTag CreateLogoSection()
        {
            var logoDiv = new HtmlTag("div", attrs => attrs.WithClasses("govuk-header__logo"));

            var logoLink = new HtmlTag("a", attrs => attrs
                .With("href", options.HomePageUrl ?? "/")
                .WithClasses("govuk-header__link", "govuk-header__link--homepage"));

            logoLink.InnerHtml.AppendHtml(GenerateLogo(new LogoOptions
            {
                Classes = "govuk-header__logotype",
                AriaLabelText = "GOV.UK",
                UseTudorCrown = options.UseTudorCrown,
                Rebrand = rebrand
            }));

            if (options.ProductName?.IsEmpty() == false)
            {
                var productNameSpan = new HtmlTag("span", attrs => attrs
                    .WithClasses("govuk-header__product-name"));
                productNameSpan.InnerHtml.AppendHtml(options.ProductName);
                logoLink.InnerHtml.AppendHtml(productNameSpan);
            }

            logoDiv.InnerHtml.AppendHtml(logoLink);
            return logoDiv;
        }

        HtmlTag CreateContentSection()
        {
            var contentDiv = new HtmlTag("div", attrs => attrs.WithClasses("govuk-header__content"));

            if (options.ServiceName?.IsEmpty() == false)
            {
                if (options.ServiceUrl?.IsEmpty() == false)
                {
                    var serviceLink = new HtmlTag("a", attrs => attrs
                        .With("href", options.ServiceUrl)
                        .WithClasses("govuk-header__link", "govuk-header__service-name"));
                    serviceLink.InnerHtml.AppendHtml(options.ServiceName);
                    contentDiv.InnerHtml.AppendHtml(serviceLink);
                }
                else
                {
                    var serviceSpan = new HtmlTag("span", attrs => attrs
                        .WithClasses("govuk-header__service-name"));
                    serviceSpan.InnerHtml.AppendHtml(options.ServiceName);
                    contentDiv.InnerHtml.AppendHtml(serviceSpan);
                }
            }

            if (options.Navigation?.Count > 0)
            {
                var navTag = CreateNavigationSection();
                contentDiv.InnerHtml.AppendHtml(navTag);
            }

            return contentDiv;
        }

        HtmlTag CreateNavigationSection()
        {
            var navigationLabel = options.NavigationLabel ?? menuButtonText;

            var navTag = new HtmlTag("nav", attrs => attrs
                .With("aria-label", navigationLabel)
                .WithClasses("govuk-header__navigation", options.NavigationClasses)
                .With(options.NavigationAttributes));

            var buttonTag = new HtmlTag("button", attrs =>
            {
                attrs
                    .With("type", "button")
                    .WithClasses("govuk-header__menu-button", "govuk-js-header-toggle")
                    .With("aria-controls", "navigation")
                    .WithBoolean("hidden");

                if (options.MenuButtonLabel?.IsEmpty() is false &&
                    options.MenuButtonLabel?.ToString() != menuButtonText)
                {
                    attrs.With("aria-label", options.MenuButtonLabel);
                }
            });
            buttonTag.InnerHtml.AppendHtml(menuButtonText);
            navTag.InnerHtml.AppendHtml(buttonTag);

            var ulTag = new HtmlTag("ul", attrs => attrs
                .With("id", "navigation")
                .WithClasses("govuk-header__navigation-list"));

            if (options.Navigation is not null)
            {
                foreach (var item in options.Navigation)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    if (item.Text?.IsEmpty() == false || item.Html?.IsEmpty() == false)
                    {
                        var liTag = CreateNavigationItem(item);
                        ulTag.InnerHtml.AppendHtml(liTag);
                    }
                }
            }

            navTag.InnerHtml.AppendHtml(ulTag);
            return navTag;
        }

        HtmlTag CreateNavigationItem(HeaderOptionsNavigationItem item)
        {
            var liTag = new HtmlTag("li", attrs => attrs
                .WithClasses(
                    "govuk-header__navigation-item",
                    item.Active is true ? "govuk-header__navigation-item--active" : null));

            if (!item.Href.IsEmpty())
            {
                var aTag = new HtmlTag("a", attrs => attrs
                    .WithClasses("govuk-header__link")
                    .With("href", item.Href)
                    .With(item.Attributes));
                aTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));
                liTag.InnerHtml.AppendHtml(aTag);
            }
            else
            {
                liTag.InnerHtml.AppendHtml(HtmlOrText(item.Html, item.Text));
            }

            return liTag;
        }
    }
}
