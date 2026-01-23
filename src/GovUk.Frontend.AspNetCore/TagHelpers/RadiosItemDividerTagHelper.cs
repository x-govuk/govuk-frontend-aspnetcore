using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the divider text to separate items in a GDS radios component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
public class RadiosItemDividerTagHelper : TagHelper
{
    internal const string TagName = "govuk-radios-divider";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var radiosContext = context.GetContextItem<RadiosContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        radiosContext.AddItem(new RadiosItemDivider
        {
            Attributes = output.Attributes.ToAttributeDictionary(),
            Content = content.Snapshot()
        });

        output.SuppressOutput();
    }
}
