using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a data cell in a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableRowTagHelper.TagName)]
public class TableCellTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-cell";
    private const string FormatAttributeName = "format";
    private const string ColSpanAttributeName = "colspan";
    private const string RowSpanAttributeName = "rowspan";

    /// <summary>
    /// The format of the cell.
    /// </summary>
    /// <remarks>
    /// When set to <c>"numeric"</c> the cell content will be right-aligned.
    /// </remarks>
    [HtmlAttributeName(FormatAttributeName)]
    public string? Format { get; set; }

    /// <summary>
    /// The number of columns the cell should span.
    /// </summary>
    [HtmlAttributeName(ColSpanAttributeName)]
    public int? ColSpan { get; set; }

    /// <summary>
    /// The number of rows the cell should span.
    /// </summary>
    [HtmlAttributeName(RowSpanAttributeName)]
    public int? RowSpan { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var rowContext = context.GetContextItem<TableRowContext>();

        var childContent = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        rowContext.AddCell(new TableOptionsColumn
        {
            Html = new(childContent.GetContent()),
            Format = Format is not null ? new(Format) : null,
            Classes = classes,
            ColSpan = ColSpan,
            RowSpan = RowSpan,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
