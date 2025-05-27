using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record FileUploadOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public TemplateString? Value { get; set; }
    public bool? Disabled { get; set; }
    public bool? Multiple { get; set; }
    public TemplateString? DescribedBy { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public FileUploadOptionsFormGroup? FormGroup { get; set; }
    [JsonPropertyName("javascript")]
    public bool? JavaScript { get; set; }
    public TemplateString? ChooseFilesButtonText { get; set; }
    public TemplateString? DropInstructionText { get; set; }
    public FileUploadOptionsMultipleFilesChosenText? MultipleFilesChosenText { get; set; }
    public TemplateString? NoFileChosenText { get; set; }
    public TemplateString? EnteredDropZoneText { get; set; }
    public TemplateString? LeftDropZoneText { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record FileUploadOptionsFormGroup : FormGroupOptions
{
    public FileUploadOptionsBeforeInput? BeforeInput { get; set; }
    public FileUploadOptionsAfterInput? AfterInput { get; set; }
}

public record FileUploadOptionsBeforeInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record FileUploadOptionsAfterInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record FileUploadOptionsMultipleFilesChosenText
{
    public TemplateString? Other { get; set; }
    public TemplateString? One { get; set; }
}
