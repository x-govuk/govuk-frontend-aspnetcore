using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class LanguageNavigationContext
{
    private readonly List<LanguageNavigationOptionsItem> _items = [];

    public IReadOnlyCollection<LanguageNavigationOptionsItem> Items => _items;

    public void AddItem(LanguageNavigationOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
