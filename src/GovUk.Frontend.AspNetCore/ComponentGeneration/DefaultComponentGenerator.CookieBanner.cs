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
                .With("data-nosnippet", string.Empty)
                .With("role", "region")
                .With("aria-label", options.AriaLabel ?? "Cookie banner");

            if (options.Hidden == true)
            {
                attrs.WithBoolean("hidden");
            }

            attrs.With(options.Attributes);
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

        async Task<HtmlTag> CreateMessageTagAsync(CookieBannerOptionsMessage message)
        {
            var messageTag = new HtmlTag("div", attrs =>
            {
                attrs
                    .WithClasses("govuk-cookie-banner__message", message.Classes, "govuk-width-container")
                    .With("role", message.Role);

                if (message.Hidden == true)
                {
                    attrs.WithBoolean("hidden");
                }

                attrs.With(message.Attributes);
            });

            var gridRowTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-row"));
            var gridColumnTag = new HtmlTag("div", attrs => attrs.WithClasses("govuk-grid-column-two-thirds"));

            // Add heading if present
            if (message.HeadingHtml?.IsEmpty() == false || message.HeadingText?.IsEmpty() == false)
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

            // Add content
            var contentTag = new HtmlTag("div", attrs =>
            {
                attrs
                    .WithClasses("govuk-cookie-banner__content")
                    .With(message.ContentAttributes);
            });

            if (message.Html?.IsEmpty() == false || message.Text?.IsEmpty() == false)
            {
                var content = HtmlOrText(message.Html, message.Text);
                
                // If text was provided (not HTML), wrap it in a paragraph tag
                if (message.Html?.IsEmpty() != false && message.Text?.IsEmpty() == false)
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

            // Add actions if present
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

        async Task<HtmlTag> CreateActionTagAsync(CookieBannerOptionsMessageAction action)
        {
            var href = action.Href?.ToHtmlString();
            var type = action.Type?.ToHtmlString();

            // If href is empty or type is "button", use the button component
            if (string.IsNullOrEmpty(href) || type == "button")
            {
                var buttonOptions = new ButtonOptions
                {
                    Text = action.Text?.ToHtmlString(),
                    Type = action.Type ?? "button",
                    Name = action.Name,
                    Value = action.Value,
                    Classes = action.Classes,
                    Href = action.Href,
                    Attributes = action.Attributes
                };

                // Generate button by calling the existing button generator
                var buttonComponent = await GenerateButtonAsync(buttonOptions);
                
                // Extract the HtmlTag from the button component
                if (buttonComponent is HtmlTagGovUkComponent htmlTagComponent)
                {
                    return htmlTagComponent.Tag;
                }
                
                throw new InvalidOperationException("Button generation did not return an HtmlTag component.");
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
