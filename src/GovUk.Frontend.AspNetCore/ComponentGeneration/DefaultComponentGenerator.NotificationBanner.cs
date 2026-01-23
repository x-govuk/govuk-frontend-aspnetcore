using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateNotificationBannerAsync(NotificationBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var type = options.Type;
        var isSuccessBanner = type == "success";
        var typeClass = isSuccessBanner ? new TemplateString($"govuk-notification-banner--{type}") : null;
        var role = DetermineRole(options.Role, isSuccessBanner);
        var titleId = options.TitleId ?? "govuk-notification-banner-title";
        var titleHeadingLevel = options.TitleHeadingLevel ?? 2;
        var title = DetermineTitle(options.TitleHtml, options.TitleText);

        var outerTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-notification-banner", typeClass, options.Classes)
                .With("role", role)
                .With("aria-labelledby", titleId)
                .With("data-module", "govuk-notification-banner");

            if (options.DisableAutoFocus is true or false)
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
        if (!content.IsEmpty())
        {
            var hasHtml = !options.Html.IsEmpty();
            var hasText = !options.Text.IsEmpty();
            if (!hasHtml && hasText)
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

        static TemplateString DetermineRole(TemplateString? role, bool isSuccessBanner)
        {
            if (role is not null)
            {
                return role;
            }

            if (isSuccessBanner)
            {
                return "alert";
            }

            return "region";
        }

        IHtmlContent DetermineTitle(TemplateString? titleHtml, TemplateString? titleText)
        {
            if (!titleHtml.IsEmpty())
            {
                return titleHtml.GetRawHtml();
            }

            if (!titleText.IsEmpty())
            {
                return titleText;
            }

            return new HtmlString(isSuccessBanner ? "Success" : "Important");
        }
    }
}
