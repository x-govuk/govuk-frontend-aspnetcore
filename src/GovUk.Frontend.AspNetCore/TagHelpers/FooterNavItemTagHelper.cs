using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a navigation section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterNavItemsTagHelper.TagName)]
public class FooterNavItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-nav-item";

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

        var navItemsContext = context.GetContextItem<FooterNavItemsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        var href = output.GetUrlAttribute("href");
        attributes.Remove("href", out _);

        navItemsContext.Items.Add(new FooterOptionsNavigationItem
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
