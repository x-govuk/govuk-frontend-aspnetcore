using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TableContext
{
    private readonly List<IReadOnlyCollection<TableOptionsColumn>> _rows = [];
    private readonly List<TableOptionsHead> _headCells = [];

    public IReadOnlyCollection<IReadOnlyCollection<TableOptionsColumn>> Rows => _rows.AsReadOnly();
    
    public IReadOnlyCollection<TableOptionsHead>? Head => _headCells.Count > 0 ? _headCells.AsReadOnly() : null;

    public TemplateString? Caption { get; set; }

    public TemplateString? CaptionClasses { get; set; }

    public bool? FirstCellIsHeader { get; set; }

    public void AddRow(IReadOnlyCollection<TableOptionsColumn> row)
    {
        ArgumentNullException.ThrowIfNull(row);
        _rows.Add(row);
    }

    public void AddHeadCell(TableOptionsHead cell)
    {
        ArgumentNullException.ThrowIfNull(cell);
        _headCells.Add(cell);
    }
}
