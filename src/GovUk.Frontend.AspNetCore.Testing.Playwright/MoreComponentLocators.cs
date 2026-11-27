using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// Locates the parts of a pagination component.
/// </summary>
public sealed class PaginationLocator
{
    internal PaginationLocator(ILocator locator)
    {
        Locator = locator;
        Previous = locator.Locator(".govuk-pagination__prev a");
        Next = locator.Locator(".govuk-pagination__next a");
        Items = locator.Locator(".govuk-pagination__item");
        Current = locator.Locator(".govuk-pagination__item--current");
    }

    /// <summary>
    /// Gets the pagination component's <c>&lt;nav&gt;</c>.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the link to the previous page.
    /// </summary>
    public ILocator Previous { get; }

    /// <summary>
    /// Gets the link to the next page.
    /// </summary>
    public ILocator Next { get; }

    /// <summary>
    /// Gets the numbered items, including ellipses.
    /// </summary>
    public ILocator Items { get; }

    /// <summary>
    /// Gets the item for the current page.
    /// </summary>
    public ILocator Current { get; }

    /// <summary>
    /// Locates the link to page <paramref name="number"/>.
    /// </summary>
    /// <param name="number">The page number, e.g. <c>2</c>.</param>
    public ILocator GetLinkByNumber(int number) =>
        Items.Locator("a").Filter(new()
        {
            HasTextRegex = Selectors.ExactText(number.ToString(System.Globalization.CultureInfo.InvariantCulture))
        });
}

/// <summary>
/// Locates the parts of a breadcrumbs component.
/// </summary>
public sealed class BreadcrumbsLocator
{
    internal BreadcrumbsLocator(ILocator locator)
    {
        Locator = locator;
        Items = locator.Locator(".govuk-breadcrumbs__list-item");
        Current = locator.Locator(".govuk-breadcrumbs__list-item[aria-current='page']");
    }

    /// <summary>
    /// Gets the component's root element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the items, in order.
    /// </summary>
    public ILocator Items { get; }

    /// <summary>
    /// Gets the item for the current page, marked with <c>aria-current="page"</c>.
    /// </summary>
    public ILocator Current { get; }

    /// <summary>
    /// Locates the link whose text is <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The link's text, e.g. <c>Home</c>.</param>
    public ILocator GetLinkByText(string text) =>
        Locator.Locator(".govuk-breadcrumbs__link").Filter(new() { HasTextRegex = Selectors.ExactText(text) });
}

/// <summary>
/// Locates the parts of a panel component.
/// </summary>
public sealed class PanelLocator
{
    internal PanelLocator(ILocator locator)
    {
        Locator = locator;
        Title = locator.Locator(".govuk-panel__title");
        Body = locator.Locator(".govuk-panel__body");
    }

    /// <summary>
    /// Gets the panel's root element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the panel's title, e.g. <c>Application complete</c>.
    /// </summary>
    public ILocator Title { get; }

    /// <summary>
    /// Gets the panel's body.
    /// </summary>
    public ILocator Body { get; }
}

/// <summary>
/// Locates the parts of a warning text component.
/// </summary>
public sealed class WarningTextLocator
{
    internal WarningTextLocator(ILocator locator)
    {
        Locator = locator;
        Text = locator.Locator(".govuk-warning-text__text");
    }

    /// <summary>
    /// Gets the component's root element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the element with the warning.
    /// </summary>
    /// <remarks>
    /// Its text starts with visually hidden text read out in place of the icon (<c>Warning</c> by default), so check it
    /// with <c>ToContainTextAsync</c> rather than <c>ToHaveTextAsync</c>.
    /// </remarks>
    public ILocator Text { get; }
}

/// <summary>
/// Locates the parts of a details component.
/// </summary>
public sealed class DetailsLocator
{
    internal DetailsLocator(ILocator locator)
    {
        Locator = locator;
        Summary = locator.Locator(".govuk-details__summary");
        Text = locator.Locator(".govuk-details__text");
    }

    /// <summary>
    /// Gets the component's <c>&lt;details&gt;</c> element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the summary that's clicked to open the component.
    /// </summary>
    public ILocator Summary { get; }

    /// <summary>
    /// Gets the text that's revealed when the component is opened.
    /// </summary>
    public ILocator Text { get; }

