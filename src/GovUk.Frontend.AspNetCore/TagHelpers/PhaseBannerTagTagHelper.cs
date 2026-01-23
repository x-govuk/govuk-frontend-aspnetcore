using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the tag in a GDS phase banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PhaseBannerTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use for the tag in the phase banner.")]
public class PhaseBannerTagTagHelper : TagHelper
{
    internal const string TagName = "govuk-phase-banner-tag";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var phaseBannerContext = context.GetContextItem<PhaseBannerContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        phaseBannerContext.SetTag(
            new TagOptions
            {
                Text = null,
                Html = content.ToTemplateString(),
                Classes = classes,
                Attributes = attributes
            },
            output.TagName);

        output.SuppressOutput();
    }
}
