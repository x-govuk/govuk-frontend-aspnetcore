namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateErrorMessageAsync(ErrorMessageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var visuallyHiddenText = options.VisuallyHiddenText?.ToHtmlString() ?? DefaultErrorMessageVisuallyHiddenText;
        var content = HtmlOrText(options.Html, options.Text);

        var tag = new HtmlTag("p", attrs => attrs
            .With("id", options.Id)
            .WithClasses("govuk-error-message", options.Classes)
            .With(options.Attributes));

        if (!string.IsNullOrEmpty(visuallyHiddenText))
        {
            var vht = new HtmlTag("span", attrs => attrs
                .WithClasses("govuk-visually-hidden"))
            {
                visuallyHiddenText + ":"
            };

            tag.InnerHtml.AppendHtml(vht);
            tag.InnerHtml.Append(" ");
        }

        tag.InnerHtml.AppendHtml(content);

        return GenerateFromHtmlTagAsync(tag);
    }
}
