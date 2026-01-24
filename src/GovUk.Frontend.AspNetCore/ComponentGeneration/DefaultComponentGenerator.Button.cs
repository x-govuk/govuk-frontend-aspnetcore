using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateButtonAsync(ButtonOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var element = TemplateString.Coalesce(options.Element, !options.Href.IsEmpty() ? "a" : "button");

        var tag = element == "a" ? CreateLinkButton() : element == "input" ? CreateInputButton() : CreateButtonElement();

        return GenerateFromHtmlTagAsync(tag);

        HtmlTag CreateLinkButton()
        {
            var tag = new HtmlTag("a", attrs =>
            {
                attrs
                    .With("href", options.Href ?? "#")
                    .With("role", "button")
                    .With("draggable", "false");

                AddCommonButtonAttributes(attrs);
            });

            AppendButtonContent(tag);

            return tag;
        }

        HtmlTag CreateButtonElement()
        {
            var tag = new HtmlTag("button", attrs =>
            {
                attrs
                    .With("value", options.Value)
                    .With("type", options.Type ?? "submit");

                AddButtonSpecificAttributes(attrs);
                AddCommonButtonAttributes(attrs);
            });

            AppendButtonContent(tag);

            return tag;
        }

        HtmlTag CreateInputButton()
        {
            var tag = new HtmlTag("input", attrs =>
            {
                attrs
                    .With("value", options.Text)
                    .With("type", options.Type ?? "submit");

                AddButtonSpecificAttributes(attrs);
                AddCommonButtonAttributes(attrs);
            });

            tag.TagRenderMode = TagRenderMode.SelfClosing;

            return tag;
        }

        void AddButtonSpecificAttributes(HtmlTag.AttributeBuilder attrs)
        {
            attrs.With("name", options.Name);

            if (options.Disabled == true)
            {
                attrs
                    .WithBoolean("disabled")
                    .With("aria-disabled", "true");
            }

            if (options.PreventDoubleClick is { } preventDoubleClick)
            {
                attrs.With("data-prevent-double-click", preventDoubleClick ? "true" : "false");
            }
        }

        void AddCommonButtonAttributes(HtmlTag.AttributeBuilder attrs)
        {
            attrs
                .WithClasses("govuk-button", options.IsStartButton is true ? "govuk-button--start" : null, options.Classes)
                .With("data-module", "govuk-button")
                .With("id", options.Id)
                .With(options.Attributes);
        }

        void AppendButtonContent(HtmlTag tag)
        {
            tag.InnerHtml.AppendHtml(HtmlOrText(options.Html, options.Text));

            if (options.IsStartButton == true)
            {
                tag.InnerHtml.AppendHtml(CreateStartIcon());
            }
        }

        HtmlTag CreateStartIcon()
        {
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

            path.TagRenderMode = TagRenderMode.SelfClosing;

            svg.InnerHtml.AppendHtml(path);

            return svg;
        }
    }
}
