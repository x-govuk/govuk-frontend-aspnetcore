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
    }

    private static ErrorSummaryLocator GetErrorSummary(LocatorScope scope) => new(scope.Locator(".govuk-error-summary"));

    private static NotificationBannerLocator GetNotificationBanner(LocatorScope scope) =>
        new(scope.Locator(".govuk-notification-banner"));

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
