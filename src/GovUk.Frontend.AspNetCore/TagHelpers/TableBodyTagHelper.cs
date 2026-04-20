using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the body section of a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[RestrictChildren(TableRowTagHelper.TagName)]
public class TableBodyTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-body";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        _ = await output.GetChildContentAsync();

        output.SuppressOutput();
    }
}
