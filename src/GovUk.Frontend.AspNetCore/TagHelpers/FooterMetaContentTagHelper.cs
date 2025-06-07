using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the meta section of a GDS footer component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FooterMetaTagHelper.TagName)]
public class FooterMetaContentTagHelper : TagHelper
{
    internal const string TagName = "govuk-footer-meta-content";

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var metaContext = context.GetContextItem<FooterMetaContext>();

        if (metaContext.Content is not null)
        {
            throw ExceptionHelper.OnlyOneElementIsPermittedIn(output.TagName, FooterMetaTagHelper.TagName);
        }

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);

        metaContext.Content = (content.ToTemplateString(), attributes, output.TagName);
    }
}
