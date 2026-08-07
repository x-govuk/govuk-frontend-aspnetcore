using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the body in a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PanelTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = PanelTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the panel body.")]
public class PanelBodyTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-body";
    internal const string ShortTagName = ShortTagNames.PanelBody;

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

        panelContext.SetBody(content.Snapshot(), attributes);

        output.SuppressOutput();
    }
}
