using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a row in a GDS summary list component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListTagHelper.TagName)]
[RestrictChildren(SummaryListRowKeyTagHelper.TagName, SummaryListRowValueTagHelper.TagName, SummaryListRowActionsTagHelper.TagName)]
public class SummaryListRowTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var summaryListContext = context.GetContextItem<SummaryListContext>();
        var rowContext = new SummaryListRowContext();

        using (context.SetScopedContextItem(rowContext))
        {
            _ = await output.GetChildContentAsync();
        }

        rowContext.ThrowIfIncomplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        summaryListContext.AddRow(new SummaryListOptionsRow()
        {
            Classes = classes,
            Key = rowContext.Key,
            Value = rowContext.Value,
            Actions = rowContext.Actions,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
