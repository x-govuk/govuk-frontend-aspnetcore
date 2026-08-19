using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a cell in a row of a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableRowTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TableRowTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the table cell.")]
public class TableCellTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-cell";
    internal const string ShortTagName = ShortTagNames.TableCell;

    private const string FormatAttributeName = "format";
    private const string ColSpanAttributeName = "colspan";
    private const string RowSpanAttributeName = "rowspan";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// The format of the cell's content.
    /// </summary>
    /// <remarks>
    /// Specify <c>numeric</c> to right align the content.
    /// Ignored when the cell is the first in its row and the table has <c>first-cell-is-header</c> specified.
    /// </remarks>
    [HtmlAttributeName(FormatAttributeName)]
    public string? Format { get; set; }

    /// <summary>
    /// The number of columns the cell spans.
    /// </summary>
    [HtmlAttributeName(ColSpanAttributeName)]
    public int? ColSpan { get; set; }

    /// <summary>
    /// The number of rows the cell spans.
    /// </summary>
    [HtmlAttributeName(RowSpanAttributeName)]
    public int? RowSpan { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var rowContext = context.GetContextItem<TableRowContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        rowContext.AddCell(new TableOptionsColumn
        {
            Text = null,
            Html = content.Snapshot(),
            Format = Format is not null ? new TemplateString(Format) : null,
            Classes = classes,
            ColSpan = ColSpan,
            RowSpan = RowSpan,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
