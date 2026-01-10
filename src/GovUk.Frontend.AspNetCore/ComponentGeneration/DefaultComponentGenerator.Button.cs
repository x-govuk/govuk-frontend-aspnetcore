using Microsoft.AspNetCore.Html;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateButtonAsync(ButtonOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Determine the element type
        var element = DetermineButtonElement(options);

        HtmlTag tag;

        if (element == "a")
        {
            tag = CreateLinkButton(options);
        }
        else if (element == "input")
        {
            tag = CreateInputButton(options);
        }
        else // button
        {
            tag = CreateButtonElement(options);
        }

        return GenerateFromHtmlTagAsync(tag);
    }

    private static string DetermineButtonElement(ButtonOptions options)
    {
        if (options.Element is not null)
        {
#pragma warning disable CA1308 // Element names should be lowercase
            return options.Element.ToHtmlString(raw: true).ToLower(System.Globalization.CultureInfo.InvariantCulture);
#pragma warning restore CA1308
        }

        if (options.Href is not null)
        {
            return "a";
        }

        return "button";
    }

    private HtmlTag CreateLinkButton(ButtonOptions options)
    {
        var tag = new HtmlTag("a", attrs => attrs
            .With("href", options.Href ?? "#")
            .With("role", "button")
            .With("draggable", "false")
            .WithClasses("govuk-button", options.IsStartButton == true ? "govuk-button--start" : null, options.Classes)
            .With("data-module", "govuk-button")
            .With("id", options.Id)
            .With(options.Attributes));

        AppendButtonContent(tag, options);

        return tag;
    }

    private HtmlTag CreateButtonElement(ButtonOptions options)
    {
        var tag = new HtmlTag("button", attrs =>
        {
            attrs
                .With("value", options.Value)
                .With("type", options.Type ?? "submit")
                .With("name", options.Name);

            if (options.Disabled == true)
            {
                attrs.WithBoolean("disabled");
                attrs.With("aria-disabled", "true");
            }

            if (options.PreventDoubleClick == true || options.PreventDoubleClick == false)
            {
                attrs.With("data-prevent-double-click", options.PreventDoubleClick.Value ? "true" : "false");
            }

            attrs
                .WithClasses("govuk-button", options.IsStartButton == true ? "govuk-button--start" : null, options.Classes)
                .With("data-module", "govuk-button")
                .With("id", options.Id)
                .With(options.Attributes);
        });

        AppendButtonContent(tag, options);

        return tag;
    }

    private HtmlTag CreateInputButton(ButtonOptions options)
    {
        var tag = new HtmlTag("input", attrs =>
        {
            attrs
                .With("value", options.Text)
                .With("type", options.Type ?? "submit")
                .With("name", options.Name);

            if (options.Disabled == true)
            {
                attrs.WithBoolean("disabled");
                attrs.With("aria-disabled", "true");
            }

            if (options.PreventDoubleClick == true || options.PreventDoubleClick == false)
            {
                attrs.With("data-prevent-double-click", options.PreventDoubleClick.Value ? "true" : "false");
            }

            attrs
                .WithClasses("govuk-button", options.IsStartButton == true ? "govuk-button--start" : null, options.Classes)
                .With("data-module", "govuk-button")
                .With("id", options.Id)
                .With(options.Attributes);
        });

        tag.TagRenderMode = Microsoft.AspNetCore.Mvc.Rendering.TagRenderMode.SelfClosing;

        return tag;
    }

    private void AppendButtonContent(HtmlTag tag, ButtonOptions options)
    {
        // Add the button text or HTML
        tag.InnerHtml.AppendHtml(HtmlOrText(options.Html, options.Text));

        // Add the start icon if needed
        if (options.IsStartButton == true)
        {
            tag.InnerHtml.AppendHtml(CreateStartIcon());
        }
    }

    private static HtmlTag CreateStartIcon()
    {
        // The SVG needs focusable="false" so that Internet Explorer does not
        // treat it as an interactive element - without this it will be
        // 'focusable' when using the keyboard to navigate.
        var svg = new HtmlTag("svg", attrs => attrs
            .WithClasses("govuk-button__start-icon")
            .With("xmlns", "http://www.w3.org/2000/svg")
            .With("width", "17.5")
            .With("height", "19")
            .With("viewBox", "0 0 33 40")
            .With("aria-hidden", "true")
            .With("focusable", "false"));

        var path = new HtmlTag("path", attrs => attrs
            .With("fill", "currentColor")
            .With("d", "M0 0h13l20 20-20 20H0l20-20z"));
        
        path.TagRenderMode = Microsoft.AspNetCore.Mvc.Rendering.TagRenderMode.SelfClosing;

        svg.InnerHtml.AppendHtml(path);

        return svg;
    }
}
