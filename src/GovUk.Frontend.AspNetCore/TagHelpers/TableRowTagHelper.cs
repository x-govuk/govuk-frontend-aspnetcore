using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a row in a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = TableBodyTagHelper.TagName)]
[RestrictChildren(TableCellTagHelper.TagName)]
public class TableRowTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-row";

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new TableRowContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var tableContext = context.GetContextItem<TableContext>();
        var rowContext = context.GetContextItem<TableRowContext>();

        _ = await output.GetChildContentAsync();

        tableContext.AddRow(rowContext.Cells);

        output.SuppressOutput();
    }
}