    /// <summary>
    /// Opens the component by clicking its summary, unless it's already open.
    /// </summary>
    public async Task OpenAsync()
    {
        if (!await Locator.EvaluateAsync<bool>("details => details.open").ConfigureAwait(false))
        {
            await Summary.ClickAsync().ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Locates the parts of a section of an accordion.
/// </summary>
/// <remarks>
/// GOV.UK Frontend's JavaScript turns each section's heading into a button that shows and hides the section; the
/// locators work before and after it does.
/// </remarks>
public sealed class AccordionSectionLocator
{
    internal const string HeadingSelector =
        ".govuk-accordion__section-heading-text, span.govuk-accordion__section-button";

    internal AccordionSectionLocator(ILocator locator)
    {
        Locator = locator;
        Heading = locator.Locator(HeadingSelector);
        Button = locator.Locator("button.govuk-accordion__section-button");
        Summary = locator.Locator(".govuk-accordion__section-summary");
        Content = locator.Locator(".govuk-accordion__section-content");
    }

    /// <summary>
    /// Gets the section's element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the section's heading text.
    /// </summary>
    public ILocator Heading { get; }

    /// <summary>
    /// Gets the button that shows and hides the section, once GOV.UK Frontend's JavaScript has added it.
    /// </summary>
    public ILocator Button { get; }

    /// <summary>
    /// Gets the summary under the heading.
    /// </summary>
    public ILocator Summary { get; }

    /// <summary>
    /// Gets the section's content.
    /// </summary>
    public ILocator Content { get; }

    /// <summary>
    /// Shows the section by clicking its button, unless it's already shown or the accordion hasn't been enhanced by
    /// GOV.UK Frontend's JavaScript, in which case its content is always shown.
    /// </summary>
    public async Task ExpandAsync()
    {
        if (await Button.CountAsync().ConfigureAwait(false) > 0 &&
            await Button.GetAttributeAsync("aria-expanded").ConfigureAwait(false) == "false")
        {
            await Button.ClickAsync().ConfigureAwait(false);
        }
    }
}

/// <summary>
/// Locates a tab and its panel in a tabs component.
/// </summary>
/// <remarks>
/// <see cref="Panel"/> relies on GOV.UK Frontend's JavaScript, which links each panel to its tab; without it, as on
/// narrow screens, the panels are shown one after another and are found by their headings instead.
/// </remarks>
public sealed class TabLocator
{
    internal TabLocator(ILocator tab, ILocator panel)
    {
        Tab = tab;
        Panel = panel;
    }

    /// <summary>
    /// Gets the tab's link.
    /// </summary>
    public ILocator Tab { get; }

    /// <summary>
    /// Gets the tab's panel.
    /// </summary>
    public ILocator Panel { get; }

    /// <summary>
    /// Selects the tab by clicking it.
    /// </summary>
    public Task SelectAsync() => Tab.ClickAsync();
}

/// <summary>
/// Locates the parts of a task in a task list.
/// </summary>
public sealed class TaskListItemLocator
{
    internal const string NameSelector = ".govuk-task-list__name-and-hint > :first-child";

    internal TaskListItemLocator(ILocator locator)
    {
        Locator = locator;
        Name = locator.Locator(NameSelector);
        Link = locator.Locator(".govuk-task-list__link");
        Hint = locator.Locator(".govuk-task-list__hint");
        Status = locator.Locator(".govuk-task-list__status");
    }

    /// <summary>
    /// Gets the task's <c>&lt;li&gt;</c>.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the task's name, a link if the task can be started.
    /// </summary>
    public ILocator Name { get; }

    /// <summary>
    /// Gets the task's link, if it has one.
    /// </summary>
    public ILocator Link { get; }

    /// <summary>
    /// Gets the task's hint.
    /// </summary>
    public ILocator Hint { get; }

    /// <summary>
    /// Gets the task's status, e.g. <c>Completed</c>, which may be a tag.
    /// </summary>
    public ILocator Status { get; }
}

/// <summary>
/// Locates the parts of a table.
/// </summary>
public sealed class TableLocator
{
    internal TableLocator(ILocator locator)
    {
        Locator = locator;
        Caption = locator.Locator(".govuk-table__caption");
        Headers = locator.Locator("thead th");
        Rows = locator.Locator("tbody tr");
    }

    /// <summary>
    /// Gets the <c>&lt;table&gt;</c>.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the caption.
    /// </summary>
    public ILocator Caption { get; }

    /// <summary>
    /// Gets the column headers.
    /// </summary>
    public ILocator Headers { get; }

    /// <summary>
    /// Gets the rows in the table's body.
    /// </summary>
    public ILocator Rows { get; }

    /// <summary>
    /// Locates the row in the table's body whose first cell's text is <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The first cell's text.</param>
    public TableRowLocator GetRowByFirstCellText(string text) =>
        new(Rows.Filter(new()
        {
            Has = Locator.Page.Locator(":scope > :first-child").Filter(new() { HasTextRegex = Selectors.ExactText(text) })
        }));
}

/// <summary>
/// Locates the parts of a row in a table.
/// </summary>
public sealed class TableRowLocator
{
    internal TableRowLocator(ILocator locator)
    {
        Locator = locator;
        Cells = locator.Locator(":scope > th, :scope > td");
    }

    /// <summary>
    /// Gets the row's <c>&lt;tr&gt;</c>.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the row's cells.
    /// </summary>
    public ILocator Cells { get; }

    /// <summary>
    /// Locates the row's cell in the column whose header is <paramref name="header"/>.
    /// </summary>
    /// <remarks>
    /// Columns are counted by cell, so this doesn't allow for cells that span more than one column.
    /// </remarks>
    /// <param name="header">The column's header, e.g. <c>Date</c>.</param>
    public ILocator GetCellByHeader(string header)
    {
        var headerCell = $"ancestor::table[1]/thead/tr/th[normalize-space(.)={Selectors.XPathLiteral(header)}]";

        return Locator.Locator($"xpath=./*[{headerCell}][position() = count({headerCell}/preceding-sibling::*) + 1]");
    }
}
