namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateHeaderAsync(HeaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var rebrand = options.Rebrand ?? false;
        var menuButtonText = options.MenuButtonText?.ToString() ?? "Menu";

        var headerTag = new HtmlTag("header", attrs => attrs
            .WithClasses("govuk-header", options.Classes)
            .With("data-module", "govuk-header")
            .With(options.Attributes));

        var containerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-header__container", options.ContainerClasses?.ToString() ?? "govuk-width-container")
            .With(options.ContainerAttributes));

        // Logo section
        var logoDiv = CreateLogoSection(options, rebrand);
        containerTag.InnerHtml.AppendHtml(logoDiv);

        // Content section (service name and navigation)
        if (options.ServiceName?.IsEmpty() == false || (options.Navigation?.Count ?? 0) > 0)
        {
            var contentDiv = CreateContentSection(options, menuButtonText);
            containerTag.InnerHtml.AppendHtml(contentDiv);
        }

        headerTag.InnerHtml.AppendHtml(containerTag);

        return GenerateFromHtmlTagAsync(headerTag);

        HtmlTag CreateLogoSection(HeaderOptions options, bool rebrand)
        {
            var logoDiv = new HtmlTag("div", attrs => attrs.WithClasses("govuk-header__logo"));

            var logoLink = new HtmlTag("a", attrs => attrs
                .With("href", options.HomePageUrl?.ToString() ?? "/")
                .WithClasses("govuk-header__link", "govuk-header__link--homepage"));

            // Use shared logo generation logic
            logoLink.InnerHtml.AppendHtml(GenerateLogo(new LogoOptions
            {
                Classes = "govuk-header__logotype",
                AriaLabelText = "GOV.UK",
                UseTudorCrown = options.UseTudorCrown,
                Rebrand = rebrand
            }));

            // Product name
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

        HtmlTag CreateContentSection(HeaderOptions options, string menuButtonText)
        {
            var contentDiv = new HtmlTag("div", attrs => attrs.WithClasses("govuk-header__content"));

            // Service name
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

            // Navigation
            if (options.Navigation?.Count > 0)
            {
                var navTag = CreateNavigationSection(options, menuButtonText);
                contentDiv.InnerHtml.AppendHtml(navTag);
            }

            return contentDiv;
        }

        HtmlTag CreateNavigationSection(HeaderOptions options, string menuButtonText)
        {
            var navigationLabel = options.NavigationLabel?.ToString() ?? menuButtonText;

            var navTag = new HtmlTag("nav", attrs => attrs
                .With("aria-label", navigationLabel)
                .WithClasses("govuk-header__navigation", options.NavigationClasses)
                .With(options.NavigationAttributes));

            // Menu button
            var buttonTag = new HtmlTag("button", attrs =>
            {
                attrs
                    .With("type", "button")
                    .WithClasses("govuk-header__menu-button", "govuk-js-header-toggle")
                    .With("aria-controls", "navigation")
                    .WithBoolean("hidden");

                if (options.MenuButtonLabel?.IsEmpty() == false &&
                    options.MenuButtonLabel?.ToString() != menuButtonText)
                {
                    attrs.With("aria-label", options.MenuButtonLabel);
                }
            });
            buttonTag.InnerHtml.Append(menuButtonText);
            navTag.InnerHtml.AppendHtml(buttonTag);

            // Navigation list
            var ulTag = new HtmlTag("ul", attrs => attrs
                .With("id", "navigation")
                .WithClasses("govuk-header__navigation-list"));

            if (options.Navigation is not null)
            {
                foreach (var item in options.Navigation)
                {
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
                    item.Active == true ? "govuk-header__navigation-item--active" : null));

            if (item.Href?.IsEmpty() == false)
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
