using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public abstract class GovUkComponentView<TModel> : RazorPage<TModel>
{
    protected IHtmlContent? Classes(params object?[] classNames)
    {
        if (classNames.Length == 0)
        {
            return null;
        }

        var builder = new HtmlContentBuilder();
        var haveWrittenClass = false;

        foreach (var part in classNames)
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
                haveWrittenClass = true;
            }
            else if (part is IHtmlContent htmlContent)
            {
                WriteSpaceIfNeeded();
                builder.AppendHtml(htmlContent);
                haveWrittenClass = true;
            }
            else
            {
                throw new NotSupportedException($"Arguments of type '{part.GetType().FullName}' are not supported.");
            }
        }

        return builder;

        void WriteSpaceIfNeeded()
        {
            if (haveWrittenClass)
            {
                builder.Append(" ");
            }
        }
    }

    protected IHtmlContent? HtmlOrText(TemplateString? html, string? text, string? fallback = null)
    {
        if (html is not null)
        {
            var value = html.ToHtmlString(HtmlEncoder, raw: true);

            if (!string.IsNullOrEmpty(value))
            {
                return new HtmlString(value);
            }
        }

        if (!string.IsNullOrEmpty(text))
        {
            return new HtmlString(HtmlEncoder.Encode(text));
        }

        if (fallback is not null)
        {
            return new HtmlString(HtmlEncoder.Encode(fallback));
        }

        return null;
    }
}
