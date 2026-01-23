namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateTagAsync(TagOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tag = new HtmlTag("strong", attrs => attrs
            .WithClasses("govuk-tag", options.Classes)
            .With(options.Attributes))
        {
            HtmlOrText(options.Html, options.Text)
        };

        return GenerateFromHtmlTagAsync(tag);
    }
}
