using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class GovUkComponentView<TModel> : RazorPage<TModel>
{
    protected IHtmlContent? Classes(params object?[] values) => CreateSpaceSeparatedAttributeValue(values);

    protected IHtmlContent? DescribedBy(params object?[] values) => CreateSpaceSeparatedAttributeValue(values);

    protected IHtmlContent? HtmlOrText(TemplateString? html, TemplateString? text, string? fallback = null)
    {
        if (html is not null)
        {
            var value = html.ToHtmlString(HtmlEncoder, raw: true);

            if (!string.IsNullOrEmpty(value))
            {
                return new HtmlString(value);
            }
        }

        if (text is not null)
        {
            var value = text.ToHtmlString(HtmlEncoder);

            if (!string.IsNullOrEmpty(value))
            {
                return new HtmlString(value);
            }
        }

        if (fallback is not null)
        {
            return new HtmlString(HtmlEncoder.Encode(fallback));
        }

        return null;
    }

    private HtmlContentBuilder? CreateSpaceSeparatedAttributeValue(params object?[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length == 0)
        {
            return null;
        }

        var builder = new HtmlContentBuilder();
        var haveWrittenValue = false;

        foreach (var part in values)
        {
            if (part is null)
            {
                continue;
            }

            if (part is string str)
            {
                if (string.IsNullOrEmpty(str))
                {
                    continue;
                }

                WriteSpaceIfNeeded();
                builder.Append(str);
                haveWrittenValue = true;
            }
            else if (part is IHtmlContent htmlContent)
            {
                WriteSpaceIfNeeded();
                builder.AppendHtml(htmlContent);
                haveWrittenValue = true;
            }
            else if (part is TemplateString templateString)
            {
                WriteSpaceIfNeeded();
                builder.AppendHtml(templateString.ToHtmlString(HtmlEncoder, raw: false));
                haveWrittenValue = true;
            }
            else
            {
                throw new NotSupportedException($"Arguments of type '{part.GetType().FullName}' are not supported.");
            }
        }

        return haveWrittenValue ? builder : null;

        void WriteSpaceIfNeeded()
        {
            if (haveWrittenValue)
            {
                builder.Append(" ");
            }
        }
    }
}
