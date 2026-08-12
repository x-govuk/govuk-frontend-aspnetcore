using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the content in a GDS accordion component item.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = AccordionItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = AccordionItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML of the section, which is hidden when the section is closed.")]
public class AccordionItemContentTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion-item-content";
    internal const string ShortTagName = ShortTagNames.Content;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var itemContext = context.GetContextItem<AccordionItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        itemContext.SetContent(attributes, content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
