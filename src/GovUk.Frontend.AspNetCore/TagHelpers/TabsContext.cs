using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TabsContext(bool haveIdPrefix)
{
    private readonly List<TabsOptionsItem> _items = [];

    public bool HaveIdPrefix { get; } = haveIdPrefix;

    public IReadOnlyList<TabsOptionsItem> Items => _items;

    public void AddItem(TabsOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }
}
