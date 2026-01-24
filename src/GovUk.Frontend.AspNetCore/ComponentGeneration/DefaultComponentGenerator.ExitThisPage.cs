namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options)
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

        var button = await GenerateButtonAsync(buttonOptions);

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
        })
        {
            button
        };

        return await GenerateFromHtmlTagAsync(container);

        static TemplateString CreateButtonContent(ExitThisPageOptions options)
        {
            if (!options.Html.IsEmpty())
            {
                return options.Html;
            }

            if (!options.Text.IsEmpty())
            {
                return options.Text;
            }

            return TemplateString.FromEncoded("<span class=\"govuk-visually-hidden\">Emergency</span> Exit this page");
        }
    }
}
