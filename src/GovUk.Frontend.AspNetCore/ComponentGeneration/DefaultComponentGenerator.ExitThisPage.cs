using GovUk.Frontend.AspNetCore.Localization;
using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var buttonContent = CreateButtonContent();

        var buttonOptions = new ButtonOptions
        {
            Html = buttonContent,
            Classes = "govuk-button--warning govuk-exit-this-page__button govuk-js-exit-this-page-button",
            Href = options.RedirectUrl ?? "https://www.bbc.co.uk/weather",
            Attributes = new AttributeCollection
            {
                { "rel", "nofollow noreferrer" }
            }
        };

        var button = await GenerateButtonAsync(buttonOptions);

        var container = new HtmlTag("div", attrs =>
        {
            attrs
                .With("id", options.Id)
                .WithClasses("govuk-exit-this-page", options.Classes)
                .With("data-module", "govuk-exit-this-page")
                .With("data-i18n.activated", options.ActivatedText ?? LocalizedText(GovUkFrontendResourceNames.ExitThisPageActivatedText))
                .With("data-i18n.timed-out", options.TimedOutText ?? LocalizedText(GovUkFrontendResourceNames.ExitThisPageTimedOutText))
                .With("data-i18n.press-two-more-times", options.PressTwoMoreTimesText ?? LocalizedText(GovUkFrontendResourceNames.ExitThisPagePressTwoMoreTimesText))
                .With("data-i18n.press-one-more-time", options.PressOneMoreTimeText ?? LocalizedText(GovUkFrontendResourceNames.ExitThisPagePressOneMoreTimeText))
                .With(options.Attributes);
        })
        {
            button
        };

        return await GenerateFromHtmlTagAsync(container);

        IHtmlContent CreateButtonContent()
        {
            if (!options.Html.IsEmpty())
            {
                return options.Html;
            }

            if (!options.Text.IsEmpty())
            {
                return new TemplateString(options.Text);
            }

            var visuallyHiddenText = LocalizedText(GovUkFrontendResourceNames.ExitThisPageVisuallyHiddenText) ?? "Emergency";
            var text = LocalizedText(GovUkFrontendResourceNames.ExitThisPageText) ?? "Exit this page";

            var visuallyHiddenTag = new HtmlTag("span", attrs => attrs.WithClasses("govuk-visually-hidden"))
            {
                visuallyHiddenText
            };

            return new TemplateString($"{visuallyHiddenTag} {text}");
        }
    }
}
