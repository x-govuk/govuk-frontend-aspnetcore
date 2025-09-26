using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[EditorBrowsable(EditorBrowsableState.Never)]
[HtmlTargetElement("*", Attributes = "_gfa-prune-empty-attributes")]
public class PruneEmptyAttributesTagHelper : TagHelper
{
    public override int Order => int.MaxValue;

    [HtmlAttributeName("_gfa-prune-empty-attributes")]
    public string? PruneEmptyAttributes { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        var attributesToPrune = (PruneEmptyAttributes ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var attr in output.Attributes.ToArray())
        {
            if (attributesToPrune.Contains(attr.Name, StringComparer.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(attr.Value.ToString()))
            {
                output.Attributes.Remove(attr);
            }
        }
    }
}
