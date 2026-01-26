namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

/// <summary>
/// Generates GOV.UK Design System components.
/// </summary>
public interface IComponentGenerator
{
    /// <summary>
    /// Generates an accordion component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateAccordionAsync(AccordionOptions options);

    /// <summary>
    /// Generates a back link component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateBackLinkAsync(BackLinkOptions options);

    /// <summary>
    /// Generates a breadcrumbs component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateBreadcrumbsAsync(BreadcrumbsOptions options);

    /// <summary>
    /// Generates a button component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateButtonAsync(ButtonOptions options);

    /// <summary>
    /// Generates a character count component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateCharacterCountAsync(CharacterCountOptions options);

    /// <summary>
    /// Generates a checkboxes component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateCheckboxesAsync(CheckboxesOptions options);

    /// <summary>
    /// Generates a cookie banner component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateCookieBannerAsync(CookieBannerOptions options);

    /// <summary>
    /// Generates a date input component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateDateInputAsync(DateInputOptions options);

    /// <summary>
    /// Generates a details component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateDetailsAsync(DetailsOptions options);

    /// <summary>
    /// Generates an error message component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateErrorMessageAsync(ErrorMessageOptions options);

    /// <summary>
    /// Generates an error summary component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateErrorSummaryAsync(ErrorSummaryOptions options);

    /// <summary>
    /// Generates an exit this page component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateExitThisPageAsync(ExitThisPageOptions options);

    /// <summary>
    /// Generates a fieldset component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateFieldsetAsync(FieldsetOptions options);

    /// <summary>
    /// Generates a file upload component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateFileUploadAsync(FileUploadOptions options);

    /// <summary>
    /// Generates a footer component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateFooterAsync(FooterOptions options);

    /// <summary>
    /// Generates a header component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateHeaderAsync(HeaderOptions options);

    /// <summary>
    /// Generates a hint component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateHintAsync(HintOptions options);

    /// <summary>
    /// Generates an input component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateInputAsync(InputOptions options);

    /// <summary>
    /// Generates an inset text component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateInsetTextAsync(InsetTextOptions options);

    /// <summary>
    /// Generates a label component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateLabelAsync(LabelOptions options);

    /// <summary>
    /// Generates a notification banner component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateNotificationBannerAsync(NotificationBannerOptions options);

    /// <summary>
    /// Generates a pagination component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GeneratePaginationAsync(PaginationOptions options);

    /// <summary>
    /// Generates a panel component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GeneratePanelAsync(PanelOptions options);

    /// <summary>
    /// Generates a password input component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GeneratePasswordInputAsync(PasswordInputOptions options);

    /// <summary>
    /// Generates a phase banner component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GeneratePhaseBannerAsync(PhaseBannerOptions options);

    /// <summary>
    /// Generates a service navigation component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateServiceNavigationAsync(ServiceNavigationOptions options);

    /// <summary>
    /// Generates a skip link component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateSkipLinkAsync(SkipLinkOptions options);

    /// <summary>
    /// Generates a summary list component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateSummaryListAsync(SummaryListOptions options);

    /// <summary>
    /// Generates a table component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateTableAsync(TableOptions options);

    /// <summary>
    /// Generates a tabs component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateTabsAsync(TabsOptions options);

    /// <summary>
    /// Generates a task list component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateTaskListAsync(TaskListOptions options);

    /// <summary>
    /// Generates a tag component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateTagAsync(TagOptions options);

    /// <summary>
    /// Generates a warning text component.
    /// </summary>
    /// <returns>A <see cref="string"/> with the component's HTML.</returns>
    ValueTask<GovUkComponent> GenerateWarningTextAsync(WarningTextOptions options);
}
