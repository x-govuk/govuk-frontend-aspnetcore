using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of an item in a GDS form component.
/// </summary>
[HtmlTargetElement(CheckboxesTagName, ParentTag = CheckboxesItemTagHelper.TagName)]
[HtmlTargetElement(RadiosTagName, ParentTag = RadiosItemTagHelper.TagName)]
// The checkboxes and radios items share the one short name, so this covers the item in either
// component; the item's context decides which component's conditional is being set
[HtmlTargetElement(ShortTagName, ParentTag = ShortTagNames.Item)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the conditional reveal for the item.")]
public class FormGroupItemConditionalTagHelper : TagHelper
{
    internal const string CheckboxesTagName = "govuk-checkboxes-item-conditional";
    internal const string RadiosTagName = "govuk-radios-item-conditional";
    private const string ShortTagName = ShortTagNames.Conditional;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var itemContext = context.GetContextItem<FormGroupItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        itemContext.SetConditional(
            new AttributeCollection(output.Attributes),
            content.Snapshot(),
            context.TagName);

        output.SuppressOutput();
    }
}
