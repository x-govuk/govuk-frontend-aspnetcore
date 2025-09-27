using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the hint of a radios item in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosItemTagHelper.TagName)]
public class RadiosItemHintTagHelper : TagHelper
{
    internal const string TagName = "govuk-radios-item-hint";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var itemContext = context.GetContextItem<RadiosItemContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        itemContext.SetHint(output.Attributes.ToAttributeDictionary(), content.Snapshot());

        output.SuppressOutput();
    }
}
