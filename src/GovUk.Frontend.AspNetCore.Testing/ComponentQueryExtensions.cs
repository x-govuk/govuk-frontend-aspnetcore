using AngleSharp.Dom;

namespace GovUk.Frontend.AspNetCore.Testing;

/// <summary>
/// Finds GOV.UK components in a document or element.
/// </summary>
/// <remarks>
/// <para>
/// Each member searches the descendants of the document or element it's called on, so calling it on an element
/// limits the search to that part of the page.
/// </para>
/// <para>
/// Members that return a single component return <see langword="null"/> when there isn't one and throw a
/// <see cref="ComponentLookupException"/> when there's more than one. <c>Get*</c> methods throw a
/// <see cref="ComponentLookupException"/> when nothing matches; the matching <c>Find*</c> methods return
/// <see langword="null"/> instead.
/// </para>
/// </remarks>
public static partial class ComponentQueryExtensions
{
    private static readonly Lookup.Subject _errorSummarySubject = new("error summary", "error summaries");
    private static readonly Lookup.Subject _notificationBannerSubject = new("notification banner", "notification banners", "titles");
    private static readonly Lookup.Subject _panelSubject = new("panel", "panels");
    private static readonly Lookup.Subject _backLinkSubject = new("back link", "back links");
    private static readonly Lookup.Subject _paginationSubject = new("pagination", "pagination components");
    private static readonly Lookup.Subject _taskListSubject = new("task list", "task lists");

    extension(IParentNode node)
    {
        /// <summary>
        /// Gets the error summary, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one error summary.</exception>
        public ErrorSummary? ErrorSummary =>
            SingleOrNone(node, ErrorSummary.Selector, e => new ErrorSummary(e), _errorSummarySubject);

        /// <summary>
        /// Gets the notification banner, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">
        /// There's more than one notification banner; use <c>NotificationBanners</c> instead.
        /// </exception>
        public NotificationBanner? NotificationBanner =>
            SingleOrNone(node, NotificationBanner.Selector, e => new NotificationBanner(e), _notificationBannerSubject);

        /// <summary>
        /// Gets every notification banner.
        /// </summary>
        public IReadOnlyList<NotificationBanner> NotificationBanners =>
            All(node, NotificationBanner.Selector, e => new NotificationBanner(e));

        /// <summary>
        /// Gets every summary list, including those in summary cards.
        /// </summary>
        public IReadOnlyList<SummaryList> SummaryLists => All(node, SummaryList.Selector, e => new SummaryList(e));

        /// <summary>
        /// Gets every summary card.
        /// </summary>
        public IReadOnlyList<SummaryCard> SummaryCards => All(node, SummaryCard.Selector, e => new SummaryCard(e));

        /// <summary>
        /// Gets every table.
        /// </summary>
        public IReadOnlyList<Table> Tables => All(node, Table.Selector, e => new Table(e));

        /// <summary>
        /// Gets the task list, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">
        /// There's more than one task list; use <c>TaskLists</c> instead.
        /// </exception>
        public TaskList? TaskList => SingleOrNone(node, TaskList.Selector, e => new TaskList(e), _taskListSubject);

        /// <summary>
        /// Gets every task list.
        /// </summary>
        public IReadOnlyList<TaskList> TaskLists => All(node, TaskList.Selector, e => new TaskList(e));

        /// <summary>
        /// Gets every tag.
        /// </summary>
        public IReadOnlyList<Tag> Tags => All(node, Tag.Selector, e => new Tag(e));

        /// <summary>
        /// Gets the panel, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one panel.</exception>
        public Panel? Panel => SingleOrNone(node, Panel.Selector, e => new Panel(e), _panelSubject);

        /// <summary>
        /// Gets the back link, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one back link.</exception>
        public BackLink? BackLink => SingleOrNone(node, BackLink.Selector, e => new BackLink(e), _backLinkSubject);

        /// <summary>
        /// Gets the pagination component, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one pagination component.</exception>
        public Pagination? Pagination =>
            SingleOrNone(node, Pagination.Selector, e => new Pagination(e), _paginationSubject);

        /// <summary>
        /// Gets every hidden input.
        /// </summary>
        public IReadOnlyList<HiddenInput> HiddenInputs => All(node, HiddenInput.Selector, e => new HiddenInput(e));

        /// <summary>
        /// Gets the summary card with the title <paramref name="title"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such summary card, or there's more than one.</exception>
        public SummaryCard GetSummaryCard(string title) => FindSummaryCard(node, title, required: true)!;

        /// <summary>
        /// Gets the summary card with the title <paramref name="title"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such summary card.</exception>
        public SummaryCard? FindSummaryCard(string title) => FindSummaryCard(node, title, required: false);

