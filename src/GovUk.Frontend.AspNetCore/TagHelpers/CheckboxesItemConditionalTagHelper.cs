using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of a checkbox item in a GDS checkboxes component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesItemTagHelper.TagName)]
public class CheckboxesItemConditionalTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-item-conditional";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var itemContext = context.GetContextItem<CheckboxesItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        itemContext.SetConditional(output.Attributes.ToAttributeDictionary(), content.Snapshot());

        output.SuppressOutput();
    }
}
