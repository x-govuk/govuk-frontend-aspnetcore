using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.ComponentGeneration.Templates.TagHelpers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[HtmlTargetElement("*", Attributes = "_gfa-trim-content")]
public class TrimContentTagHelper : TagHelper
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = (await output.GetChildContentAsync()).GetContent();
        output.Content.SetHtmlContent(content.Trim());
        output.Attributes.RemoveAll("_gfa-trim-content");
    }
}
