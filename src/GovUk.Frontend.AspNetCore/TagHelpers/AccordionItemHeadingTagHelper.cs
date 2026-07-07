using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the heading in a GDS accordion component item.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = AccordionItemTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML of the header for each section which is used both as the title for each section, and as the button to open or close each section.")]
public class AccordionItemHeadingTagHelper : TagHelper
{
    internal const string TagName = "govuk-accordion-item-heading";

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

        itemContext.SetHeading(attributes, content.ToTemplateString());

        output.SuppressOutput();
    }
}
