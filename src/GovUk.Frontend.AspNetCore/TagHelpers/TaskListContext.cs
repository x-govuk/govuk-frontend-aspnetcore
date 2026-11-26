using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

internal class TaskListContext
{
    private readonly List<TaskListOptionsItem> _items;

    public TaskListContext()
    {
        _items = [];
    }

    public IReadOnlyCollection<TaskListOptionsItem> Items => _items;

    public void AddItem(TaskListOptionsItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }
}
