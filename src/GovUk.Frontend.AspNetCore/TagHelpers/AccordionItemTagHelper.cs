using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an item in a GDS accordion component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = AccordionTagHelper.TagName)]
[RestrictChildren(AccordionItemHeadingTagHelper.TagName, AccordionItemSummaryTagHelper.TagName, AccordionItemContentTagHelper.TagName)]
public class AccordionItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion-item";

    private const string ExpandedAttributeName = "expanded";

    /// <summary>
    /// Whether the section should be expanded upon initial load.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(ExpandedAttributeName)]
    public bool? Expanded { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var accordionContext = context.GetContextItem<AccordionContext>();

        var itemContext = new AccordionItemContext();

        using (context.SetScopedContextItem(itemContext))
        {
            _ = await output.GetChildContentAsync();
        }

        itemContext.ThrowIfIncomplete();

        accordionContext.AddItem(new AccordionOptionsItem()
        {
            Expanded = Expanded ?? false,
            Heading = new AccordionOptionsItemHeading()
            {
                Html = itemContext.Heading!.Value.Content.ToTemplateString()
            },
            Summary = itemContext.Summary != null ? new AccordionOptionsItemSummary()
            {
                Html = itemContext.Summary.Value.Content.ToTemplateString()
            } : null,
            Content = new AccordionOptionsItemContent()
            {
                Html = itemContext.Content!.Value.Content.ToTemplateString()
            }
        });

        output.SuppressOutput();
    }
}
