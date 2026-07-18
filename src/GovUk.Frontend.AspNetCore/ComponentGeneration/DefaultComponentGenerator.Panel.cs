using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual async ValueTask<GovUkComponent> GeneratePanelAsync(PanelOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var headingLevel = options.HeadingLevel ?? 1;
        var titleContent = HtmlOrText(options.TitleHtml, options.TitleText);

        // The panel defaults to the confirmation variant unless the interruption variant is requested.
        var isInterruption = ClassesContain(options.Classes, "govuk-panel--interruption");

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-panel", isInterruption ? null : "govuk-panel--confirmation", options.Classes)
            .With(options.Attributes));

        var headingTag = new HtmlTag($"h{headingLevel}", attrs => attrs
            .WithClasses("govuk-panel__title")
            .With(options.TitleAttributes));
        headingTag.InnerHtml.AppendHtml(titleContent);
        outerTag.InnerHtml.AppendHtml(headingTag);

        if (!options.Html.IsEmpty() || !options.Text.IsEmpty())
        {
            var bodyContent = HtmlOrText(options.Html, options.Text);
            var bodyTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-panel__body")
                .With(options.BodyAttributes));
            bodyTag.InnerHtml.AppendHtml(bodyContent);
            outerTag.InnerHtml.AppendHtml(bodyTag);
        }

        // Actions are only rendered for the interruption variant.
        if (isInterruption && options.Actions is not null)
        {
            var actionsTag = new HtmlTag("div", attrs => attrs
                .WithClasses("govuk-panel__actions", options.Actions.Classes)
                .With(options.Actions.Attributes));

            if (options.Actions.Items is { Count: > 0 } items)
            {
                var buttonGroup = new HtmlTag("div", attrs => attrs.WithClasses("govuk-button-group"));

                foreach (var action in items)
                {
                    buttonGroup.InnerHtml.AppendHtml(await CreatePanelActionAsync(action));
                }

                actionsTag.InnerHtml.AppendHtml(buttonGroup);
            }

            outerTag.InnerHtml.AppendHtml(actionsTag);
        }

        return await GenerateFromHtmlTagAsync(outerTag);

        async ValueTask<IHtmlContent> CreatePanelActionAsync(PanelActionsItemOptions action)
        {
            // Render a button (which is itself a link-button when a href is set) when the action has
            // no href or explicitly asks for a button; otherwise render an inverse link.
            if (action.Href.IsEmpty() || action.Type == "button")
            {
                return await GenerateButtonAsync(new ButtonOptions
                {
                    Html = action.Html,
                    Text = action.Text,
                    Type = action.Type ?? "button",
                    Classes = new TemplateString("govuk-button--inverse").AppendCssClasses(action.Classes),
                    Href = action.Href,
                    Attributes = action.Attributes
                });
            }

            var linkTag = new HtmlTag("a", attrs => attrs
                .WithClasses("govuk-link", "govuk-link--inverse", action.Classes)
                .With("href", action.Href)
                .With(action.Attributes));
            linkTag.InnerHtml.AppendHtml(HtmlOrText(action.Html, action.Text));
            return linkTag;
        }
    }
}
