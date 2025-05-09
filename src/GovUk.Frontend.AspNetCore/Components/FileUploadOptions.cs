using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.Components;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record FileUploadOptions
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
    public bool? Disabled { get; set; }
    public bool? Multiple { get; set; }
    public string? DescribedBy { get; set; }
    public LabelOptions2? Label { get; set; }
    public HintOptions2? Hint { get; set; }
    public ErrorMessageOptions2? ErrorMessage { get; set; }
    public FileUploadOptionsFormGroup? FormGroup { get; set; }
    [JsonPropertyName("javascript")]
    public bool? JavaScript { get; set; }
    public string? ChooseFilesButtonText { get; set; }
    public string? DropInstructionText { get; set; }
    public FileUploadOptionsMultipleFilesChosenText? MultipleFilesChosenText { get; set; }
    public string? NoFileChosenText { get; set; }
    public string? EnteredDropZoneText { get; set; }
    public string? LeftDropZoneText { get; set; }
    public string? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record FileUploadOptionsFormGroup : FormGroupOptions2
{
    public TextHtmlAndAttributesOptions? BeforeInput { get; set; }
    public TextHtmlAndAttributesOptions? AfterInput { get; set; }
}

public record FileUploadOptionsMultipleFilesChosenText
{
    public string? Other { get; set; }
    public string? One { get; set; }
}
