using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class AccordionContext
{
    private readonly List<AccordionOptionsItem> _items;

    public AccordionContext()
    {
        _items = [];
    }

    public IReadOnlyList<AccordionOptionsItem> Items => _items;

    public void AddItem(AccordionOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
