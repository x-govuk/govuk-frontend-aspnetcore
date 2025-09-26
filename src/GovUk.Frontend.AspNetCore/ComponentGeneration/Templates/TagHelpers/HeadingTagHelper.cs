using System.ComponentModel;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[HtmlTargetElement("_gfa-heading")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class HeadingTagHelper : TagHelper
{
    [HtmlAttributeName("level")]
    public int Level { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        output.TagName = $"h{Level}";
    }
}
