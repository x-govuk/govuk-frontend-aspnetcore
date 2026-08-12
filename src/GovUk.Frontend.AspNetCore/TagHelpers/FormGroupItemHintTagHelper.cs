using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint of an item in a GDS form component.
/// </summary>
[HtmlTargetElement(CheckboxesTagName, ParentTag = CheckboxesItemTagHelper.TagName)]
[HtmlTargetElement(RadiosTagName, ParentTag = RadiosItemTagHelper.TagName)]
// The checkboxes and radios items share the one short name for the hint; the item's context
// decides which component's hint is being set
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesItemTagHelper.ShortTagName)]
[HtmlTargetElement(ShortTagName, ParentTag = RadiosItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the item's hint.")]
public class FormGroupItemHintTagHelper : TagHelper
{
    internal const string CheckboxesTagName = "govuk-checkboxes-item-hint";
    internal const string RadiosTagName = "govuk-radios-item-hint";
    private const string ShortTagName = ShortTagNames.Hint;

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

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var hintOptions = new HintOptions
        {
            Classes = classes,
            Attributes = attributes,
            Html = content.Snapshot()
        };

        itemContext.SetHint(hintOptions, context.TagName);

        output.SuppressOutput();
    }
}
