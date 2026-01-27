using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in the meta section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterMetaItemsTagHelper.TagName)]
public class FooterMetaItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-meta-item";

    private const string LinkAttributesPrefix = "link-";

    /// <summary>
    /// Additional attributes to add to the generated <c>&lt;a&gt;</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LinkAttributesPrefix)]
    public IDictionary<string, string?> LinkAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var metaItemsContext = context.GetContextItem<FooterMetaItemsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        metaItemsContext.Items.Add(new FooterOptionsMetaItem
        {
            Text = null,
            Html = content.ToTemplateString(),
            Href = href,
            Attributes = new(LinkAttributes),
            ItemAttributes = attributes
        });

        output.SuppressOutput();
    }
}
