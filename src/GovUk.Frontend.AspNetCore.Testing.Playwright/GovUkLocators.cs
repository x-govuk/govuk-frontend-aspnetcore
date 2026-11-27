using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// Locators for GOV.UK Design System components, as extension members on <see cref="IPage"/> and
/// <see cref="ILocator"/>.
/// </summary>
/// <remarks>
/// <para>
/// Like Playwright's own locators, these are lazy: nothing is looked up until a locator is used, so there's no
/// <c>Find*</c> counterpart that returns <see langword="null"/>. Check for an absent component with
/// <c>Expect(locator).ToHaveCountAsync(0)</c>.
/// </para>
/// <para>
/// Text is matched exactly, ignoring differences in whitespace.
/// </para>
/// </remarks>
public static partial class GovUkLocators
{
    extension(IPage page)
    {
        /// <summary>
        /// Gets the error summary.
        /// </summary>
        public ErrorSummaryLocator ErrorSummary => GetErrorSummary(LocatorScope.For(page));

        /// <summary>
        /// Gets the notification banner.
        /// </summary>
        public NotificationBannerLocator NotificationBanner => GetNotificationBanner(LocatorScope.For(page));

        /// <summary>
        /// Gets the back link.
        /// </summary>
        public ILocator BackLink => GetBackLink(LocatorScope.For(page));

        /// <summary>
        /// Gets the GOV.UK button whose text is <paramref name="text"/>, including a link styled as a button.
        /// </summary>
        public ILocator GetButtonByText(string text) => GetButtonByText(LocatorScope.For(page), text);

        /// <summary>
        /// Gets the summary card whose title is <paramref name="title"/>.
        /// </summary>
        public SummaryCardLocator GetSummaryCardByTitle(string title) => GetSummaryCardByTitle(LocatorScope.For(page), title);

        /// <summary>
        /// Gets the summary list row whose key is <paramref name="key"/>, in any summary list or card.
        /// </summary>
        public SummaryListRowLocator GetSummaryListRowByKey(string key) =>
            GetSummaryListRowByKey(LocatorScope.For(page), key);

        /// <summary>
        /// Locates the pagination component.
        /// </summary>
        public PaginationLocator Pagination => new(LocatorScope.For(page).Locator(".govuk-pagination"));

        /// <summary>
        /// Locates the breadcrumbs component.
        /// </summary>
        public BreadcrumbsLocator Breadcrumbs => new(LocatorScope.For(page).Locator(".govuk-breadcrumbs"));

        /// <summary>
        /// Locates the panel.
        /// </summary>
        public PanelLocator Panel => new(LocatorScope.For(page).Locator(".govuk-panel"));

        /// <summary>
        /// Locates the warning text components.
        /// </summary>
        public WarningTextLocator WarningText => new(LocatorScope.For(page).Locator(".govuk-warning-text"));

        /// <summary>
        /// Locates the inset text components.
        /// </summary>
        public ILocator InsetText => LocatorScope.For(page).Locator(".govuk-inset-text");

        /// <summary>
        /// Locates the tags.
        /// </summary>
        public ILocator Tags => LocatorScope.For(page).Locator(".govuk-tag");

        /// <summary>
        /// Locates the table, or the tables if there's more than one.
        /// </summary>
        public TableLocator Table => new(LocatorScope.For(page).Locator("table.govuk-table"));

        /// <summary>
        /// Locates the tag whose text is <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The tag's text, e.g. <c>Completed</c>.</param>
        public ILocator GetTagByText(string text) => GetTagByText(LocatorScope.For(page), text);

        /// <summary>
        /// Locates the details component whose summary is <paramref name="summary"/>.
        /// </summary>
        /// <param name="summary">The summary, e.g. <c>Help with nationality</c>.</param>
        public DetailsLocator GetDetailsBySummary(string summary) => GetDetailsBySummary(LocatorScope.For(page), summary);

        /// <summary>
        /// Locates the accordion section whose heading is <paramref name="heading"/>.
        /// </summary>
        /// <param name="heading">The section's heading, e.g. <c>Writing well for the web</c>.</param>
        public AccordionSectionLocator GetAccordionSectionByHeading(string heading) =>
            GetAccordionSectionByHeading(LocatorScope.For(page), heading);

