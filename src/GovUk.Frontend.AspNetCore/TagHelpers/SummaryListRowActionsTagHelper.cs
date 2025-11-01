using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions wrapper in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListRowTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.ShortTagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListRowTagHelper.ShortTagName)]
[RestrictChildren(
    SummaryListRowActionTagHelper.TagName,
    SummaryListRowActionTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The container element for the row's actions.")]
public class SummaryListRowActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-actions";
    internal const string ShortTagName = ShortTagNames.RowActions;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var rowContext = context.GetContextItem<SummaryListRowContext>();
        var actionsContext = new SummaryListRowActionsContext();

        using (context.SetScopedContextItem(actionsContext))
        {
            _ = await output.GetChildContentAsync();
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        rowContext.SetActions(new SummaryListOptionsRowActions()
        {
            Classes = classes,
            Items = actionsContext.Items,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
