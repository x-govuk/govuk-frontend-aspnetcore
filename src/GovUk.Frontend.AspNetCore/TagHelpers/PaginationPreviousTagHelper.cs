using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the link to the previous page in a GDS pagination component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PaginationTagHelper.TagName)]
public class PaginationPreviousTagHelper : TagHelper
{
    internal const string TagName = "govuk-pagination-previous";

    private const string LabelTextAttributeName = "label-text";
    private const string LinkAttributesPrefix = "link-";

    /// <summary>
    /// The optional label that goes underneath the link to the previous page, providing further context for the user about where the link goes.
    /// </summary>
    [HtmlAttributeName(LabelTextAttributeName)]
    public string? LabelText { get; set; }

    /// <summary>
    /// Additional attributes to add to the generated <c>a</c> element.
    /// </summary>
    [HtmlAttributeName(DictionaryAttributePrefix = LinkAttributesPrefix)]
    public IDictionary<string, string?> LinkAttributes { get; set; } = new Dictionary<string, string?>();

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var paginationContext = context.GetContextItem<PaginationContext>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        paginationContext.SetPrevious(new PaginationOptionsPrevious()
        {
            Attributes = new AttributeCollection(LinkAttributes),
            ContainerAttributes = attributes,
            Href = href,
            LabelText = LabelText,
            Html = content?.ToTemplateString(),
            Text = null
        });

        output.SuppressOutput();
    }
}
