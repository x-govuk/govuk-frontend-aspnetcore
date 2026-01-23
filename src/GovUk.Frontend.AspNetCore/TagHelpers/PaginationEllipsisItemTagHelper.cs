using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents an ellipsis item in a GDS pagination component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = PaginationTagHelper.TagName, TagStructure = TagStructure.WithoutEndTag)]
public class PaginationEllipsisItemTagHelper : TagHelper
{
    internal const string TagName = "govuk-pagination-ellipsis";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var paginationContext = context.GetContextItem<PaginationContext>();

        var attributes = new AttributeCollection(output.Attributes);

        paginationContext.AddItem(new PaginationOptionsItem
        {
            Ellipsis = true,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
