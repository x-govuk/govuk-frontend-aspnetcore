using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint of a checkbox item in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesItemTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = CheckboxesItemTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the item's hint.")]
public class CheckboxesItemHintTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-item-hint";
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
