using System.ComponentModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[HtmlTargetElement("_gfa-capture")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class CaptureTagHelper : TagHelper
{
    public IHtmlContentBuilder? To { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (To is null)
        {
            throw new InvalidOperationException("The 'to' attribute must be set.");
        }

        var content = (await output.GetChildContentAsync()).Snapshot();
        To.AppendHtml(content);

        output.SuppressOutput();
    }
}
