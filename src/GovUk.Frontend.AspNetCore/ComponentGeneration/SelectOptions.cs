namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record SelectOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public IReadOnlyCollection<SelectOptionsItem>? Items { get; set; }
    public TemplateString? Value { get; set; }
    public bool? Disabled { get; set; }
    public TemplateString? DescribedBy { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public SelectFormGroupOptions? FormGroup { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record SelectFormGroupOptions : FormGroupOptions
{
    public BeforeInputOptions? BeforeInput { get; set; }
    public AfterInputOptions? AfterInput { get; set; }
}

public record SelectOptionsItem
{
    public TemplateString? Value { get; set; }
    public string? Text { get; set; }
    public bool? Selected { get; set; }
    public bool? Disabled { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
