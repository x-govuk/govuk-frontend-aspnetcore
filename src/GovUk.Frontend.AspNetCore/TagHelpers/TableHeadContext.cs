using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TableHeadContext
{
    private readonly List<TableOptionsHead> _cells = [];

    public IReadOnlyCollection<TableOptionsHead> Cells => _cells.AsReadOnly();

    public void AddCell(TableOptionsHead cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        _cells.Add(cell);
    }

    public void ThrowIfIncomplete()
    {
        if (_cells.Count == 0)
        {
            throw ExceptionHelper.AChildElementMustBeProvided(TableHeadCellTagHelper.AllTagNames);
        }
    }
}
