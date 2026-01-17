namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateNotificationBannerAsync(NotificationBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var isSuccessBanner = options.Type?.ToHtmlString(raw: true) == "success";
        var role = DetermineRole(options.Role, isSuccessBanner);
        var titleId = options.TitleId?.ToHtmlString(raw: true) ?? "govuk-notification-banner-title";
        var titleHeadingLevel = options.TitleHeadingLevel ?? 2;
        var title = DetermineTitle(options.TitleHtml, options.TitleText, isSuccessBanner);

        var outerTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-notification-banner", isSuccessBanner ? "govuk-notification-banner--success" : null, options.Classes)
                .With("role", role)
                .With("aria-labelledby", titleId)
                .With("data-module", "govuk-notification-banner");

            if (options.DisableAutoFocus == true || options.DisableAutoFocus == false)
            {
                attrs.With("data-disable-auto-focus", options.DisableAutoFocus.Value ? "true" : "false");
            }

            attrs.With(options.Attributes);
        });

        var headerTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-notification-banner__header"));
        var titleTag = new HtmlTag($"h{titleHeadingLevel}", attrs => attrs
            .WithClasses("govuk-notification-banner__title")
            .With("id", titleId));
        titleTag.InnerHtml.AppendHtml(title);
        headerTag.InnerHtml.AppendHtml(titleTag);
        outerTag.InnerHtml.AppendHtml(headerTag);

        var contentTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-notification-banner__content"));
        var content = HtmlOrText(options.Html, options.Text);
        if (content != Microsoft.AspNetCore.Html.HtmlString.Empty)
        {
            // If we have text (not HTML), wrap it in the default paragraph style
            if (options.Html?.IsEmpty() != false && options.Text?.IsEmpty() == false)
            {
                var paragraphTag = new HtmlTag("p", attrs => attrs.WithClasses("govuk-notification-banner__heading"));
                paragraphTag.InnerHtml.AppendHtml(content);
                contentTag.InnerHtml.AppendHtml(paragraphTag);
            }
            else
            {
                contentTag.InnerHtml.AppendHtml(content);
            }
        }
        outerTag.InnerHtml.AppendHtml(contentTag);

        return GenerateFromHtmlTagAsync(outerTag);

        static string DetermineRole(TemplateString? role, bool isSuccessBanner)
        {
            if (role is not null)
            {
                return role.ToHtmlString(raw: true);
            }

            // If type is success, add role="alert" to prioritise the information in the notification banner
            // to users of assistive technologies
            if (isSuccessBanner)
            {
                return "alert";
            }

            // Otherwise add role="region" to make the notification banner a landmark to help users of
            // assistive technologies to navigate to the banner
            return "region";
        }

        Microsoft.AspNetCore.Html.IHtmlContent DetermineTitle(TemplateString? titleHtml, TemplateString? titleText, bool isSuccessBanner)
        {
            if (titleHtml?.IsEmpty() == false)
            {
                return new Microsoft.AspNetCore.Html.HtmlString(titleHtml.ToHtmlString(raw: true));
            }

            if (titleText?.IsEmpty() == false)
            {
                return titleText;
            }

            return new Microsoft.AspNetCore.Html.HtmlString(isSuccessBanner ? "Success" : "Important");
        }
    }
}
