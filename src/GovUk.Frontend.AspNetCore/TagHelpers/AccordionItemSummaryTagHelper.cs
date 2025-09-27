using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the summary in a GDS accordion component item.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = AccordionItemTagHelper.TagName)]
public class AccordionItemSummaryTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion-item-summary";

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

        itemContext.SetSummary(output.Attributes.ToAttributeDictionary(), content.Snapshot());

        output.SuppressOutput();
    }
}
