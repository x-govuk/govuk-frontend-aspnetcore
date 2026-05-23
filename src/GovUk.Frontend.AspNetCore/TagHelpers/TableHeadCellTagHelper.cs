using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a header cell in the head section of a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableHeadTagHelper.TagName)]
public class TableHeadCellTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-head-cell";
    private const string FormatAttributeName = "format";
    private const string ColSpanAttributeName = "colspan";
    private const string RowSpanAttributeName = "rowspan";

    /// <summary>
    /// The format of the header cell.
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

        var tableContext = context.GetContextItem<TableContext>();

        var childContent = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        tableContext.AddHeadCell(new TableOptionsHead
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
