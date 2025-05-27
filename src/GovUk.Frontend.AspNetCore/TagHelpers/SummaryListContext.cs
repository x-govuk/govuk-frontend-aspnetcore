using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryListContext
{
    private readonly List<SummaryListOptionsRow> _rows = new();

    public bool HaveCard { get; set; }

    public IReadOnlyList<SummaryListOptionsRow> Rows => _rows.AsReadOnly();

    public void AddRow(SummaryListOptionsRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        _rows.Add(row);
    }
}
