using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TableRowContext
{
    private readonly List<TableOptionsColumn> _cells = [];

    public IReadOnlyCollection<TableOptionsColumn> Cells => _cells.AsReadOnly();

    public void AddCell(TableOptionsColumn cell)
    {
        ArgumentNullException.ThrowIfNull(cell);
        _cells.Add(cell);
    }
}
