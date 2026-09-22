using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.IntegrationTests;

public static class PageExtensions
{
    /// <summary>
    /// Submits the form on <paramref name="page"/> by clicking the button named
    /// <paramref name="buttonName"/> and waits for the resulting navigation to have been sent.
    /// </summary>
    public static Task SubmitFormAsync(this IPage page, string buttonName) =>
        page.SubmitFormAsync(() => page.GetByRole(AriaRole.Button, new() { Name = buttonName }).ClickAsync());

    /// <summary>
    /// Runs <paramref name="submit"/>, which must submit a form, and waits for the response to the
    /// resulting navigation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Clicking a submit button returns as soon as the click has been dispatched, so without this
    /// the assertions that follow can run against the page being navigated away from. Waiting for
    /// the navigation response anchors them to the POST having happened; the assertions themselves
    /// must be web assertions (<c>Expect(...)</c>), which retry until the new document has
    /// rendered.
    /// </para>
    /// <para>
    /// Deliberately no <see cref="IPage.WaitForLoadStateAsync"/> here. It requires the navigation
    /// to have been committed by the time it's called, which a click can't guarantee, and when it's
    /// called too near the commit it waits for a <c>load</c> that has already fired and times out
    /// after 30 seconds. Waiting for the response first narrows that window but doesn't close it —
    /// it still reproduced roughly one run in ten. Playwright's guidance is to rely on web
    /// assertions to assess readiness rather than on load states.
    /// </para>
    /// </remarks>
    public static async Task SubmitFormAsync(this IPage page, Func<Task> submit) =>
        await page.RunAndWaitForResponseAsync(submit, response => response.Request.IsNavigationRequest);
}
