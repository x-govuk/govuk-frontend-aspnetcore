using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions in a GDS panel component.
/// </summary>
/// <remarks>
/// Actions are only rendered for the interruption variant of the panel (when the
/// <c>govuk-panel--interruption</c> class is applied).
/// </remarks>
[HtmlTargetElement(TagName, ParentTag = PanelTagHelper.TagName)]
[RestrictChildren(PanelActionTagHelper.TagName, PanelActionLinkTagHelper.TagName)]
public class PanelActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-panel-actions";

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new PanelActionsContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var panelContext = context.GetContextItem<PanelContext>();
        var actionsContext = context.GetContextItem<PanelActionsContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        panelContext.SetActions(new PanelActionsOptions
        {
            Items = actionsContext.Actions,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
