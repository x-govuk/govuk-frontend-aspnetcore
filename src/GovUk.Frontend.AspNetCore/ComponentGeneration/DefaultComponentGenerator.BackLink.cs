namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateBackLinkAsync(BackLinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tag = new HtmlTag("a", attrs => attrs
            .With("href", options.Href ?? "#")
            .WithClasses("govuk-back-link", options.Classes)
            .With(options.Attributes))
        {
            HtmlOrText(options.Html, options.Text, fallback: "Back")
        };

        return GenerateFromHtmlTagAsync(tag);
    }
}