        /// <summary>
        /// Locates the tab labelled <paramref name="label"/> and its panel.
        /// </summary>
        /// <param name="label">The tab's label, e.g. <c>Past week</c>.</param>
        public TabLocator GetTabByLabel(string label) => GetTabByLabel(LocatorScope.For(page), label);

        /// <summary>
        /// Locates the table whose caption is <paramref name="caption"/>.
        /// </summary>
        /// <param name="caption">The table's caption.</param>
        public TableLocator GetTableByCaption(string caption) => GetTableByCaption(LocatorScope.For(page), caption);

        /// <summary>
        /// Locates the task named <paramref name="name"/>, searching every task list.
        /// </summary>
        /// <param name="name">The task's name, e.g. <c>Company Directors</c>.</param>
        public TaskListItemLocator GetTaskListItemByName(string name) => GetTaskListItemByName(LocatorScope.For(page), name);
    }

    extension(ILocator locator)
    {
        /// <summary>
        /// Gets the error summary within this locator.
        /// </summary>
        public ErrorSummaryLocator ErrorSummary => GetErrorSummary(LocatorScope.For(locator));

        /// <summary>
        /// Gets the notification banner within this locator.
        /// </summary>
        public NotificationBannerLocator NotificationBanner => GetNotificationBanner(LocatorScope.For(locator));

        /// <summary>
        /// Gets the back link within this locator.
        /// </summary>
        public ILocator BackLink => GetBackLink(LocatorScope.For(locator));

        /// <summary>
        /// Gets the GOV.UK button within this locator whose text is <paramref name="text"/>, including a link styled
        /// as a button.
        /// </summary>
        public ILocator GetButtonByText(string text) => GetButtonByText(LocatorScope.For(locator), text);

        /// <summary>
        /// Gets the summary card within this locator whose title is <paramref name="title"/>.
        /// </summary>
        public SummaryCardLocator GetSummaryCardByTitle(string title) =>
            GetSummaryCardByTitle(LocatorScope.For(locator), title);

        /// <summary>
        /// Gets the summary list row within this locator whose key is <paramref name="key"/>.
        /// </summary>
        public SummaryListRowLocator GetSummaryListRowByKey(string key) =>
            GetSummaryListRowByKey(LocatorScope.For(locator), key);

        /// <summary>
        /// Locates the pagination component.
        /// </summary>
        public PaginationLocator Pagination => new(LocatorScope.For(locator).Locator(".govuk-pagination"));

        /// <summary>
        /// Locates the breadcrumbs component.
        /// </summary>
        public BreadcrumbsLocator Breadcrumbs => new(LocatorScope.For(locator).Locator(".govuk-breadcrumbs"));

        /// <summary>
        /// Locates the panel.
        /// </summary>
        public PanelLocator Panel => new(LocatorScope.For(locator).Locator(".govuk-panel"));

        /// <summary>
        /// Locates the warning text components.
        /// </summary>
        public WarningTextLocator WarningText => new(LocatorScope.For(locator).Locator(".govuk-warning-text"));

        /// <summary>
        /// Locates the inset text components.
        /// </summary>
        public ILocator InsetText => LocatorScope.For(locator).Locator(".govuk-inset-text");

        /// <summary>
        /// Locates the tags.
        /// </summary>
        public ILocator Tags => LocatorScope.For(locator).Locator(".govuk-tag");

        /// <summary>
        /// Locates the table, or the tables if there's more than one.
        /// </summary>
        public TableLocator Table => new(LocatorScope.For(locator).Locator("table.govuk-table"));

        /// <summary>
        /// Locates the tag whose text is <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The tag's text, e.g. <c>Completed</c>.</param>
        public ILocator GetTagByText(string text) => GetTagByText(LocatorScope.For(locator), text);

        /// <summary>
        /// Locates the details component whose summary is <paramref name="summary"/>.
        /// </summary>
        /// <param name="summary">The summary, e.g. <c>Help with nationality</c>.</param>
        public DetailsLocator GetDetailsBySummary(string summary) => GetDetailsBySummary(LocatorScope.For(locator), summary);

