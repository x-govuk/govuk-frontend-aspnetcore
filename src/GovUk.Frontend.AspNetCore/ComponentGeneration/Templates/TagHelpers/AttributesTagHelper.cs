using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[HtmlTargetElement("*", Attributes = "_gfa-attributes")]
public class AttributesTagHelper : TagHelper
{
    [HtmlAttributeName("_gfa-attributes")]
    public AttributeCollection? Attributes { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (Attributes is null)
        {
            return;
        }

        foreach (var attribute in Attributes.ToTagHelperAttributes())
        {
            output.Attributes.Add(attribute);
        }
    }
}
