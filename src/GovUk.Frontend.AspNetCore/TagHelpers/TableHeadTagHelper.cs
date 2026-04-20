using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the head section of a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[RestrictChildren(TableHeadCellTagHelper.TagName)]
public class TableHeadTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-head";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        _ = await output.GetChildContentAsync();

        output.SuppressOutput();
    }
}
