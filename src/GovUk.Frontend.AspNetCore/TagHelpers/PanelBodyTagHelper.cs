using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the body in a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PanelTagHelper.TagName)]
public class PanelBodyTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-body";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var panelContext = context.GetContextItem<PanelContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        panelContext.SetBody(content.Snapshot());

        output.SuppressOutput();
    }
}
