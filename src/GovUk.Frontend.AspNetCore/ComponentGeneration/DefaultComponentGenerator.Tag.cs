namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

internal partial class DefaultComponentGenerator
{
    public virtual Task<GovUkComponent> GenerateTagAsync(TagOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        string GetStrippedContent()
        {
            if (options.Html?.IsEmpty() == false)
            {
                return options.Html.ToHtmlString(raw: true).Trim();
            }

            if (options.Text?.IsEmpty() == false)
            {
                return options.Text.ToHtmlString();
            }

            return string.Empty;
        }

        var content = GetStrippedContent();

        var tag = new HtmlTag("strong", attrs => attrs
            .WithClasses("govuk-tag", options.Classes)
            .With(options.Attributes));

        tag.InnerHtml.AppendHtml("\n  ");
        tag.InnerHtml.AppendHtml(content);
        tag.InnerHtml.AppendHtml("\n");

        return GenerateFromHtmlTagAsync(tag);
    }
}
