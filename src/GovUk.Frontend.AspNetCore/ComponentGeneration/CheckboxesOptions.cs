namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record CheckboxesOptions
{
    public TemplateString? DescribedBy { get; set; }
    public FieldsetOptions? Fieldset { get; set; }
    public HintOptions? Hint { get; set; }
    public ErrorMessageOptions? ErrorMessage { get; set; }
    public CheckboxesOptionsFormGroup? FormGroup { get; set; }
    public TemplateString? IdPrefix { get; set; }
    public TemplateString? Name { get; set; }
    public IReadOnlyCollection<CheckboxesOptionsItem>? Items { get; set; }
    public IReadOnlyCollection<string>? Values { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record CheckboxesOptionsFormGroup : FormGroupOptions
{
    public CheckboxesOptionsBeforeInputs? BeforeInputs { get; set; }
    public CheckboxesOptionsAfterInputs? AfterInputs { get; set; }
}

public record CheckboxesOptionsBeforeInputs
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record CheckboxesOptionsAfterInputs
{
    public string? Text { get; set; }
    public string? Html { get; set; }
    [NonStandardParameter]
    public AttributeCollection? Attributes { get; set; }
}

public record CheckboxesOptionsItem
{
    public TemplateString? Text { get; set; }
    public TemplateString? Html { get; set; }
    public TemplateString? Id { get; set; }
    public TemplateString? Name { get; set; }
    public TemplateString? Value { get; set; }
    public LabelOptions? Label { get; set; }
    public HintOptions? Hint { get; set; }
    public TemplateString? Divider { get; set; }
    public bool? Checked { get; set; }
    public CheckboxesOptionsItemConditional? Conditional { get; set; }
    public TemplateString? Behaviour { get; set; }
    public bool? Disabled { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record CheckboxesOptionsItemConditional
{
    public TemplateString? Html { get; set; }
}
