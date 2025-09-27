using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class SummaryListRowActionsContext
{
    private readonly List<SummaryListOptionsRowActionsItem> _items = [];

    public IReadOnlyCollection<SummaryListOptionsRowActionsItem> Items => _items.AsReadOnly();

    public void AddItem(SummaryListOptionsRowActionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
