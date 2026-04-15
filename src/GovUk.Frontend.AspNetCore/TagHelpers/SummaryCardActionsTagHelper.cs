using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions wrapper in a GDS summary card.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryCardTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryCardTagHelper.TagName)]
[RestrictChildren(SummaryCardActionTagHelper.TagName, SummaryCardActionTagHelper.ShortTagName)]
public class SummaryCardActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-card-actions";
    internal const string ShortTagName = ShortTagNames.CardActions;

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new SummaryCardActionsContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var cardContext = context.GetContextItem<SummaryCardContext>();
        var actionsContext = context.GetContextItem<SummaryCardActionsContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        cardContext.SetActions(
            new SummaryListOptionsCardActions
            {
                Classes = classes,
                Items = actionsContext.Items,
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
