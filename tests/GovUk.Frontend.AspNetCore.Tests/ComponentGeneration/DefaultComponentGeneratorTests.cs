using System.Text.RegularExpressions;
using AngleSharp.Diffing.Core;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Html;
using Match = System.Text.RegularExpressions.Match;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public class DefaultComponentGeneratorTests
{
    private static readonly Regex _decimalEncodedHtmlPattern = new("&#(\\d+);");

    private readonly DefaultComponentGenerator _componentGenerator = new();

    [Theory]
    [ComponentFixtureData("accordion", typeof(AccordionOptions), exclude: ["with falsy values"])]
    public Task Accordion(ComponentTestCaseData<AccordionOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateAccordionAsync(options),
            amendExpectedHtml: data.Name is "default" or "with translations" ? html => html.Replace("’", "&#x2019;") : null);

    [Theory]
    [ComponentFixtureData("back-link", typeof(BackLinkOptions))]
    public Task BackLink(ComponentTestCaseData<BackLinkOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateBackLinkAsync(options));

    [Theory]
    [ComponentFixtureData("breadcrumbs", typeof(BreadcrumbsOptions))]
    public Task Breadcrumbs(ComponentTestCaseData<BreadcrumbsOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateBreadcrumbsAsync(options));

    [Theory]
    [ComponentFixtureData("button", typeof(ButtonOptions))]
    public Task Button(ComponentTestCaseData<ButtonOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateButtonAsync(options));

    [Theory]
    [ComponentFixtureData("character-count", typeof(CharacterCountOptions))]
    public Task CharacterCount(ComponentTestCaseData<CharacterCountOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateCharacterCountAsync(options),
            amendExpectedHtml: html => html.Replace("Street\nLondon\nNW1 6XE\n", "Street&#xA;London&#xA;NW1 6XE&#xA;"));

    [Theory]
    [ComponentFixtureData("checkboxes", typeof(CheckboxesOptions), exclude: ["with falsy values"])]
    public Task Checkboxes(ComponentTestCaseData<CheckboxesOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateCheckboxesAsync(options));

    [Theory]
    [ComponentFixtureData("cookie-banner", typeof(CookieBannerOptions))]
    public Task CookieBanner(ComponentTestCaseData<CookieBannerOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateCookieBannerAsync(options),
            amendExpectedHtml: html => Regex.Replace(html, "</div>\n\n\n  </div>\n</div>$", "</div>\n\n  </div>\n</div>"));

    [Theory]
    [ComponentFixtureData("date-input", typeof(DateInputOptions))]
    public Task DateInput(ComponentTestCaseData<DateInputOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateDateInputAsync(options));

    [Theory]
    [ComponentFixtureData("details", typeof(DetailsOptions))]
    public Task Details(ComponentTestCaseData<DetailsOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateDetailsAsync(options),
            amendExpectedHtml: html => html.Replace("’", "&#x2019;"));

    [Theory]
    [ComponentFixtureData("error-message", typeof(ErrorMessageOptions))]
    public Task ErrorMessage(ComponentTestCaseData<ErrorMessageOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateErrorMessageAsync(options));

    [Theory]
    [ComponentFixtureData("error-summary", typeof(ErrorSummaryOptions))]
    public Task ErrorSummary(ComponentTestCaseData<ErrorSummaryOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateErrorSummaryAsync(options));

    [Theory]
    [ComponentFixtureData("exit-this-page", typeof(ExitThisPageOptions))]
    public Task ExitThisPage(ComponentTestCaseData<ExitThisPageOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateExitThisPageAsync(options));

    [Theory]
    [ComponentFixtureData("fieldset", typeof(FieldsetOptions))]
    public Task Fieldset(ComponentTestCaseData<FieldsetOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateFieldsetAsync(options));

    [Theory]
    [ComponentFixtureData("file-upload", typeof(FileUploadOptions), only: "translated")]
    public Task FileUpload(ComponentTestCaseData<FileUploadOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateFileUploadAsync(options),
            amendExpectedHtml: html => html.Replace("C:&#x5C;fakepath&#x5C;myphoto.jpg", "C:\\fakepath\\myphoto.jpg"));

    [Theory]
    [ComponentFixtureData("footer", typeof(FooterOptions))]
    public Task Footer(ComponentTestCaseData<FooterOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateFooterAsync(options),
            amendExpectedHtml: html =>
            {
                html = html.Replace("©", "&#xA9;");

                if (data.Name is "with custom HTML content licence and copyright notice")
                {
                    html = html.Replace("Mae&#x2019;r", "Mae’r");
                }
                else
                {
                    html = html.Replace("Mae’r", "Mae&#x2019;r");
                }

                return html;
            },
            compareWhitespace: false);

    [Theory]
    [ComponentFixtureData("header", typeof(HeaderOptions))]
    public Task Header(ComponentTestCaseData<HeaderOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateHeaderAsync(options),
            compareWhitespace: false);

    [Theory]
    [ComponentFixtureData("hint", typeof(HintOptions))]
    public Task Hint(ComponentTestCaseData<HintOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateHintAsync(options),
            amendExpectedHtml:
                // Account for weird encoding differences
                data.Name == "default" ?
                    html => html.Replace(
                    "\nFor example, &#x27;QQ 12 34 56 C&#x27;.\n",
                    "&#xA;For example, &#x27;QQ 12 34 56 C&#x27;.&#xA;",
                    StringComparison.OrdinalIgnoreCase) :
                null);

    [Theory]
    [ComponentFixtureData("input", typeof(InputOptions))]
    public Task Input(ComponentTestCaseData<InputOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateInputAsync(options),
            amendExpectedHtml:
                data.Name is not "with prefix with html" ?
                    html => html
                        .Replace("’", "&#x2019;")
                        .Replace("‘", "&#x2018;")
                        .Replace("£", "&#xA3;") :
                    null);

    [Theory]
    [ComponentFixtureData("inset-text", typeof(InsetTextOptions))]
    public Task InsetText(ComponentTestCaseData<InsetTextOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateInsetTextAsync(options));

    [Theory]
    [ComponentFixtureData("label", typeof(LabelOptions))]
    public Task Label(ComponentTestCaseData<LabelOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateLabelAsync(options));

    [Theory]
    [ComponentFixtureData("notification-banner", typeof(NotificationBannerOptions))]
    public Task NotificationBanner(ComponentTestCaseData<NotificationBannerOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateNotificationBannerAsync(options));

    [Theory]
    [ComponentFixtureData("pagination", typeof(PaginationOptions))]
    public Task Pagination(ComponentTestCaseData<PaginationOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GeneratePaginationAsync(options),
            amendExpectedHtml: html => html.Replace("précédente", "pr&#xE9;c&#xE9;dente"));

    [Theory]
    [ComponentFixtureData("panel", typeof(PanelOptions))]
    public Task Panel(ComponentTestCaseData<PanelOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GeneratePanelAsync(options));

    [Theory]
    [ComponentFixtureData("password-input", typeof(PasswordInputOptions))]
    public Task PasswordInput(ComponentTestCaseData<PasswordInputOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GeneratePasswordInputAsync(options),
            compareWhitespace: false);

    [Theory]
    [ComponentFixtureData("phase-banner", typeof(PhaseBannerOptions))]
    public Task PhaseBanner(ComponentTestCaseData<PhaseBannerOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GeneratePhaseBannerAsync(options),
            amendExpectedHtml: html => html.Replace("–", "&#x2013;"));

    [Theory]
    [ComponentFixtureData(
        "service-navigation",
        typeof(ServiceNavigationOptions),
        exclude: ["with navigation having empty values", "with navigation having only empty values"])]
    public Task ServiceNavigation(ComponentTestCaseData<ServiceNavigationOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateServiceNavigationAsync(options),
            compareWhitespace: false);

    [Theory]
    [ComponentFixtureData("skip-link", typeof(SkipLinkOptions))]
    public Task SkipLink(ComponentTestCaseData<SkipLinkOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateSkipLinkAsync(options));

    [Theory]
    [ComponentFixtureData("summary-list", typeof(SummaryListOptions), exclude: "with falsy values")]
    public Task SummaryList(ComponentTestCaseData<SummaryListOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateSummaryListAsync(options),
            amendExpectedHtml: html => html.Replace("Gatsby’s", "Gatsby&#x2019;s"));

    [Theory]
    [ComponentFixtureData("table", typeof(TableOptions), exclude: "with falsy items")]
    public Task Table(ComponentTestCaseData<TableOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTableAsync(options),
            amendExpectedHtml: html => html.Replace("£", "&#xA3;"));

    [Theory]
    [ComponentFixtureData("tabs", typeof(TabsOptions), exclude: "with falsy values")]
    public Task Tabs(ComponentTestCaseData<TabsOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTabsAsync(options));

    [Theory]
    [ComponentFixtureData("tag", typeof(TagOptions))]
    public Task Tag(ComponentTestCaseData<TagOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTagAsync(options));

    [Theory]
    [ComponentFixtureData("task-list", typeof(TaskListOptions), exclude: "with empty values")]
    public Task TaskList(ComponentTestCaseData<TaskListOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTaskListAsync(options),
            amendExpectedHtml: html => html.Replace("£", "&#xA3;"));

    [Theory]
    [ComponentFixtureData("textarea", typeof(TextareaOptions))]
    public Task Textarea(ComponentTestCaseData<TextareaOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTextareaAsync(options),
            amendExpectedHtml: html => html
                .Replace("’", "&#x2019;")
                .Replace("‘", "&#x2018;")
                .Replace("Street\nLondon\nNW1 6XE\n", "Street&#xA;London&#xA;NW1 6XE&#xA;"));

    [Theory]
    [ComponentFixtureData("warning-text", typeof(WarningTextOptions))]
    public Task WarningText(ComponentTestCaseData<WarningTextOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateWarningTextAsync(options),
            amendExpectedHtml: html => html.Replace("£", "&#xA3;").Replace("’", "&#x2019;"));

    private async Task CheckComponentHtmlMatchesExpectedHtml<TOptions>(
        ComponentTestCaseData<TOptions> testCaseData,
        Func<DefaultComponentGenerator, TOptions, ValueTask<IHtmlContent>> generateComponent,
        bool compareWhitespace = true,
        Predicate<IDiff>? excludeDiff = null,
        Func<string, string>? amendExpectedHtml = null)
    {
        var html = (await generateComponent(_componentGenerator, testCaseData.Options)).ToHtmlString();

        // Some of the fixtures have characters with different encodings to what we produce;
        // normalize those before comparing:
        var expectedHtml = _decimalEncodedHtmlPattern
            .Replace(
                testCaseData.ExpectedHtml,
                (Match match) =>
                {
                    var encodedDecimal = int.Parse(match.Groups[1].Value);
                    return $"&#x{encodedDecimal:X};";
                });

        if (amendExpectedHtml is not null)
        {
            expectedHtml = amendExpectedHtml(expectedHtml);
        }

        // Semantic comparison
        AssertEx.HtmlEqual(expectedHtml, html, excludeDiff);

        // For exact character-by-character equality
        if (compareWhitespace)
        {
            Assert.Equal(expectedHtml.ReplaceLineEndings(), html.ReplaceLineEndings());
        }
    }
}
