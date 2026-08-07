namespace GovUk.Frontend.AspNetCore.Localization;

/// <summary>
/// The resource names recognized by <see cref="IGovUkFrontendLocalizer"/>.
/// </summary>
/// <remarks>
/// <para>
/// Names follow the pattern <c>{Component}.{Parameter}[.{Variant}]</c>, where <c>{Parameter}</c> is the
/// govuk-frontend Nunjucks parameter name wherever one exists.
/// </para>
/// <para>
/// Values are HTML-encoded when they are rendered, except for names ending in <c>Html</c>, which are
/// rendered as-is. Values containing a <c>%{…}</c> placeholder must keep it verbatim; values for the
/// <c>DateInput.ErrorMessage.*</c> names must contain exactly one <c>{0}</c>, which is replaced with the
/// name of the field in error.
/// </para>
/// <para>
/// Names under &quot;client-side content&quot; have no built-in English. When no value is supplied the
/// corresponding <c>data-i18n</c> attribute is omitted and the govuk-frontend JavaScript uses its own
/// English default, so a value must be supplied for every name in a group to localize it fully.
/// </para>
/// </remarks>
public static class GovUkFrontendResourceNames
{
    // Back link

    /// <summary>The back link's text. The default is <c>Back</c>.</summary>
    public const string BackLinkText = "BackLink.Text";

    // Breadcrumbs

    /// <summary>The breadcrumbs' <c>aria-label</c>. The default is <c>Breadcrumb</c>.</summary>
    public const string BreadcrumbsLabelText = "Breadcrumbs.LabelText";

    // Character count

    /// <summary>
    /// The character count's textarea description when a maximum number of characters is set.
    /// The default is <c>You can enter up to %{count} characters</c>.
    /// </summary>
    public const string CharacterCountTextareaDescriptionTextCharacters = "CharacterCount.TextareaDescriptionText.Characters";

    /// <summary>
    /// The character count's textarea description when a maximum number of words is set.
    /// The default is <c>You can enter up to %{count} words</c>.
    /// </summary>
    public const string CharacterCountTextareaDescriptionTextWords = "CharacterCount.TextareaDescriptionText.Words";

    // Cookie banner

    /// <summary>The cookie banner's <c>aria-label</c>. The default is <c>Cookie banner</c>.</summary>
    public const string CookieBannerAriaLabel = "CookieBanner.AriaLabel";

    // Date input

    /// <summary>The date input's day field label. The default is <c>Day</c>.</summary>
    public const string DateInputDayLabel = "DateInput.DayLabel";

    /// <summary>The date input's month field label. The default is <c>Month</c>.</summary>
    public const string DateInputMonthLabel = "DateInput.MonthLabel";

    /// <summary>The date input's year field label. The default is <c>Year</c>.</summary>
    public const string DateInputYearLabel = "DateInput.YearLabel";

    // Date input model binding error messages

    /// <summary>The message used when only the day is missing. The default is <c>{0} must include a day</c>.</summary>
    public const string DateInputErrorMessageMissingDay = "DateInput.ErrorMessage.MissingDay";

    /// <summary>The message used when only the month is missing. The default is <c>{0} must include a month</c>.</summary>
    public const string DateInputErrorMessageMissingMonth = "DateInput.ErrorMessage.MissingMonth";

    /// <summary>The message used when only the year is missing. The default is <c>{0} must include a year</c>.</summary>
    public const string DateInputErrorMessageMissingYear = "DateInput.ErrorMessage.MissingYear";

    /// <summary>The message used when the day and month are missing. The default is <c>{0} must include a day and month</c>.</summary>
    public const string DateInputErrorMessageMissingDayAndMonth = "DateInput.ErrorMessage.MissingDayAndMonth";

    /// <summary>The message used when the day and year are missing. The default is <c>{0} must include a day and year</c>.</summary>
    public const string DateInputErrorMessageMissingDayAndYear = "DateInput.ErrorMessage.MissingDayAndYear";

    /// <summary>The message used when the month and year are missing. The default is <c>{0} must include a month and year</c>.</summary>
    public const string DateInputErrorMessageMissingMonthAndYear = "DateInput.ErrorMessage.MissingMonthAndYear";

    /// <summary>The message used when the date is not a real date. The default is <c>{0} must be a real date</c>.</summary>
    public const string DateInputErrorMessageInvalidDate = "DateInput.ErrorMessage.InvalidDate";

    // Error message

