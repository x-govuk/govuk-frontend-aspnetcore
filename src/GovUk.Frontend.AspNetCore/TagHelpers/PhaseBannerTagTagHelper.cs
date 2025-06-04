using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the tag in a GDS phase banner component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PhaseBannerTagHelper.TagName)]
public class PhaseBannerTagTagHelper : TagHelper
{
    internal const string TagName = "govuk-phase-banner-tag";

    /// <summary>
    /// Creates a <see cref="PhaseBannerTagTagHelper"/>.
    /// </summary>
    public PhaseBannerTagTagHelper()
    {
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var phaseBannerContext = (PhaseBannerContext)context.Items[typeof(PhaseBannerContext)];

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        phaseBannerContext.SetTag(output.Attributes.ToAttributeDictionary(), content.Snapshot());

        output.SuppressOutput();
    }
}
