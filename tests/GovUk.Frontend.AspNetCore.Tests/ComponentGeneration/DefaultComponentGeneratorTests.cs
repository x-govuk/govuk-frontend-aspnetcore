using AngleSharp.Diffing.Core;
using GovUk.Frontend.AspNetCore.ComponentGeneration;

namespace GovUk.Frontend.AspNetCore.Tests.ComponentGeneration;

public partial class DefaultComponentGeneratorTests
{
    private readonly DefaultComponentGenerator _componentGenerator = TestUtils.CreateComponentGenerator();

    [Theory]
    [ComponentFixtureData("accordion", typeof(AccordionOptions), exclude: ["with falsy values"])]
    public Task Accordion(ComponentTestCaseData<AccordionOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateAccordionAsync(options));

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
            (generator, options) => generator.GenerateCharacterCountAsync(options));

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
            (generator, options) => generator.GenerateCookieBannerAsync(options));

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
            (generator, options) => generator.GenerateDetailsAsync(options));

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
            (generator, options) => generator.GenerateFileUploadAsync(options));

    [Theory]
    [ComponentFixtureData(
        "footer",
        typeof(FooterOptions),
        exclude: ["with no content licence", "with only custom meta", "with only meta links"])]  // TEMP until v4 as requires breaking change
    public Task Footer(ComponentTestCaseData<FooterOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateFooterAsync(options));

    [Theory]
    [ComponentFixtureData("header", typeof(HeaderOptions))]
    public Task Header(ComponentTestCaseData<HeaderOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateHeaderAsync(options));

    [Theory]
    [ComponentFixtureData("hint", typeof(HintOptions))]
    public Task Hint(ComponentTestCaseData<HintOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateHintAsync(options));

    [Theory]
    [ComponentFixtureData("input", typeof(InputOptions))]
    public Task Input(ComponentTestCaseData<InputOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateInputAsync(options));

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
            (generator, options) => generator.GeneratePaginationAsync(options));

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
            (generator, options) => generator.GeneratePasswordInputAsync(options));

    [Theory]
    [ComponentFixtureData("phase-banner", typeof(PhaseBannerOptions))]
    public Task PhaseBanner(ComponentTestCaseData<PhaseBannerOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GeneratePhaseBannerAsync(options));

    [Theory]
    [ComponentFixtureData("select", typeof(SelectOptions), exclude: ["with falsy items", "with falsy values"])]
    public Task Select(ComponentTestCaseData<SelectOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateSelectInputAsync(options));

    [Theory]
    [ComponentFixtureData(
        "service-navigation",
        typeof(ServiceNavigationOptions),
        exclude: ["with navigation having empty values", "with navigation having only empty values"])]
    public Task ServiceNavigation(ComponentTestCaseData<ServiceNavigationOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateServiceNavigationAsync(options));

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
            (generator, options) => generator.GenerateSummaryListAsync(options));

    [Theory]
    [ComponentFixtureData("table", typeof(TableOptions), exclude: "with falsy items")]
    public Task Table(ComponentTestCaseData<TableOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTableAsync(options));

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
            (generator, options) => generator.GenerateTaskListAsync(options));

    [Theory]
    [ComponentFixtureData("textarea", typeof(TextareaOptions))]
    public Task Textarea(ComponentTestCaseData<TextareaOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateTextareaAsync(options));

    [Theory]
    [ComponentFixtureData("warning-text", typeof(WarningTextOptions))]
    public Task WarningText(ComponentTestCaseData<WarningTextOptions> data) =>
        CheckComponentHtmlMatchesExpectedHtml(
            data,
            (generator, options) => generator.GenerateWarningTextAsync(options));

    private async Task CheckComponentHtmlMatchesExpectedHtml<TOptions>(
        ComponentTestCaseData<TOptions> testCaseData,
        Func<DefaultComponentGenerator, TOptions, Task<GovUkComponent>> generateComponent,
        Predicate<IDiff>? excludeDiff = null)
    {
        var html = (await generateComponent(_componentGenerator, testCaseData.Options)).GetHtml();
        var expectedHtml = testCaseData.ExpectedHtml;
        AssertEx.HtmlEqual(expectedHtml, html, excludeDiff);
    }
}
