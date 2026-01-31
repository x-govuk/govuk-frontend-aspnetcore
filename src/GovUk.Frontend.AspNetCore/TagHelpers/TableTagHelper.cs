using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS table component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(TableCaptionTagHelper.TagName, TableHeadTagHelper.TagName, TableBodyTagHelper.TagName, TableRowTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.Table)]
public class TableTagHelper : TagHelper
{
    internal const string TagName = "govuk-table";
    private const string FirstCellIsHeaderAttributeName = "first-cell-is-header";

    private readonly IComponentGenerator _componentGenerator;

    /// <summary>
    /// Creates a new <see cref="TableTagHelper"/>.
    /// </summary>
    public TableTagHelper(IComponentGenerator componentGenerator)
    {
        ArgumentNullException.ThrowIfNull(componentGenerator);

        _componentGenerator = componentGenerator;
    }

    /// <summary>
    /// Whether the first cell in each row should be treated as a header.
    /// </summary>
    [HtmlAttributeName(FirstCellIsHeaderAttributeName)]
    public bool? FirstCellIsHeader { get; set; }

    /// <inheritdoc/>
    public override void Init(TagHelperContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetContextItem(new TableContext());
    }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var tableContext = context.GetContextItem<TableContext>();

        _ = await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        if (FirstCellIsHeader.HasValue)
        {
            tableContext.FirstCellIsHeader = FirstCellIsHeader;
        }

        var componentOptions = new TableOptions
        {
            Rows = tableContext.Rows.Count > 0 ? tableContext.Rows : null,
            Head = tableContext.Head,
            Caption = tableContext.Caption,
            CaptionClasses = tableContext.CaptionClasses,
            FirstCellIsHeader = tableContext.FirstCellIsHeader,
            Classes = classes,
            Attributes = attributes
        };

        var component = await _componentGenerator.GenerateTableAsync(componentOptions);

        component.ApplyToTagHelper(output);
    }
}
