using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryCardActionsContext
{
    private readonly List<SummaryListOptionsCardActionsItem> _items = [];

    public IReadOnlyCollection<SummaryListOptionsCardActionsItem> Items => _items.AsReadOnly();

    public void AddItem(SummaryListOptionsCardActionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
