using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the text in a GDS details component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DetailsTagHelper.TagName)]
public class DetailsTextTagHelper : TagHelper
{
    internal const string TagName = "govuk-details-text";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var detailsContext = context.GetContextItem<DetailsContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        detailsContext.SetText(new AttributeCollection(output.Attributes), content);

        output.SuppressOutput();
    }
}
