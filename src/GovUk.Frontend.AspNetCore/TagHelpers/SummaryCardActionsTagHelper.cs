using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions wrapper in a GDS summary card.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryCardTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryCardTagHelper.TagName)]
[RestrictChildren(
    SummaryCardActionTagHelper.TagName,
    SummaryCardActionTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The container element for the card's actions.")]
public class SummaryCardActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-card-actions";
    internal const string ShortTagName = ShortTagNames.CardActions;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var cardContext = context.GetContextItem<SummaryCardContext>();
        var actionsContext = new SummaryCardActionsContext();

        using (context.SetScopedContextItem(actionsContext))
        {
            _ = await output.GetChildContentAsync();
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        cardContext.SetActions(new SummaryListOptionsCardActions()
        {
            Classes = classes,
            Items = actionsContext.Items,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
