using System.ComponentModel;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[HtmlTargetElement("*", Attributes = "_gfa-trim")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class TrimContentTagHelper : TagHelper
{
    [HtmlAttributeName("_gfa-trim")]
    public bool TrimContent { get; set; }

    public override int Order => int.MaxValue;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (!TrimContent)
        {
            return;
        }

        var content = await output.GetChildContentAsync();
        var html = content.ToHtmlString(HtmlEncoder.Default);
        output.Content.SetHtmlContent(html.Trim());
    }
}
