using Microsoft.Playwright;

namespace GovUk.Frontend.AspNetCore.Testing.Playwright;

public static partial class GovUkLocators
{
    extension(IPage page)
    {
        /// <summary>
        /// Fills in the date input whose legend is <paramref name="legend"/> with <paramref name="date"/>.
        /// </summary>
        public Task FillDateInputByLegendAsync(string legend, DateOnly date) => page.GetDateInputByLegend(legend).FillAsync(date);

        /// <summary>
        /// Checks the radio labelled <paramref name="label"/> in the radios whose legend is <paramref name="legend"/>.
        /// </summary>
        public Task CheckRadioByLabelAsync(string legend, string label) =>
            page.GetRadiosByLegend(legend).GetItemByLabel(label).Input.CheckAsync();

        /// <summary>
        /// Checks the checkbox labelled <paramref name="label"/> in the checkboxes whose legend is
        /// <paramref name="legend"/>.
        /// </summary>
        public Task CheckCheckboxByLabelAsync(string legend, string label) =>
            page.GetCheckboxesByLegend(legend).GetItemByLabel(label).Input.CheckAsync();

        /// <summary>
        /// Unchecks the checkbox labelled <paramref name="label"/> in the checkboxes whose legend is
        /// <paramref name="legend"/>.
        /// </summary>
        public Task UncheckCheckboxByLabelAsync(string legend, string label) =>
            page.GetCheckboxesByLegend(legend).GetItemByLabel(label).Input.UncheckAsync();

        /// <summary>
        /// Selects the option whose text is <paramref name="optionText"/> in the select labelled
        /// <paramref name="label"/>.
        /// </summary>
        /// <returns>The values of the selected options.</returns>
        public Task<IReadOnlyList<string>> SelectOptionByTextAsync(string label, string optionText) =>
            page.GetSelectByLabel(label).SelectOptionByTextAsync(optionText);

        /// <summary>
        /// Submits a form by clicking the GOV.UK button whose text is <paramref name="buttonText"/>, and waits for the
        /// response to the navigation it starts.
        /// </summary>
        /// <remarks>
        /// See <c>SubmitFormAsync(Func&lt;Task&gt;)</c> for why this waits for the response, and why assertions
        /// that follow it should be web assertions (<c>Expect(...)</c>).
        /// </remarks>
        public Task SubmitFormByButtonTextAsync(string buttonText) =>
            page.SubmitFormAsync(() => page.GetButtonByText(buttonText).ClickAsync());

        /// <summary>
        /// Runs <paramref name="submit"/>, which must submit a form, and waits for the response to the navigation it
        /// starts.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Clicking a submit button returns as soon as the click has been dispatched, so without this the assertions
        /// that follow can run against the page being navigated away from. Waiting for the navigation response
        /// anchors them to the request having been made; the assertions themselves must be web assertions
        /// (<c>Expect(...)</c>), which retry until the new document has rendered.
        /// </para>
        /// <para>
        /// This deliberately doesn't call <see cref="IPage.WaitForLoadStateAsync"/>. That requires the navigation to
        /// have been committed by the time it's called, which a click can't guarantee, and when it's called too near
        /// the commit it waits for a <c>load</c> event that has already fired and times out. Waiting for the response
        /// first narrows that window but doesn't close it. Playwright's guidance is to rely on web assertions to
        /// assess readiness rather than on load states.
        /// </para>
        /// </remarks>
        public async Task SubmitFormAsync(Func<Task> submit)
        {
            ArgumentNullException.ThrowIfNull(submit);

            await page.RunAndWaitForResponseAsync(submit, response => response.Request.IsNavigationRequest)
                .ConfigureAwait(false);
        }
    }
}
