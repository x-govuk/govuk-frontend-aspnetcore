using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a row in a GDS summary list component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListTagHelper.TagName)]
[RestrictChildren(
    SummaryListRowKeyTagHelper.TagName,
    SummaryListRowKeyTagHelper.ShortTagName,
    SummaryListRowValueTagHelper.TagName,
    SummaryListRowValueTagHelper.ShortTagName,
    SummaryListRowActionsTagHelper.TagName,
    SummaryListRowActionsTagHelper.ShortTagName)]
public class SummaryListRowTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row";
    internal const string ShortTagName = ShortTagNames.Row;

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new SummaryListRowContext(context.TagName));
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var summaryListContext = context.GetContextItem<SummaryListContext>();
        var rowContext = context.GetContextItem<SummaryListRowContext>();

        await output.GetChildContentAsync();

        rowContext.ThrowIfIncomplete();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        summaryListContext.AddRow(new SummaryListOptionsRow
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