        /// <summary>
        /// Locates the accordion section whose heading is <paramref name="heading"/>.
        /// </summary>
        /// <param name="heading">The section's heading, e.g. <c>Writing well for the web</c>.</param>
        public AccordionSectionLocator GetAccordionSectionByHeading(string heading) =>
            GetAccordionSectionByHeading(LocatorScope.For(locator), heading);

        /// <summary>
        /// Locates the tab labelled <paramref name="label"/> and its panel.
        /// </summary>
        /// <param name="label">The tab's label, e.g. <c>Past week</c>.</param>
        public TabLocator GetTabByLabel(string label) => GetTabByLabel(LocatorScope.For(locator), label);

        /// <summary>
        /// Locates the table whose caption is <paramref name="caption"/>.
        /// </summary>
        /// <param name="caption">The table's caption.</param>
        public TableLocator GetTableByCaption(string caption) => GetTableByCaption(LocatorScope.For(locator), caption);

        /// <summary>
        /// Locates the task named <paramref name="name"/>, searching every task list.
        /// </summary>
        /// <param name="name">The task's name, e.g. <c>Company Directors</c>.</param>
        public TaskListItemLocator GetTaskListItemByName(string name) => GetTaskListItemByName(LocatorScope.For(locator), name);
    }

    private static ErrorSummaryLocator GetErrorSummary(LocatorScope scope) => new(scope.Locator(".govuk-error-summary"));

    private static NotificationBannerLocator GetNotificationBanner(LocatorScope scope) =>
        new(scope.Locator(".govuk-notification-banner"));

    private static ILocator GetTagByText(LocatorScope scope, string text) =>
        scope.Locator(".govuk-tag").Filter(new() { HasTextRegex = Selectors.ExactText(text) });

    private static DetailsLocator GetDetailsBySummary(LocatorScope scope, string summary) =>
        new(scope.Locator("details.govuk-details").Filter(new()
        {
            Has = scope.Page.Locator(".govuk-details__summary-text").Filter(new() { HasTextRegex = Selectors.ExactText(summary) })
        }));

    private static AccordionSectionLocator GetAccordionSectionByHeading(LocatorScope scope, string heading) =>
        new(scope.Locator(".govuk-accordion__section").Filter(new()
        {
            Has = scope.Page.Locator(AccordionSectionLocator.HeadingSelector)
                .Filter(new() { HasTextRegex = Selectors.ExactText(heading) })
        }));

    private static TabLocator GetTabByLabel(LocatorScope scope, string label) =>
        new(
            scope.Locator("a.govuk-tabs__tab").Filter(new() { HasTextRegex = Selectors.ExactText(label) }),
            scope.GetByRole(AriaRole.Tabpanel, label));

    private static TableLocator GetTableByCaption(LocatorScope scope, string caption) =>
        new(scope.Locator("table.govuk-table").Filter(new()
        {
            Has = scope.Page.Locator(".govuk-table__caption").Filter(new() { HasTextRegex = Selectors.ExactText(caption) })
        }));

    private static TaskListItemLocator GetTaskListItemByName(LocatorScope scope, string name) =>
        new(scope.Locator(".govuk-task-list__item").Filter(new()
        {
            Has = scope.Page.Locator(TaskListItemLocator.NameSelector).Filter(new() { HasTextRegex = Selectors.ExactText(name) })
        }));

    private static ILocator GetBackLink(LocatorScope scope) => scope.Locator(".govuk-back-link");

    private static ILocator GetButtonByText(LocatorScope scope, string text) =>
        scope.GetByRole(AriaRole.Button, text).And(scope.Locator(".govuk-button"));

    private static SummaryCardLocator GetSummaryCardByTitle(LocatorScope scope, string title) =>
        new(scope.Locator(".govuk-summary-card").Filter(new()
        {
            Has = scope.Page.Locator(".govuk-summary-card__title").Filter(new() { HasTextRegex = Selectors.ExactText(title) })
        }));

    private static SummaryListRowLocator GetSummaryListRowByKey(LocatorScope scope, string key) =>
        SummaryListRowLocator.Create(scope.Locator(".govuk-summary-list__row"), scope.Page, key);
}
