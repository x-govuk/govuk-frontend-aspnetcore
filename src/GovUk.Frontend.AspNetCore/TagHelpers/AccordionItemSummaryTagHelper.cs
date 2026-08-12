using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the summary in a GDS accordion component item.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = AccordionItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = AccordionItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML for the summary line.")]
public class AccordionItemSummaryTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion-item-summary";
    internal const string ShortTagName = ShortTagNames.Summary;

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

        itemContext.SetSummary(attributes, content.Snapshot(), context.TagName);

        output.SuppressOutput();
    }
}
