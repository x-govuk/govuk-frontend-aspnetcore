using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the actions wrapper in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
[RestrictChildren(SummaryListRowActionTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.SummaryListRowActions)]
public class SummaryListRowActionsTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-actions";

    /// <summary>
    /// Creates a new <see cref="SummaryListRowActionsTagHelper"/>.
    /// </summary>
    public SummaryListRowActionsTagHelper()
    {
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var rowContext = context.GetContextItem<SummaryListRowContext>();
        var actionsContext = new SummaryListRowActionsContext();

        using (context.SetScopedContextItem(actionsContext))
        {
            await output.GetChildContentAsync();
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
