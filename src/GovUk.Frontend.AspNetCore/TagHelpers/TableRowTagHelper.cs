using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a row in a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TableTagHelper.TagName)]
[RestrictChildren(TableCellTagHelper.TagName, TableCellTagHelper.ShortTagName)]
public class TableRowTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-row";
    internal const string ShortTagName = ShortTagNames.TableRow;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

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

        await output.GetChildContentAsync();

        rowContext.ThrowIfIncomplete();

        tableContext.AddRow(
            new TableOptionsRow
            {
                Cells = rowContext.Cells,
                Attributes = new AttributeCollection(output.Attributes)
            },
            context.TagName);

        output.SuppressOutput();
    }
}
