namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateSkipLinkAsync(SkipLinkOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tag = new HtmlTag("a", attrs => attrs
            .With("href", options.Href ?? "#content")
            .WithClasses("govuk-skip-link", options.Classes)
            .With("data-module", "govuk-skip-link")
            .With(options.Attributes))
        {
            HtmlOrText(options.Html, options.Text)
        };

        return GenerateFromHtmlTagAsync(tag);
    }
}
