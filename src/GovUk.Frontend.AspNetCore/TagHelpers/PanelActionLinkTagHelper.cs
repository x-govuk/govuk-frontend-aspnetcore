using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a link action in a GDS panel component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PanelActionsTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = PanelActionsTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the link.")]
public class PanelActionLinkTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-action-link";
    internal const string ShortTagName = ShortTagNames.ActionLink;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var actionsContext = context.GetContextItem<PanelActionsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        actionsContext.Actions.Add(new PanelActionsItemOptions
        {
            Html = content.Snapshot(),
            Href = href,
            Type = null,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
