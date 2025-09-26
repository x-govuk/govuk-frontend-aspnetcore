namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record LogoOptions
{
    public TemplateString? Classes { get; set; }
    public bool? Rebrand { get; set; }
    public bool? UseLogotype { get; set; }
    public bool? UseTudorCrown { get; set; }
    public TemplateString? AriaLabelText { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
