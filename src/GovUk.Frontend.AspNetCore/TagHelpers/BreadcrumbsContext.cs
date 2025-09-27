using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class BreadcrumbsContext
{
    private readonly List<BreadcrumbsOptionsItem> _items;

    public BreadcrumbsContext()
    {
        _items = [];
    }

    public IReadOnlyCollection<BreadcrumbsOptionsItem> Items => _items;

    public void AddItem(BreadcrumbsOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
