using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions wrapper in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListRowTagHelper.ShortTagName)]
[RestrictChildren(SummaryListRowActionTagHelper.TagName, SummaryListRowActionTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The container element for the row's actions.")]
public class SummaryListRowActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-actions";
    internal const string ShortTagName = ShortTagNames.RowActions;

    /// <summary>
    /// Creates a new <see cref="SummaryListRowActionsTagHelper"/>.
    /// </summary>
    public SummaryListRowActionsTagHelper()
    {
    }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        context.SetContextItem(new SummaryListRowActionsContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var rowContext = context.GetContextItem<SummaryListRowContext>();
        var actionsContext = context.GetContextItem<SummaryListRowActionsContext>();

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        rowContext.SetActions(
            new SummaryListOptionsRowActions
            {
                Classes = classes,
                Items = actionsContext.Items,
                Attributes = attributes
            },
            context.TagName);

        output.SuppressOutput();
    }
}
