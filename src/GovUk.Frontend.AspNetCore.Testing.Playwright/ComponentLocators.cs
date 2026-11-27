using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

/// <summary>
/// Locates the parts of a GOV.UK error summary.
/// </summary>
/// <remarks>
/// Check the errors with <see cref="GovUkLocatorAssertions.ToHaveErrorSummaryErrorsAsync"/> on <see cref="Items"/>,
/// or with Playwright's own matchers, e.g. <c>Expect(summary.Items).ToHaveTextAsync(["…"])</c>.
/// </remarks>
public sealed class ErrorSummaryLocator
{
    internal ErrorSummaryLocator(ILocator locator)
    {
        Locator = locator;
        Title = locator.Locator(".govuk-error-summary__title");
        Description = locator.Locator(".govuk-error-summary__body > p");
        Items = locator.Locator(".govuk-error-summary__list > li");
    }

    /// <summary>
    /// Gets the error summary's root element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the error summary's title, e.g. <c>There is a problem</c>.
    /// </summary>
    public ILocator Title { get; }

    /// <summary>
    /// Gets the error summary's description, the paragraph before its list of errors.
    /// </summary>
    public ILocator Description { get; }

    /// <summary>
    /// Gets the error summary's list items, one per error, in order.
    /// </summary>
    public ILocator Items { get; }

    /// <summary>
    /// Gets the link for the error whose text is <paramref name="text"/>, which moves focus to the field with that
    /// error.
    /// </summary>
    public ILocator GetLinkByText(string text) =>
        Items.Locator("a").Filter(new() { HasTextRegex = Selectors.ExactText(text) });
}

/// <summary>
/// Locates the parts of a GOV.UK notification banner.
/// </summary>
public sealed class NotificationBannerLocator
{
    internal NotificationBannerLocator(ILocator locator)
    {
        Locator = locator;
        Title = locator.Locator(".govuk-notification-banner__title");
        Heading = locator.Locator(".govuk-notification-banner__heading");
        Content = locator.Locator(".govuk-notification-banner__content");
    }

    /// <summary>
    /// Locates the link in the banner's content whose text is <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The link's text, e.g. <c>View your application</c>.</param>
    public ILocator GetLinkByText(string text) =>
        Content.Locator("a").Filter(new() { HasTextRegex = Selectors.ExactText(text) });

    /// <summary>
    /// Gets the notification banner's root element. A success banner has the
    /// <c>govuk-notification-banner--success</c> class.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the notification banner's title, e.g. <c>Important</c> or <c>Success</c>.
    /// </summary>
    public ILocator Title { get; }

    /// <summary>
    /// Gets the notification banner's heading, if its content is a heading.
    /// </summary>
    public ILocator Heading { get; }

    /// <summary>
    /// Gets the notification banner's content.
    /// </summary>
    public ILocator Content { get; }
}

/// <summary>
/// Locates the parts of a GOV.UK summary card.
/// </summary>
public sealed class SummaryCardLocator
{
    internal SummaryCardLocator(ILocator locator)
    {
        Locator = locator;
        Title = locator.Locator(".govuk-summary-card__title");
        Actions = locator.Locator(".govuk-summary-card__actions a");
    }

    /// <summary>
    /// Gets the summary card's root element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the summary card's title.
    /// </summary>
    public ILocator Title { get; }

    /// <summary>
    /// Gets the links in the summary card's title bar.
    /// </summary>
    public ILocator Actions { get; }

    /// <summary>
    /// Gets the link in the summary card's title bar whose visible text is <paramref name="text"/>, followed by any
    /// visually hidden text, e.g. <c>Delete</c> for <c>Delete&lt;span class="govuk-visually-hidden"&gt; university
    /// of Bristol&lt;/span&gt;</c>.
    /// </summary>
    public ILocator GetActionByText(string text) =>
        Actions.Filter(new() { HasTextRegex = Selectors.StartsWithWords(text) });

    /// <summary>
    /// Gets the row in the summary card's summary list whose key is <paramref name="key"/>.
    /// </summary>
    public SummaryListRowLocator GetRowByKey(string key) => SummaryListRowLocator.Create(Locator, key);
}

/// <summary>
/// Locates the parts of a row in a GOV.UK summary list.
/// </summary>
public sealed class SummaryListRowLocator
{
    private SummaryListRowLocator(ILocator locator)
    {
        Locator = locator;
        Key = locator.Locator(".govuk-summary-list__key");
        Value = locator.Locator(".govuk-summary-list__value");
        Actions = locator.Locator(".govuk-summary-list__actions a");
    }

    /// <summary>
    /// Gets the row's element.
    /// </summary>
    public ILocator Locator { get; }

    /// <summary>
    /// Gets the row's key.
    /// </summary>
    public ILocator Key { get; }

    /// <summary>
    /// Gets the row's value.
    /// </summary>
    public ILocator Value { get; }

    /// <summary>
    /// Gets the row's action links.
    /// </summary>
    public ILocator Actions { get; }

    /// <summary>
    /// Gets the row's action link whose visible text is <c>Change</c>.
    /// </summary>
    public ILocator ChangeLink => GetActionByText("Change");

    /// <summary>
    /// Gets the row's action link whose visible text is <paramref name="text"/>, followed by any visually hidden
    /// text, e.g. <c>Change</c> for <c>Change&lt;span class="govuk-visually-hidden"&gt; name&lt;/span&gt;</c>.
    /// </summary>
    public ILocator GetActionByText(string text) =>
        Actions.Filter(new() { HasTextRegex = Selectors.StartsWithWords(text) });

    internal static SummaryListRowLocator Create(ILocator scope, string key) =>
        Create(scope.Locator(".govuk-summary-list__row"), scope.Page, key);

    internal static SummaryListRowLocator Create(ILocator rows, IPage page, string key) =>
        new(rows.Filter(new()
        {
            Has = page.Locator(".govuk-summary-list__key").Filter(new() { HasTextRegex = Selectors.ExactText(key) })
        }));
}
