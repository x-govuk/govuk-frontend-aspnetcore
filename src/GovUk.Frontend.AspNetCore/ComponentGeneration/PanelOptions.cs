namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record PanelOptions
{
    public TemplateString? TitleText { get; set; }
    public TemplateString? TitleHtml { get; set; }
    public int? HeadingLevel { get; set; }
    public TemplateString? Text { get; set; }
    public TemplateString? Html { get; set; }
    public PanelActionsOptions? Actions { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? TitleAttributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? BodyAttributes { get; set; }
}

public record PanelActionsOptions
{
    public IReadOnlyCollection<PanelActionsItemOptions>? Items { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}

public record PanelActionsItemOptions
{
    public TemplateString? Text { get; set; }

    [NonStandardParameter]
    public TemplateString? Html { get; set; }

    public TemplateString? Href { get; set; }
    public TemplateString? Type { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }
}
