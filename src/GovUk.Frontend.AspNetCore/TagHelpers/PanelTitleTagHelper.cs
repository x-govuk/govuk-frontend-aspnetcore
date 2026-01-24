using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title in a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PanelTagHelper.TagName)]
public class PanelTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-title";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var panelContext = context.GetContextItem<PanelContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        panelContext.SetTitle(content.ToTemplateString(), attributes);

        output.SuppressOutput();
    }
}
