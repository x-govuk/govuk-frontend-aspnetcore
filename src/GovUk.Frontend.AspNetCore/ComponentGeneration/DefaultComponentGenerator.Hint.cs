namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateHintAsync(HintOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var tag = new HtmlTag("div", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-hint", options.Classes)
            .With(options.Attributes))
        {
            HtmlOrText(options.Html, options.Text)
        };

        return GenerateFromHtmlTagAsync(tag);
    }
}
