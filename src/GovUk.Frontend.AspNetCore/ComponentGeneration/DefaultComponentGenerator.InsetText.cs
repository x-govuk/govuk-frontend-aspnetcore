namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual ValueTask<GovUkComponent> GenerateInsetTextAsync(InsetTextOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var content = HtmlOrText(options.Html, options.Text);

        var tag = new HtmlTag("div", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-inset-text", options.Classes)
            .With(options.Attributes));

        tag.InnerHtml.AppendHtml(content);

        return GenerateFromHtmlTagAsync(tag);
    }
}
