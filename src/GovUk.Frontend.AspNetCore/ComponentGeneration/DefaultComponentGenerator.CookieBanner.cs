using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateCookieBannerAsync(CookieBannerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var bannerTag = new HtmlTag("div", attrs =>
        {
            attrs
                .WithClasses("govuk-cookie-banner", options.Classes)
                .WithBoolean("data-nosnippet")
                .With("role", "region")
                .With("aria-label", options.AriaLabel ?? "Cookie banner")
                .WithBoolean("hidden", options.Hidden is true)
                .With(options.Attributes);
        });

        if (options.Messages is not null)
        {
            foreach (var message in options.Messages)
            {
                var messageTag = await CreateMessageTagAsync(message);
                bannerTag.InnerHtml.AppendHtml(messageTag);
            }
        }

        return await GenerateFromHtmlTagAsync(bannerTag);

        async Task<IHtmlContent> CreateMessageTagAsync(CookieBannerOptionsMessage message)
        {
            var messageTag = new HtmlTag("div", attrs =>
            {
                attrs
                    .WithClasses("govuk-cookie-banner__message", message.Classes, "govuk-width-container")
                    .With("role", message.Role)
                    .WithBoolean("hidden", message.Hidden is true)
                    .With(message.Attributes);
            });

            var gridRowTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-row"));
            var gridColumnTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-column-two-thirds"));

            if (!message.HeadingHtml.IsEmpty() || !message.HeadingText.IsEmpty())
            {
                var headingTag = new HtmlTag("h2", attrs =>
                {
                    attrs
                        .WithClasses("govuk-cookie-banner__heading", "govuk-heading-m")
                        .With(message.HeadingAttributes);
                });

                headingTag.InnerHtml.AppendHtml(HtmlOrText(message.HeadingHtml, message.HeadingText));
                gridColumnTag.InnerHtml.AppendHtml(headingTag);
            }

            var contentTag = new HtmlTag("div", attrs =>
            {
                attrs
                    .WithClasses("govuk-cookie-banner__content")
                    .With(message.ContentAttributes);
            });

            if (!message.Html.IsEmpty() || !message.Text.IsEmpty())
            {
                var content = HtmlOrText(message.Html, message.Text);

                if (message.Html.IsEmpty() && !message.Text.IsEmpty())
                {
                    var textParagraphTag = new HtmlTag("p", attrs => attrs.WithClasses("govuk-body"));
                    textParagraphTag.InnerHtml.AppendHtml(content);
                    contentTag.InnerHtml.AppendHtml(textParagraphTag);
                }
                else
                {
                    contentTag.InnerHtml.AppendHtml(content);
                }
            }

            gridColumnTag.InnerHtml.AppendHtml(contentTag);
            gridRowTag.InnerHtml.AppendHtml(gridColumnTag);
            messageTag.InnerHtml.AppendHtml(gridRowTag);

            if (message.Actions is not null && message.Actions.Count > 0)
            {
                var buttonGroupTag = new HtmlTag("div", attrs =>
                {
                    attrs
                        .WithClasses("govuk-button-group")
                        .With(message.ActionsAttributes);
                });

                foreach (var action in message.Actions)
                {
                    var actionTag = await CreateActionTagAsync(action);
                    buttonGroupTag.InnerHtml.AppendHtml(actionTag);
                }

                messageTag.InnerHtml.AppendHtml(buttonGroupTag);
            }

            return messageTag;
        }

        async Task<IHtmlContent> CreateActionTagAsync(CookieBannerOptionsMessageAction action)
        {
            var href = action.Href;
            var type = action.Type;

            if (href.IsEmpty() || type == "button")
            {
                var buttonOptions = new ButtonOptions
                {
                    Text = action.Text,
                    Type = action.Type ?? "button",
                    Name = action.Name,
                    Value = action.Value,
                    Classes = action.Classes,
                    Href = action.Href,
                    Attributes = action.Attributes
                };

                return await GenerateButtonAsync(buttonOptions);
            }

            // Otherwise, render as a plain link
            var linkTag = new HtmlTag("a", attrs =>
            {
                attrs
                    .WithClasses("govuk-link", action.Classes)
                    .With("href", href)
                    .With(action.Attributes);
            });

            if (action.Text is not null)
            {
                linkTag.InnerHtml.AppendHtml(action.Text);
            }

            return linkTag;
        }
    }
}
