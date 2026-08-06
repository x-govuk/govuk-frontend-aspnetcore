using Microsoft.AspNetCore.Html;
using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record PasswordInputOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public TemplateString? Value { get; set; }
    public bool? Disabled { get; set; }
    public TemplateString? DescribedBy { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public PasswordInputOptionsFormGroup? FormGroup { get; set; }
    public TemplateString? Classes { get; set; }
    [JsonPropertyName("autocomplete")]
    public TemplateString? AutoComplete { get; set; }
    public AttributeCollection? Attributes { get; set; }
    public string? ShowPasswordText { get; set; }
    public string? HidePasswordText { get; set; }
    public string? ShowPasswordAriaLabelText { get; set; }
    public string? HidePasswordAriaLabelText { get; set; }
    public string? PasswordShownAnnouncementText { get; set; }
    public string? PasswordHiddenAnnouncementText { get; set; }
    public PasswordInputOptionsButton? Button { get; set; }
}

public record PasswordInputOptionsFormGroup : FormGroupOptions
{
    public PasswordInputOptionsBeforeInput? BeforeInput { get; set; }
    public PasswordInputOptionsAfterInput? AfterInput { get; set; }
}

public record PasswordInputOptionsBeforeInput
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
}

public record PasswordInputOptionsAfterInput
{
    public string? Text { get; set; }
    public IHtmlContent? Html { get; set; }
}

public record PasswordInputOptionsButton
{
    public TemplateString? Classes { get; set; }
}
