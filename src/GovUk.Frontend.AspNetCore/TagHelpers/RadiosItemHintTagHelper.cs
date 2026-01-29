using GovUk.Frontend.AspNetCore.ComponentGeneration;
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

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var hintOptions = new HintOptions
        {
            Classes = classes,
            Attributes = attributes,
            Html = content.ToTemplateString()
        };

        itemContext.SetHint(hintOptions, output.TagName);

        output.SuppressOutput();
    }
}
