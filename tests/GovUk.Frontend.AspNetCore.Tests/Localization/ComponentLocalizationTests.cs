using System.Reflection;
using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.Localization;

namespace GovUk.Frontend.AspNetCore.Tests.Localization;

public class ComponentLocalizationTests
{
    private const string Marker = "LOCALIZED";

    /// <summary>
    /// Every resource name that <see cref="DefaultComponentGenerator"/> is responsible for, mapped to
    /// options that reach it.
    /// </summary>
    private static readonly Dictionary<string, Func<DefaultComponentGenerator, ValueTask<GovUkComponent>>> Renderers = new()
    {
        [GovUkFrontendResourceNames.BackLinkText] =
            g => g.GenerateBackLinkAsync(new BackLinkOptions()),

        [GovUkFrontendResourceNames.BreadcrumbsLabelText] =
            g => g.GenerateBreadcrumbsAsync(new BreadcrumbsOptions()),

        [GovUkFrontendResourceNames.CharacterCountTextareaDescriptionTextCharacters] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountTextareaDescriptionTextWords] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CookieBannerAriaLabel] =
            g => g.GenerateCookieBannerAsync(new CookieBannerOptions()),

        [GovUkFrontendResourceNames.DateInputDayLabel] =
            g => g.GenerateDateInputAsync(new DateInputOptions { Id = "d" }),

        [GovUkFrontendResourceNames.DateInputMonthLabel] =
            g => g.GenerateDateInputAsync(new DateInputOptions { Id = "d" }),

        [GovUkFrontendResourceNames.DateInputYearLabel] =
            g => g.GenerateDateInputAsync(new DateInputOptions { Id = "d" }),

        [GovUkFrontendResourceNames.ErrorMessageVisuallyHiddenText] =
            g => g.GenerateErrorMessageAsync(new ErrorMessageOptions { Text = "Something went wrong" }),

        [GovUkFrontendResourceNames.ExitThisPageText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.ExitThisPageVisuallyHiddenText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.FooterContentLicenceHtml] =
            g => g.GenerateFooterAsync(new FooterOptions { ContentLicence = new FooterOptionsContentLicence() }),

        [GovUkFrontendResourceNames.FooterCopyrightText] =
            g => g.GenerateFooterAsync(new FooterOptions { Copyright = new FooterOptionsCopyright() }),

        [GovUkFrontendResourceNames.FooterMetaVisuallyHiddenTitle] =
            g => g.GenerateFooterAsync(new FooterOptions { Meta = new FooterOptionsMeta() }),

        [GovUkFrontendResourceNames.NotificationBannerTitleTextSuccess] =
            g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions { Type = "success" }),

        [GovUkFrontendResourceNames.NotificationBannerTitleTextImportant] =
            g => g.GenerateNotificationBannerAsync(new NotificationBannerOptions()),

        [GovUkFrontendResourceNames.PaginationLandmarkLabel] =
            g => g.GeneratePaginationAsync(new PaginationOptions()),

        [GovUkFrontendResourceNames.PaginationPreviousText] =
            g => g.GeneratePaginationAsync(new PaginationOptions { Previous = new PaginationOptionsPrevious { Href = "#" } }),

        [GovUkFrontendResourceNames.PaginationNextText] =
            g => g.GeneratePaginationAsync(new PaginationOptions { Next = new PaginationOptionsNext { Href = "#" } }),

        [GovUkFrontendResourceNames.PaginationLinkVisuallyHiddenText] =
            g => g.GeneratePaginationAsync(new PaginationOptions { Next = new PaginationOptionsNext { Href = "#" } }),

        [GovUkFrontendResourceNames.PaginationItemVisuallyHiddenText] =
            g => g.GeneratePaginationAsync(new PaginationOptions
            {
                Items = [new PaginationOptionsItem { Number = "1", Href = "#" }]
            }),

        [GovUkFrontendResourceNames.PasswordInputShowPasswordText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.PasswordInputShowPasswordAriaLabelText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.PasswordInputHidePasswordText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.PasswordInputHidePasswordAriaLabelText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.PasswordInputPasswordShownAnnouncementText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.PasswordInputPasswordHiddenAnnouncementText] =
            g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" }),

        [GovUkFrontendResourceNames.ServiceNavigationMenuButtonText] =
            g => g.GenerateServiceNavigationAsync(new ServiceNavigationOptions
            {
                ServiceName = "Service",
                Navigation = [new ServiceNavigationOptionsNavigationItem { Text = "Item", Href = "#" }]
            }),

        [GovUkFrontendResourceNames.ServiceNavigationAriaLabel] =
            g => g.GenerateServiceNavigationAsync(new ServiceNavigationOptions { ServiceName = "Service" }),

        [GovUkFrontendResourceNames.TabsTitle] =
            g => g.GenerateTabsAsync(new TabsOptions()),

        [GovUkFrontendResourceNames.WarningTextIconFallbackText] =
            g => g.GenerateWarningTextAsync(new WarningTextOptions { Text = "Careful" }),

        [GovUkFrontendResourceNames.AccordionHideAllSectionsText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.AccordionHideSectionText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.AccordionHideSectionAriaLabelText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.AccordionShowAllSectionsText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.AccordionShowSectionText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.AccordionShowSectionAriaLabelText] =
            g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOther] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextOne] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextZero] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextTwo] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextFew] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersUnderLimitTextMany] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersAtLimitText] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOther] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextOne] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextZero] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextTwo] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextFew] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountCharactersOverLimitTextMany] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOther] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextOne] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextZero] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextTwo] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextFew] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsUnderLimitTextMany] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsAtLimitText] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOther] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextOne] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextZero] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextTwo] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextFew] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.CharacterCountWordsOverLimitTextMany] =
            g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxWords = 10 }),

        [GovUkFrontendResourceNames.ExitThisPageActivatedText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.ExitThisPageTimedOutText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.ExitThisPagePressTwoMoreTimesText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.ExitThisPagePressOneMoreTimeText] =
            g => g.GenerateExitThisPageAsync(new ExitThisPageOptions()),

        [GovUkFrontendResourceNames.FileUploadChooseFilesButtonText] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadNoFileChosenText] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadDropInstructionText] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadEnteredDropZoneText] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadLeftDropZoneText] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextOther] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextOne] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextZero] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextTwo] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextFew] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true }),

        [GovUkFrontendResourceNames.FileUploadMultipleFilesChosenTextMany] =
            g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true })
    };

    /// <summary>
    /// Resource names that aren't the component generator's responsibility, so aren't covered by
    /// <see cref="Renderers"/>.
    /// </summary>
    private static readonly string[] NamesCoveredElsewhere =
    [
        // Covered by ErrorSummaryTagHelperTests and GeneratedErrorSummaryTagHelperTests
        GovUkFrontendResourceNames.ErrorSummaryTitleText,

        // Covered by TitleTagHelperTests
        GovUkFrontendResourceNames.TitleErrorPrefix,

        // Covered by DateInputModelBinderLocalizationTests
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDay,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingMonth,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingYear,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDayAndMonth,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingDayAndYear,
        GovUkFrontendResourceNames.DateInputErrorMessageMissingMonthAndYear,
        GovUkFrontendResourceNames.DateInputErrorMessageInvalidDate
    ];

    [Fact]
    public void EveryResourceName_IsCoveredByATest()
    {
        var allNames = GetAllResourceNames();
        var coveredNames = Renderers.Keys.Concat(NamesCoveredElsewhere).ToHashSet();

        var uncovered = allNames.Except(coveredNames).Order().ToArray();
        Assert.Empty(uncovered);

        // Guard against a renderer being left behind after a name is removed.
        var unknown = coveredNames.Except(allNames).Order().ToArray();
        Assert.Empty(unknown);
    }

    [Fact]
    public async Task LocalizedContent_ReachesTheRenderedOutput()
    {
        var missing = new List<string>();

        foreach (var (name, render) in Renderers)
        {
            var generator = new DefaultComponentGenerator(DelegateLocalizer.ForName(name, Marker));
            var html = (await render(generator)).GetContent().ToHtmlString();

            if (!html.Contains(Marker, StringComparison.Ordinal))
            {
                missing.Add(name);
            }
        }

        Assert.Empty(missing.Order());
    }

    [Fact]
    public async Task WithNoLocalizedContent_ClientSideAttributesAreOmitted()
    {
        // The govuk-frontend JavaScript supplies its own English defaults, so a data-i18n attribute
        // must only appear once content has actually been supplied.
        var generator = TestUtils.CreateComponentGenerator();

        var components = await Task.WhenAll(
            RenderAsync(g => g.GenerateAccordionAsync(new AccordionOptions { Id = "a" })),
            RenderAsync(g => g.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 10 })),
            RenderAsync(g => g.GenerateExitThisPageAsync(new ExitThisPageOptions())),
            RenderAsync(g => g.GenerateFileUploadAsync(new FileUploadOptions { Name = "f", JavaScript = true })),
            RenderAsync(g => g.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" })));

        foreach (var html in components)
        {
            Assert.DoesNotContain("data-i18n", html, StringComparison.Ordinal);
        }

        async Task<string> RenderAsync(Func<DefaultComponentGenerator, ValueTask<GovUkComponent>> render) =>
            (await render(generator)).GetContent().ToHtmlString();
    }

    [Fact]
    public async Task LocalizedText_IsHtmlEncoded()
    {
        const string unencoded = "Iechyd & Gofal <b>";
        const string encoded = "Iechyd &amp; Gofal &lt;b&gt;";

        var notEncoded = new List<string>();

        foreach (var (name, render) in Renderers)
        {
            // The single HTML-valued name is deliberately rendered as-is.
            if (name == GovUkFrontendResourceNames.FooterContentLicenceHtml)
            {
                continue;
            }

            // Only the name under test supplies content; supplying it for every name would let one
            // component's output mask another's.
            var generator = new DefaultComponentGenerator(DelegateLocalizer.ForName(name, unencoded));
            var html = (await render(generator)).GetContent().ToHtmlString();

            if (html.Contains(unencoded, StringComparison.Ordinal) || !html.Contains(encoded, StringComparison.Ordinal))
            {
                notEncoded.Add(name);
            }
        }

        Assert.Empty(notEncoded.Order());
    }

    [Fact]
    public async Task LocalizedHtml_IsNotEncoded()
    {
        var generator = new DefaultComponentGenerator(
            DelegateLocalizer.ForName(GovUkFrontendResourceNames.FooterContentLicenceHtml, "Trwydded <a href=\"#\">agored</a>"));

        var component = await generator.GenerateFooterAsync(new FooterOptions
        {
            ContentLicence = new FooterOptionsContentLicence()
        });

        Assert.Contains("Trwydded <a href=\"#\">agored</a>", component.GetContent().ToHtmlString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task PasswordInputShowPasswordText_SetsButtonTextAndClientSideAttributeTogether()
    {
        var generator = new DefaultComponentGenerator(
            DelegateLocalizer.ForName(GovUkFrontendResourceNames.PasswordInputShowPasswordText, "Dangos"));

        var component = await generator.GeneratePasswordInputAsync(new PasswordInputOptions { Name = "p" });
        var html = component.GetContent().ToHtmlString();

        Assert.Contains("data-i18n.show-password=\"Dangos\"", html, StringComparison.Ordinal);
        Assert.Contains(">Dangos</button>", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CharacterCountTextareaDescription_KeepsTheCountPlaceholder()
    {
        var generator = new DefaultComponentGenerator(DelegateLocalizer.ForName(
            GovUkFrontendResourceNames.CharacterCountTextareaDescriptionTextCharacters,
            "Gallwch nodi hyd at %{count} o gymeriadau"));

        var component = await generator.GenerateCharacterCountAsync(new CharacterCountOptions { Name = "c", MaxLength = 100 });

        Assert.Contains("Gallwch nodi hyd at 100 o gymeriadau", component.GetContent().ToHtmlString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task DateInputItemLabels_AreOnlyLocalizedForTheCanonicalItems()
    {
        var generator = new DefaultComponentGenerator(
            DelegateLocalizer.ForName(GovUkFrontendResourceNames.DateInputDayLabel, "Diwrnod"));

        var component = await generator.GenerateDateInputAsync(new DateInputOptions
        {
            Id = "d",
            Items = [new DateInputOptionsItem { Name = "day" }, new DateInputOptionsItem { Name = "quarter" }]
        });

        var html = component.GetContent().ToHtmlString();

        Assert.Contains("Diwrnod", html, StringComparison.Ordinal);
        Assert.Contains("Quarter", html, StringComparison.Ordinal);
    }

    private static HashSet<string> GetAllResourceNames() =>
        typeof(GovUkFrontendResourceNames)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToHashSet();
}
