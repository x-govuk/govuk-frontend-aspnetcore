namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Generates GOV.UK Design System components.
/// </summary>
public interface IComponentGenerator
{
    /// <summary>
    /// Generates an accordion component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateAccordionAsync(AccordionOptions options);

    /// <summary>
    /// Generates a back link component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateBackLinkAsync(BackLinkOptions options);

    /// <summary>
    /// Generates a breadcrumbs component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateBreadcrumbsAsync(BreadcrumbsOptions options);

    /// <summary>
    /// Generates a button component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateButtonAsync(ButtonOptions options);

    /// <summary>
    /// Generates a character count component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options);

    /// <summary>
    /// Generates a checkboxes component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateCheckboxesAsync(CheckboxesOptions options);

    /// <summary>
    /// Generates a cookie banner component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateCookieBannerAsync(CookieBannerOptions options);

    /// <summary>
    /// Generates a date input component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateDateInputAsync(DateInputOptions options);

    /// <summary>
    /// Generates a details component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateDetailsAsync(DetailsOptions options);

    /// <summary>
    /// Generates an error message component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateErrorMessageAsync(ErrorMessageOptions options);

    /// <summary>
    /// Generates an error summary component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateErrorSummaryAsync(ErrorSummaryOptions options);

    /// <summary>
    /// Generates an exit this page component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options);

    /// <summary>
    /// Generates a fieldset component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateFieldsetAsync(FieldsetOptions options);

    /// <summary>
    /// Generates a file upload component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateFileUploadAsync(FileUploadOptions options);

    /// <summary>
    /// Generates a footer component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateFooterAsync(FooterOptions options);

    /// <summary>
    /// Generates a header component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateHeaderAsync(HeaderOptions options);

    /// <summary>
    /// Generates a hint component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateHintAsync(HintOptions options);

    /// <summary>
    /// Generates an input component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateInputAsync(InputOptions options);

    /// <summary>
    /// Generates an inset text component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateInsetTextAsync(InsetTextOptions options);

    /// <summary>
    /// Generates a label component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateLabelAsync(LabelOptions options);

    /// <summary>
    /// Generates a notification banner component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateNotificationBannerAsync(NotificationBannerOptions options);

    /// <summary>
    /// Generates a pagination component.
    /// </summary>
    ValueTask<GovUkComponent> GeneratePaginationAsync(PaginationOptions options);

    /// <summary>
    /// Generates a panel component.
    /// </summary>
    ValueTask<GovUkComponent> GeneratePanelAsync(PanelOptions options);

    /// <summary>
    /// Generates a password input component.
    /// </summary>
    ValueTask<GovUkComponent> GeneratePasswordInputAsync(PasswordInputOptions options);

    /// <summary>
    /// Generates a phase banner component.
    /// </summary>
    ValueTask<GovUkComponent> GeneratePhaseBannerAsync(PhaseBannerOptions options);

    /// <summary>
    /// Generates a radios component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateRadiosAsync(RadiosOptions options);

    /// <summary>
    /// Generates a select component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateSelectInputAsync(SelectOptions options);

    /// <summary>
    /// Generates a service navigation component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateServiceNavigationAsync(ServiceNavigationOptions options);

    /// <summary>
    /// Generates a skip link component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateSkipLinkAsync(SkipLinkOptions options);

    /// <summary>
    /// Generates a summary list component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateSummaryListAsync(SummaryListOptions options);

    /// <summary>
    /// Generates a table component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateTableAsync(TableOptions options);

    /// <summary>
    /// Generates a tabs component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateTabsAsync(TabsOptions options);

    /// <summary>
    /// Generates a task list component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateTaskListAsync(TaskListOptions options);

    /// <summary>
    /// Generates a tag component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateTagAsync(TagOptions options);

    /// <summary>
    /// Generates a textarea component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateTextareaAsync(TextareaOptions options);

    /// <summary>
    /// Generates a warning text component.
    /// </summary>
    ValueTask<GovUkComponent> GenerateWarningTextAsync(WarningTextOptions options);
}
