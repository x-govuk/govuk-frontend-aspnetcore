using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an action in a GDS summary card.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryCardActionsTagHelper.TagName)]
public class SummaryCardActionTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-card-action";

    private const string VisuallyHiddenTextAttributeName = "visually-hidden-text";

    /// <summary>
    /// The visually hidden text for the action link.
    /// </summary>
    [HtmlAttributeName(VisuallyHiddenTextAttributeName)]
    public string? VisuallyHiddenText { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var actionsContext = context.GetContextItem<SummaryCardActionsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);
        attributes.Remove("href", out _);
        var href = output.GetUrlAttribute("href");

        actionsContext.AddItem(new SummaryListOptionsCardActionsItem
        {
            Href = href,
            Text = null,
            Html = content.ToTemplateString(),
            VisuallyHiddenText = VisuallyHiddenText,
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
