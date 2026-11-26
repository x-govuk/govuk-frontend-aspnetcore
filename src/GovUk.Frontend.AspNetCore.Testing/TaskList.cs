using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// A task list component.
/// </summary>
public sealed class TaskList
{
    internal const string Selector = "ul.govuk-task-list";

    internal static readonly Lookup.Subject ItemSubject = new("task list item", "tasks", "names");

    internal TaskList(IElement element)
    {
        Element = element;
        Items = Lookup.Children(element, "govuk-task-list__item", e => new TaskListItem(e));
    }

    /// <summary>
    /// Gets the task list's <c>&lt;ul&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the tasks, in order.
    /// </summary>
    public IReadOnlyList<TaskListItem> Items { get; }

    /// <summary>
    /// Gets the task named <paramref name="name"/>.
    /// </summary>
    /// <exception cref="ComponentLookupException">There is no such task, or there's more than one.</exception>
    public TaskListItem GetItem(string name) => FindItem(name, required: true)!;

    /// <summary>
    /// Gets the task named <paramref name="name"/>, or <see langword="null"/> if there isn't one.
    /// </summary>
    /// <exception cref="ComponentLookupException">There's more than one such task.</exception>
    public TaskListItem? FindItem(string name) => FindItem(name, required: false);

    private TaskListItem? FindItem(string name, bool required)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Lookup.Single(
            Element,
            Items,
            i => i.Name == name,
            i => i.Name,
            ItemSubject,
            $"a task named {LookupFailure.Quote(name)}",
            required);
    }

    /// <inheritdoc/>
    public override string ToString() => $"Task list: {LookupFailure.QuoteAll(Items.Select(i => i.Name))}";
}

/// <summary>
/// A task in a <see cref="TaskList"/>.
/// </summary>
public sealed class TaskListItem
{
    internal TaskListItem(IElement element)
    {
        Element = element;

        var nameAndHint = Lookup.Child(element, "govuk-task-list__name-and-hint");
        var nameElement = nameAndHint?.Children.FirstOrDefault(c => !c.ClassList.Contains("govuk-task-list__hint"));
        Name = TextExtraction.GetText(nameElement);
        Href = nameElement?.LocalName == "a" ? nameElement.GetAttribute("href") : null;
        Hint = TextExtraction.GetTextOrNull(Lookup.Child(nameAndHint, "govuk-task-list__hint"));

        var status = Lookup.Child(element, "govuk-task-list__status");
        Status = TextExtraction.GetText(status);
        var tag = status?.QuerySelector(Tag.Selector);
        StatusTag = tag is null ? null : new Tag(tag);
    }

    /// <summary>
    /// Gets the task's <c>&lt;li&gt;</c> element.
    /// </summary>
    public IElement Element { get; }

    /// <summary>
    /// Gets the task's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the hint under the task's name, or <see langword="null"/> if there isn't one.
    /// </summary>
    public string? Hint { get; }

    /// <summary>
    /// Gets the status text, e.g. <c>Completed</c>. This is the text of <see cref="StatusTag"/> if there is one.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the tag the status is shown in, or <see langword="null"/> if it's plain text.
    /// </summary>
    public Tag? StatusTag { get; }

    /// <summary>
    /// Gets the link's <c>href</c>, or <see langword="null"/> if the task isn't a link.
    /// </summary>
    public string? Href { get; }

    /// <inheritdoc/>
    public override string ToString() => $"{Name}: {Status}";
}