        /// <summary>
        /// Gets the row with the key <paramref name="key"/>, searching every summary list.
        /// </summary>
        /// <remarks>
        /// To find a row in a particular summary card, use <see cref="GetSummaryCard"/> and then
        /// <see cref="SummaryCard.GetRow"/>.
        /// </remarks>
        /// <exception cref="ComponentLookupException">There is no such row, or there's more than one.</exception>
        public SummaryListRow GetSummaryListRow(string key) => FindSummaryListRow(node, key, required: true)!;

        /// <summary>
        /// Gets the row with the key <paramref name="key"/>, searching every summary list, or <see langword="null"/>
        /// if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such row.</exception>
        public SummaryListRow? FindSummaryListRow(string key) => FindSummaryListRow(node, key, required: false);

        /// <summary>
        /// Gets the table with the caption <paramref name="caption"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such table, or there's more than one.</exception>
        public Table GetTable(string caption) => FindTable(node, caption, required: true)!;

        /// <summary>
        /// Gets the table with the caption <paramref name="caption"/>, or <see langword="null"/> if there isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such table.</exception>
        public Table? FindTable(string caption) => FindTable(node, caption, required: false);

        /// <summary>
        /// Gets the task named <paramref name="name"/>, searching every task list.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such task, or there's more than one.</exception>
        public TaskListItem GetTaskListItem(string name) => FindTaskListItem(node, name, required: true)!;

        /// <summary>
        /// Gets the task named <paramref name="name"/>, searching every task list, or <see langword="null"/> if there
        /// isn't one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such task.</exception>
        public TaskListItem? FindTaskListItem(string name) => FindTaskListItem(node, name, required: false);

        /// <summary>
        /// Gets the value of the hidden input named <paramref name="name"/>.
        /// </summary>
        /// <exception cref="ComponentLookupException">There is no such hidden input, or there's more than one.</exception>
        public string GetHiddenInputValue(string name) => FindHiddenInput(node, name, required: true)!.Value;

        /// <summary>
        /// Gets the value of the hidden input named <paramref name="name"/>, or <see langword="null"/> if there isn't
        /// one.
        /// </summary>
        /// <exception cref="ComponentLookupException">There's more than one such hidden input.</exception>
        public string? FindHiddenInputValue(string name) => FindHiddenInput(node, name, required: false)?.Value;
    }

    private static IReadOnlyList<T> All<T>(IParentNode node, string selector, Func<IElement, T> create)
    {
        ArgumentNullException.ThrowIfNull(node);

        return Lookup.All(node, selector, create);
    }

    private static T? SingleOrNone<T>(IParentNode node, string selector, Func<IElement, T> create, Lookup.Subject subject)
        where T : class =>
        Lookup.SingleOrNone((INode)node, All(node, selector, create), subject);

    private static SummaryCard? FindSummaryCard(IParentNode node, string title, bool required)
    {
        ArgumentNullException.ThrowIfNull(title);

        return Lookup.Single(
            (INode)node,
            node.SummaryCards,
            c => c.Title == title,
            c => c.Title,
            SummaryCard.Subject,
            $"a summary card with the title {LookupFailure.Quote(title)}",
            required);
    }

    private static SummaryListRow? FindSummaryListRow(IParentNode node, string key, bool required)
    {
        ArgumentNullException.ThrowIfNull(key);

        return Lookup.Single(
            (INode)node,
            node.SummaryLists.SelectMany(l => l.Rows).ToArray(),
            r => r.Key == key,
            r => r.Key,
            SummaryList.RowSubject,
            $"a row with the key {LookupFailure.Quote(key)}",
            required);
    }

    private static Table? FindTable(IParentNode node, string caption, bool required)
    {
        ArgumentNullException.ThrowIfNull(caption);

        return Lookup.Single(
            (INode)node,
            node.Tables,
            t => t.Caption == caption,
            t => t.Caption ?? "(no caption)",
            Table.Subject,
            $"a table with the caption {LookupFailure.Quote(caption)}",
            required);
    }

    private static TaskListItem? FindTaskListItem(IParentNode node, string name, bool required)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Lookup.Single(
            (INode)node,
            node.TaskLists.SelectMany(l => l.Items).ToArray(),
            i => i.Name == name,
            i => i.Name,
            TaskList.ItemSubject,
            $"a task named {LookupFailure.Quote(name)}",
            required);
    }

    private static HiddenInput? FindHiddenInput(IParentNode node, string name, bool required)
    {
        ArgumentNullException.ThrowIfNull(name);

        return Lookup.Single(
            (INode)node,
            node.HiddenInputs,
            i => i.Name == name,
            i => i.Name,
            HiddenInput.Subject,
            $"a hidden input named {LookupFailure.Quote(name)}",
            required);
    }
}
