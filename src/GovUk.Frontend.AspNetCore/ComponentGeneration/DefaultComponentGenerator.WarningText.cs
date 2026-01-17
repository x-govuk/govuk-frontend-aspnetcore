namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateWarningTextAsync(WarningTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var iconFallbackText = options.IconFallbackText?.ToHtmlString() ?? "Warning";
        var content = HtmlOrText(options.Html, options.Text);

        var outerTag = new HtmlTag("div", attrs => attrs
            .WithClasses("govuk-warning-text", options.Classes)
            .With(options.Attributes));

        var iconTag = new HtmlTag("span", attrs => attrs
            .WithClasses("govuk-warning-text__icon")
            .With("aria-hidden", "true"))
        {
            "!"
        };
        outerTag.InnerHtml.AppendHtml(iconTag);

        var textTag = new HtmlTag("strong", attrs => attrs
            .WithClasses("govuk-warning-text__text"));

        if (!string.IsNullOrEmpty(iconFallbackText))
        {
            var visuallyHiddenTag = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-visually-hidden"))
            {
                iconFallbackText
            };
            textTag.InnerHtml.AppendHtml(visuallyHiddenTag);
            textTag.InnerHtml.Append(" ");
        }

        textTag.InnerHtml.AppendHtml(content);
        outerTag.InnerHtml.AppendHtml(textTag);

        return GenerateFromHtmlTagAsync(outerTag);
    }
}
