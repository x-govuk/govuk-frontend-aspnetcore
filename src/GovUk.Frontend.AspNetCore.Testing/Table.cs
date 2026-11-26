using System.Globalization;
using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A table component.
/// </summary>
/// <remarks>
/// Cells are matched to column headers by position, taking <c>colspan</c> into account but not <c>rowspan</c>.
/// </remarks>
public sealed class Table
{
    internal const string Selector = "table.govuk-table";

    internal static readonly Lookup.Subject Subject = new("table", "tables", "captions");

    private static readonly Lookup.Subject _rowSubject = new("table row", "rows", "first cells");

    internal Table(IElement element)
    {
        Element = element;

        Caption = TextExtraction.GetTextOrNull(element.Children.FirstOrDefault(c => c.LocalName == "caption"));

        var headerRow = element.Children.FirstOrDefault(c => c.LocalName == "thead")?.Children
            .FirstOrDefault(c => c.LocalName == "tr");
        HeaderCells = headerRow is null ? [] : CreateCells(headerRow);
        Headers = HeaderCells.Select(c => c.Text).ToArray();

        Rows = element.Children
            .Where(c => c.LocalName is "tbody" or "tfoot")
            .SelectMany(section => section.Children)
            .Concat(element.Children)
            .Where(c => c.LocalName == "tr")
            .Select(tr => new TableRow(this, tr))
            .ToArray();
    }

    /// <summary>
    /// Gets the <c>&lt;table&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the caption, or <see langword="null"/> if the table doesn't have one.
    /// </summary>
    public string? Caption { get; }

    /// <summary>
    /// Gets the column headers' text.
    /// </summary>
    public IReadOnlyList<string> Headers { get; }

    /// <summary>
    /// Gets the column header cells.
    /// </summary>
    public IReadOnlyList<TableCell> HeaderCells { get; }

    /// <summary>
    /// Gets the rows of the table body, not including the header row.
    /// </summary>
    public IReadOnlyList<TableRow> Rows { get; }

    /// <summary>
    /// Gets the row whose first cell's text is <paramref name="firstCellText"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such row, or there's more than one.</exception>
    public TableRow GetRowByFirstCellText(string firstCellText) => FindRowByFirstCellText(firstCellText, required: true)!;

    /// <summary>
    /// Gets the row whose first cell's text is <paramref name="firstCellText"/>, or <see langword="null"/> if there
    /// isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such row.</exception>
    public TableRow? FindRowByFirstCellText(string firstCellText) => FindRowByFirstCellText(firstCellText, required: false);

    private TableRow? FindRowByFirstCellText(string firstCellText, bool required)
    {
        ArgumentNullException.ThrowIfNull(firstCellText);

        return Lookup.Single(
            Element,
            Rows,
            r => r.Cells.Count > 0 && r.Cells[0].Text == firstCellText,
            r => r.Cells.Count > 0 ? r.Cells[0].Text : "",
            _rowSubject,
            $"a row whose first cell is {LookupFailure.Quote(firstCellText)}",
            required);
    }

    internal int GetColumnIndex(string header)
    {
        ArgumentNullException.ThrowIfNull(header);

        var matches = HeaderCells.Where(c => c.Text == header).ToArray();

        if (matches.Length == 1)
        {
            return matches[0].ColumnIndex;
        }

        throw new ComponentLookupException(new LookupFailure(
            matches.Length == 0 ? "Table column not found." : "More than one table column found.",
            $"a column with the header {LookupFailure.Quote(header)}",
            matches.Length == 0
                ? LookupFailure.DescribeCandidates("columns", "headers", Headers)
                : $"{matches.Length} columns",
            LookupFailure.FormatHtml(Element)));
    }

    internal static IReadOnlyList<TableCell> CreateCells(IElement row)
    {
        var cells = new List<TableCell>();
        var columnIndex = 0;

        foreach (var cell in row.Children.Where(c => c.LocalName is "th" or "td"))
        {
            var colSpan = int.TryParse(cell.GetAttribute("colspan"), NumberStyles.None, CultureInfo.InvariantCulture, out var span) && span > 0
                ? span
                : 1;

            cells.Add(new TableCell(cell, columnIndex, colSpan));
            columnIndex += colSpan;
        }

        return cells.AsReadOnly();
    }

    /// <inheritdoc/>
    public override string ToString() => Caption is null ? "Table" : $"Table: {Caption}";
}

/// <summary>
/// A row in a <see cref="Table"/>.
/// </summary>
public sealed class TableRow
{
    private readonly Table _table;

    internal TableRow(Table table, IElement element)
    {
        _table = table;
        Element = element;
        Cells = Table.CreateCells(element);
    }

    /// <summary>
    /// Gets the <c>&lt;tr&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the row's cells, including any header cells.
    /// </summary>
    public IReadOnlyList<TableCell> Cells { get; }

    /// <summary>
    /// Gets the cell in the column whose header is <paramref name="header"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">
    /// The table has no column with the header <paramref name="header"/>, more than one, or this row has no cell in
    /// that column.
    /// </exception>
    public TableCell this[string header] => GetCellByHeader(header);

    /// <summary>
    /// Gets the cell in the column whose header is <paramref name="header"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">
    /// The table has no column with the header <paramref name="header"/>, more than one, or this row has no cell in
    /// that column.
    /// </exception>
    public TableCell GetCellByHeader(string header)
    {
        var columnIndex = _table.GetColumnIndex(header);

        return Cells.FirstOrDefault(c => columnIndex >= c.ColumnIndex && columnIndex < c.ColumnIndex + c.ColumnSpan)
            ?? throw new ComponentLookupException(new LookupFailure(
                "Table cell not found.",
                $"a cell in the column {LookupFailure.Quote(header)}",
                $"a row with {Cells.Count} cells",
                LookupFailure.FormatHtml(Element)));
    }

    /// <inheritdoc/>
    public override string ToString() => string.Join(" | ", Cells.Select(c => c.Text));
}

/// <summary>
/// A cell in a <see cref="Table"/>.
/// </summary>
public sealed class TableCell
{
    internal TableCell(IElement element, int columnIndex, int columnSpan)
    {
        Element = element;
        ColumnIndex = columnIndex;
        ColumnSpan = columnSpan;
        Lines = TextExtraction.GetLines(element);
        Text = string.Join(" ", Lines);
    }

    /// <summary>
    /// Gets the <c>&lt;td&gt;</c> or <c>&lt;th&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the cell's text as a single line.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the cell's lines, as split by line breaks and block-level elements.
    /// </summary>
    public IReadOnlyList<string> Lines { get; }

    /// <summary>
    /// Gets whether the cell is a header cell (<c>&lt;th&gt;</c>).
    /// </summary>
    public bool IsHeader => Element.LocalName == "th";

    internal int ColumnIndex { get; }

    internal int ColumnSpan { get; }

    /// <inheritdoc/>
    public override string ToString() => Text;
}
