using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async Task<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var buttonContent = CreateButtonContent(options);

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

        var buttonComponent = await GenerateButtonAsync(buttonOptions);
        var buttonHtml = buttonComponent.GetHtml();

        var container = new HtmlTag("div", attrs =>
        {
            attrs
                .With("id", options.Id)
                .WithClasses("govuk-exit-this-page", options.Classes)
                .With("data-module", "govuk-exit-this-page")
                .With("data-i18n.activated", options.ActivatedText)
                .With("data-i18n.timed-out", options.TimedOutText)
                .With("data-i18n.press-two-more-times", options.PressTwoMoreTimesText)
                .With("data-i18n.press-one-more-time", options.PressOneMoreTimeText)
                .With(options.Attributes);
        });

        container.InnerHtml.AppendHtml(buttonHtml);

        return await GenerateFromHtmlTagAsync(container);

        static TemplateString CreateButtonContent(ExitThisPageOptions options)
        {
            if (options.Html?.IsEmpty() == false)
            {
                return options.Html.Value;
            }

            if (options.Text?.IsEmpty() == false)
            {
                return options.Text.Value;
            }

            return ((TemplateString?)new HtmlString("<span class=\"govuk-visually-hidden\">Emergency</span> Exit this page")).Value;
        }
    }
}
