using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Frontend.AspNetCore.HtmlGeneration;

internal partial class ComponentGenerator
{
    internal const string ErrorSummaryDefaultTitle = "There is a problem";
    internal const string ErrorSummaryElement = "div";

    public TagBuilder GenerateErrorSummary(
        bool? disableAutoFocus,
        IHtmlContent titleContent,
        AttributeDictionary titleAttributes,
        IHtmlContent descriptionContent,
        AttributeDictionary descriptionAttributes,
        AttributeDictionary attributes,
        IEnumerable<ErrorSummaryItem> items)
    {
        Guard.ArgumentNotNull(nameof(items), items);

        var tagBuilder = new TagBuilder(ErrorSummaryElement);
        tagBuilder.MergeAttributes(attributes);
        tagBuilder.MergeCssClass("govuk-error-summary");
        tagBuilder.Attributes.Add("data-module", "govuk-error-summary");

        if (disableAutoFocus.HasValue)
        {
            tagBuilder.Attributes.Add("data-disable-auto-focus", disableAutoFocus.Value ? "true" : "false");
        }

        var alert = new TagBuilder("div");
        alert.Attributes.Add("role", "alert");

        var heading = new TagBuilder("h2");
        heading.MergeAttributes(titleAttributes);
        heading.MergeCssClass("govuk-error-summary__title");
        heading.InnerHtml.AppendHtml(titleContent);
        alert.InnerHtml.AppendHtml(heading);

        var body = new TagBuilder("div");
        body.MergeCssClass("govuk-error-summary__body");

        if (descriptionContent is not null)
        {
            var p = new TagBuilder("p");
            p.MergeAttributes(descriptionAttributes);
            p.InnerHtml.AppendHtml(descriptionContent);
            body.InnerHtml.AppendHtml(p);
        }

        var itemsArray = items.ToArray();
        if (itemsArray.Length > 0)
        {

            var ul = new TagBuilder("ul");
            ul.MergeCssClass("govuk-list");
            ul.MergeCssClass("govuk-error-summary__list");

            var itemIndex = 0;
            foreach (var item in itemsArray)
            {
                Guard.ArgumentValidNotNull(
                    nameof(items),
                    $"Item {itemIndex} is not valid; {nameof(ErrorSummaryItem.Content)} cannot be null.",
                    item.Content,
                    item.Content is not null);

                var li = new TagBuilder("li");
                li.MergeOptionalAttributes(item.Attributes);

                if (item.Href is not null)
                {
                    var a = new TagBuilder("a");
                    a.MergeOptionalAttributes(item.LinkAttributes);
                    a.MergeAttribute("href", item.Href);
                    a.InnerHtml.AppendHtml(item.Content);

                    li.InnerHtml.AppendHtml(a);
                }
                else
                {
                    li.InnerHtml.AppendHtml(item.Content);
                }

                ul.InnerHtml.AppendHtml(li);

                itemIndex++;
            }

            body.InnerHtml.AppendHtml(ul);
        }

        alert.InnerHtml.AppendHtml(body);

        tagBuilder.InnerHtml.AppendHtml(alert);

        return tagBuilder;
    }
}
