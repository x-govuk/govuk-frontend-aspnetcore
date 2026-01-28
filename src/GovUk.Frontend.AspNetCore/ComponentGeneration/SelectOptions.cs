namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record SelectOptions
{
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public IReadOnlyCollection<SelectOptionsItem?>? Items { get; set; }
    public TemplateString? Value { get; set; }
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
    public SelectOptionsBeforeInput? BeforeInput { get; set; }
    public SelectOptionsAfterInput? AfterInput { get; set; }
}

public record SelectOptionsBeforeInput
{
    public TemplateString? Text { get; set; }
    public TemplateString? Html { get; set; }
}

public record SelectOptionsAfterInput
{
    public TemplateString? Text { get; set; }
    public TemplateString? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record SelectOptionsItem
{
    public TemplateString? Value { get; set; }
    public TemplateString? Text { get; set; }
    public bool? Selected { get; set; }
    public bool? Disabled { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
