using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents a cell in the head row of a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableHeadTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TableHeadTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the table head cell.")]
public class TableHeadCellTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-head-cell";
    internal const string ShortTagName = ShortTagNames.TableHeadCell;

    private const string FormatAttributeName = "format";
    private const string ColSpanAttributeName = "colspan";
    private const string RowSpanAttributeName = "rowspan";

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <summary>
    /// The format of the cell's content.
    /// </summary>
    /// <remarks>
    /// Specify <c>numeric</c> to right align the content.
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

        var headContext = context.GetContextItem<TableHeadContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        headContext.AddCell(new TableOptionsHead
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
