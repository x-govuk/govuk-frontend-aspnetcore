using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a GDS breadcrumbs component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = BreadcrumbsTagHelper.TagName)]
//[HtmlTargetElement(ShortTagName, ParentTag = BreadcrumbsTagHelper.TagName)]
public class BreadcrumbsItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-breadcrumbs-item";
    //internal const string ShortTagName = ShortTagNames.Item;

    private const string LinkAttributesPrefix = "link-";

    /// <summary>
    /// Additional attributes for the generated <c>a</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LinkAttributesPrefix)]
    public IDictionary<string, string?> LinkAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var breadcrumbsContext = context.GetContextItem<BreadcrumbsContext>();

        var content = (await output.GetChildContentAsync()).Snapshot();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        breadcrumbsContext.AddItem(new BreadcrumbsOptionsItem()
        {
            ItemAttributes = attributes,
            Href = href,
            Attributes = new AttributeCollection(LinkAttributes),
            Html = content.ToTemplateString()
        });

        output.SuppressOutput();
    }
}
