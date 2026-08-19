using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the head row in a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TableTagHelper.TagName)]
[RestrictChildren(TableHeadCellTagHelper.TagName, TableHeadCellTagHelper.ShortTagName)]
public class TableHeadTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-head";
    internal const string ShortTagName = ShortTagNames.TableHead;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new TableHeadContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var tableContext = context.GetContextItem<TableContext>();
        var headContext = context.GetContextItem<TableHeadContext>();

        await output.GetChildContentAsync();

        headContext.ThrowIfIncomplete();

        tableContext.SetHead(headContext.Cells, new AttributeCollection(output.Attributes), context.TagName);

        output.SuppressOutput();
    }
}
