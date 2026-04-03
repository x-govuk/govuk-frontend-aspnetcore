using System.Text.Json.Serialization;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record HeaderOptions
{
    [JsonPropertyName("homepageUrl")]
    public TemplateString? HomePageUrl { get; set; }
    public TemplateString? ProductName { get; set; }
    public TemplateString? ContainerClasses { get; set; }
    public TemplateString? Classes { get; set; }
    public AttributeCollection? Attributes { get; set; }

    [NonStandardParameter]
    public AttributeCollection? ContainerAttributes { get; set; }
}