    /// <summary>
    /// The error message's visually hidden text, excluding the trailing colon. The default is <c>Error</c>.
    /// </summary>
    public const string ErrorMessageVisuallyHiddenText = "ErrorMessage.VisuallyHiddenText";

    // Error summary

    /// <summary>The error summary's title. The default is <c>There is a problem</c>.</summary>
    public const string ErrorSummaryTitleText = "ErrorSummary.TitleText";

    // Exit this page

    /// <summary>The exit this page button's text. The default is <c>Exit this page</c>.</summary>
    public const string ExitThisPageText = "ExitThisPage.Text";

    /// <summary>The exit this page button's visually hidden prefix. The default is <c>Emergency</c>.</summary>
    public const string ExitThisPageVisuallyHiddenText = "ExitThisPage.VisuallyHiddenText";

    // Footer

    /// <summary>
    /// The footer's content licence, as HTML. The default is the Open Government Licence v3.0 sentence
    /// including its link. This value is rendered as-is and is not HTML-encoded.
    /// </summary>
    public const string FooterContentLicenceHtml = "Footer.ContentLicence.Html";

    /// <summary>The footer's copyright text. The default is <c>© Crown copyright</c>.</summary>
    public const string FooterCopyrightText = "Footer.Copyright.Text";

    /// <summary>The footer meta section's visually hidden title. The default is <c>Support links</c>.</summary>
    public const string FooterMetaVisuallyHiddenTitle = "Footer.Meta.VisuallyHiddenTitle";

    // Notification banner

    /// <summary>The notification banner's title when its type is <c>success</c>. The default is <c>Success</c>.</summary>
    public const string NotificationBannerTitleTextSuccess = "NotificationBanner.TitleText.Success";

    /// <summary>The notification banner's title for all other types. The default is <c>Important</c>.</summary>
    public const string NotificationBannerTitleTextImportant = "NotificationBanner.TitleText.Important";

    // Pagination

    /// <summary>The pagination's <c>aria-label</c>. The default is <c>Pagination</c>.</summary>
    public const string PaginationLandmarkLabel = "Pagination.LandmarkLabel";

    /// <summary>The pagination's previous link text. The default is <c>Previous</c>.</summary>
    public const string PaginationPreviousText = "Pagination.Previous.Text";

    /// <summary>The pagination's next link text. The default is <c>Next</c>.</summary>
    public const string PaginationNextText = "Pagination.Next.Text";

    /// <summary>
    /// The visually hidden suffix on the pagination's previous and next links. The default is <c>page</c>.
    /// A space is added before it when it is rendered.
    /// </summary>
    public const string PaginationLinkVisuallyHiddenText = "Pagination.LinkVisuallyHiddenText";

    /// <summary>
    /// The <c>aria-label</c> of a pagination page link. The default is <c>Page %{number}</c>.
    /// </summary>
    public const string PaginationItemVisuallyHiddenText = "Pagination.Item.VisuallyHiddenText";

    // Password input

    /// <summary>
    /// The password input toggle button's text when the password is hidden. The default is <c>Show</c>.
    /// Supplying a value also sets the <c>data-i18n.show-password</c> attribute.
    /// </summary>
    public const string PasswordInputShowPasswordText = "PasswordInput.ShowPasswordText";

    /// <summary>
    /// The password input toggle button's <c>aria-label</c> when the password is hidden.
    /// The default is <c>Show password</c>. Supplying a value also sets the
    /// <c>data-i18n.show-password-aria-label</c> attribute.
    /// </summary>
    public const string PasswordInputShowPasswordAriaLabelText = "PasswordInput.ShowPasswordAriaLabelText";

    // Service navigation

    /// <summary>The service navigation's mobile menu button text. The default is <c>Menu</c>.</summary>
    public const string ServiceNavigationMenuButtonText = "ServiceNavigation.MenuButtonText";

    /// <summary>The service navigation's <c>aria-label</c>. The default is <c>Service information</c>.</summary>
    public const string ServiceNavigationAriaLabel = "ServiceNavigation.AriaLabel";

    // Tabs

    /// <summary>The tabs' title. The default is <c>Contents</c>.</summary>
    public const string TabsTitle = "Tabs.Title";

    // Title

    /// <summary>
    /// The prefix added to the page title when the page has errors, including the trailing colon.
    /// The default is <c>Error:</c>.
    /// </summary>
    public const string TitleErrorPrefix = "Title.ErrorPrefix";

    // Warning text

    /// <summary>The warning text's icon fallback text. The default is <c>Warning</c>.</summary>
    public const string WarningTextIconFallbackText = "WarningText.IconFallbackText";

