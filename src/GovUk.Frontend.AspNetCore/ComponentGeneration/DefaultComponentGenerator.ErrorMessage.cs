namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateErrorMessageAsync(ErrorMessageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var visuallyHiddenText = options.VisuallyHiddenText ?? "Error";
        var content = HtmlOrText(options.Html, options.Text);

        var tag = new HtmlTag("p", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-error-message", options.Classes)
            .With(options.Attributes));

        if (!visuallyHiddenText.IsEmpty())
        {
            var vhtTag = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-visually-hidden"))
            {
                new TemplateString($"{visuallyHiddenText}:")
            };

            tag.InnerHtml.AppendHtml(vhtTag);
            tag.InnerHtml.Append(" ");
        }

        tag.InnerHtml.AppendHtml(content);

        return GenerateFromHtmlTagAsync(tag);
    }
}
