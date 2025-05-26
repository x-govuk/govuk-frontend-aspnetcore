using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.Components;

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
    public TemplateString? ShowPasswordText { get; set; }
    public TemplateString? HidePasswordText { get; set; }
    public TemplateString? ShowPasswordAriaLabelText { get; set; }
    public TemplateString? HidePasswordAriaLabelText { get; set; }
    public TemplateString? PasswordShownAnnouncementText { get; set; }
    public TemplateString? PasswordHiddenAnnouncementText { get; set; }
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
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record PasswordInputOptionsAfterInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record PasswordInputOptionsButton
{
    public TemplateString? Classes { get; set; }
}