    // Client-side content: accordion

    /// <summary>The accordion's &quot;hide all sections&quot; text. There is no built-in default.</summary>
    public const string AccordionHideAllSectionsText = "Accordion.HideAllSectionsText";

    /// <summary>The accordion's &quot;hide section&quot; text. There is no built-in default.</summary>
    public const string AccordionHideSectionText = "Accordion.HideSectionText";

    /// <summary>The accordion's &quot;hide section&quot; <c>aria-label</c>. There is no built-in default.</summary>
    public const string AccordionHideSectionAriaLabelText = "Accordion.HideSectionAriaLabelText";

    /// <summary>The accordion's &quot;show all sections&quot; text. There is no built-in default.</summary>
    public const string AccordionShowAllSectionsText = "Accordion.ShowAllSectionsText";

    /// <summary>The accordion's &quot;show section&quot; text. There is no built-in default.</summary>
    public const string AccordionShowSectionText = "Accordion.ShowSectionText";

    /// <summary>The accordion's &quot;show section&quot; <c>aria-label</c>. There is no built-in default.</summary>
    public const string AccordionShowSectionAriaLabelText = "Accordion.ShowSectionAriaLabelText";

    // Client-side content: character count

    /// <summary>
    /// The character count's description of the remaining characters when more than one remains.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextOther = "CharacterCount.CharactersUnderLimitText.Other";

    /// <summary>
    /// The character count's description of the remaining characters when one remains.
    /// There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextOne = "CharacterCount.CharactersUnderLimitText.One";

    /// <summary>
    /// The character count's description of the remaining characters when the <c>zero</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextZero = "CharacterCount.CharactersUnderLimitText.Zero";

    /// <summary>
    /// The character count's description of the remaining characters when the <c>two</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextTwo = "CharacterCount.CharactersUnderLimitText.Two";

    /// <summary>
    /// The character count's description of the remaining characters when the <c>few</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextFew = "CharacterCount.CharactersUnderLimitText.Few";

    /// <summary>
    /// The character count's description of the remaining characters when the <c>many</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersUnderLimitTextMany = "CharacterCount.CharactersUnderLimitText.Many";

    /// <summary>The character count's description when the character limit is reached. There is no built-in default.</summary>
    public const string CharacterCountCharactersAtLimitText = "CharacterCount.CharactersAtLimitText";

    /// <summary>
    /// The character count's description of the excess characters when more than one is over the limit.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextOther = "CharacterCount.CharactersOverLimitText.Other";

    /// <summary>
    /// The character count's description of the excess characters when one is over the limit.
    /// There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextOne = "CharacterCount.CharactersOverLimitText.One";

    /// <summary>
    /// The character count's description of the excess characters when the <c>zero</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextZero = "CharacterCount.CharactersOverLimitText.Zero";

    /// <summary>
    /// The character count's description of the excess characters when the <c>two</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextTwo = "CharacterCount.CharactersOverLimitText.Two";

    /// <summary>
    /// The character count's description of the excess characters when the <c>few</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextFew = "CharacterCount.CharactersOverLimitText.Few";

    /// <summary>
    /// The character count's description of the excess characters when the <c>many</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountCharactersOverLimitTextMany = "CharacterCount.CharactersOverLimitText.Many";

    /// <summary>
    /// The character count's description of the remaining words when more than one remains.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextOther = "CharacterCount.WordsUnderLimitText.Other";

    /// <summary>
    /// The character count's description of the remaining words when one remains.
    /// There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextOne = "CharacterCount.WordsUnderLimitText.One";

    /// <summary>
    /// The character count's description of the remaining words when the <c>zero</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextZero = "CharacterCount.WordsUnderLimitText.Zero";

    /// <summary>
    /// The character count's description of the remaining words when the <c>two</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextTwo = "CharacterCount.WordsUnderLimitText.Two";

    /// <summary>
    /// The character count's description of the remaining words when the <c>few</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextFew = "CharacterCount.WordsUnderLimitText.Few";

    /// <summary>
    /// The character count's description of the remaining words when the <c>many</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsUnderLimitTextMany = "CharacterCount.WordsUnderLimitText.Many";

    /// <summary>The character count's description when the word limit is reached. There is no built-in default.</summary>
    public const string CharacterCountWordsAtLimitText = "CharacterCount.WordsAtLimitText";

    /// <summary>
    /// The character count's description of the excess words when more than one is over the limit.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextOther = "CharacterCount.WordsOverLimitText.Other";

