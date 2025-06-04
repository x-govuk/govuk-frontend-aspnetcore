using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the conditional reveal of a radios item in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosItemTagHelper.TagName)]
public class RadiosItemConditionalTagHelper : TagHelper
{
    internal const string TagName = "govuk-radios-item-conditional";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var itemContext = context.GetContextItem<RadiosItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        itemContext.SetConditional(output.Attributes.ToAttributeDictionary(), content.Snapshot());

        output.SuppressOutput();
    }
}
