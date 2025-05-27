using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record TextareaOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public bool? Spellcheck { get; set; }
    public int? Rows { get; set; }
    public TemplateString? Value { get; set; }
    public bool? Disabled { get; set; }
    public TemplateString? DescribedBy { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public TextareaOptionsFormGroup? FormGroup { get; set; }
    public TemplateString? Classes { get; set; }
    [JsonPropertyName("autocomplete")]
    public TemplateString? AutoComplete { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record TextareaOptionsFormGroup : FormGroupOptions
{
    public TextareaOptionsBeforeInput? BeforeInput { get; set; }
    public TextareaOptionsAfterInput? AfterInput { get; set; }
}

public record TextareaOptionsBeforeInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record TextareaOptionsAfterInput
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}