    /// <summary>
    /// The character count's description of the excess words when one is over the limit.
    /// There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextOne = "CharacterCount.WordsOverLimitText.One";

    /// <summary>
    /// The character count's description of the excess words when the <c>zero</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextZero = "CharacterCount.WordsOverLimitText.Zero";

    /// <summary>
    /// The character count's description of the excess words when the <c>two</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextTwo = "CharacterCount.WordsOverLimitText.Two";

    /// <summary>
    /// The character count's description of the excess words when the <c>few</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextFew = "CharacterCount.WordsOverLimitText.Few";

    /// <summary>
    /// The character count's description of the excess words when the <c>many</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string CharacterCountWordsOverLimitTextMany = "CharacterCount.WordsOverLimitText.Many";

    // Client-side content: exit this page

    /// <summary>The exit this page &quot;activated&quot; announcement. There is no built-in default.</summary>
    public const string ExitThisPageActivatedText = "ExitThisPage.ActivatedText";

    /// <summary>The exit this page &quot;timed out&quot; announcement. There is no built-in default.</summary>
    public const string ExitThisPageTimedOutText = "ExitThisPage.TimedOutText";

    /// <summary>The exit this page &quot;press two more times&quot; announcement. There is no built-in default.</summary>
    public const string ExitThisPagePressTwoMoreTimesText = "ExitThisPage.PressTwoMoreTimesText";

    /// <summary>The exit this page &quot;press one more time&quot; announcement. There is no built-in default.</summary>
    public const string ExitThisPagePressOneMoreTimeText = "ExitThisPage.PressOneMoreTimeText";

    // Client-side content: file upload

    /// <summary>The file upload's &quot;choose files&quot; button text. There is no built-in default.</summary>
    public const string FileUploadChooseFilesButtonText = "FileUpload.ChooseFilesButtonText";

    /// <summary>The file upload's &quot;no file chosen&quot; text. There is no built-in default.</summary>
    public const string FileUploadNoFileChosenText = "FileUpload.NoFileChosenText";

    /// <summary>The file upload's drop instruction. There is no built-in default.</summary>
    public const string FileUploadDropInstructionText = "FileUpload.DropInstructionText";

    /// <summary>The file upload's &quot;entered drop zone&quot; announcement. There is no built-in default.</summary>
    public const string FileUploadEnteredDropZoneText = "FileUpload.EnteredDropZoneText";

    /// <summary>The file upload's &quot;left drop zone&quot; announcement. There is no built-in default.</summary>
    public const string FileUploadLeftDropZoneText = "FileUpload.LeftDropZoneText";

    /// <summary>
    /// The file upload's description of the chosen files when more than one is chosen.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextOther = "FileUpload.MultipleFilesChosenText.Other";

    /// <summary>
    /// The file upload's description of the chosen files when one is chosen. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextOne = "FileUpload.MultipleFilesChosenText.One";

    /// <summary>
    /// The file upload's description of the chosen files when the <c>zero</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextZero = "FileUpload.MultipleFilesChosenText.Zero";

    /// <summary>
    /// The file upload's description of the chosen files when the <c>two</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextTwo = "FileUpload.MultipleFilesChosenText.Two";

    /// <summary>
    /// The file upload's description of the chosen files when the <c>few</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextFew = "FileUpload.MultipleFilesChosenText.Few";

    /// <summary>
    /// The file upload's description of the chosen files when the <c>many</c> plural category applies.
    /// Use the <c>%{count}</c> placeholder. There is no built-in default.
    /// </summary>
    public const string FileUploadMultipleFilesChosenTextMany = "FileUpload.MultipleFilesChosenText.Many";

    // Client-side content: password input

    /// <summary>
    /// The password input toggle button's text when the password is shown. There is no built-in default.
    /// </summary>
    public const string PasswordInputHidePasswordText = "PasswordInput.HidePasswordText";

    /// <summary>
    /// The password input toggle button's <c>aria-label</c> when the password is shown.
    /// There is no built-in default.
    /// </summary>
    public const string PasswordInputHidePasswordAriaLabelText = "PasswordInput.HidePasswordAriaLabelText";

    /// <summary>The password input's &quot;password shown&quot; announcement. There is no built-in default.</summary>
    public const string PasswordInputPasswordShownAnnouncementText = "PasswordInput.PasswordShownAnnouncementText";

    /// <summary>The password input's &quot;password hidden&quot; announcement. There is no built-in default.</summary>
    public const string PasswordInputPasswordHiddenAnnouncementText = "PasswordInput.PasswordHiddenAnnouncementText";
}
