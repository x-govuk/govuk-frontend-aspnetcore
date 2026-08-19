using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Generates a GDS table component.
/// </summary>
[HtmlTargetElement(TagName)]
[RestrictChildren(
    TableCaptionTagHelper.TagName,
    TableCaptionTagHelper.ShortTagName,
    TableHeadTagHelper.TagName,
    TableHeadTagHelper.ShortTagName,
    TableRowTagHelper.TagName,
    TableRowTagHelper.ShortTagName)]
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
    /// Whether the first cell in each row is a header cell.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
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

        await output.GetChildContentAsync();

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        var component = await _componentGenerator.GenerateTableAsync(new TableOptions
        {
            Rows = tableContext.Rows,
            Head = tableContext.Head?.Cells,
            HeadAttributes = tableContext.Head?.Attributes,
            Caption = tableContext.Caption?.Content,
            CaptionClasses = tableContext.Caption?.Classes,
            CaptionAttributes = tableContext.Caption?.Attributes,
            FirstCellIsHeader = FirstCellIsHeader,
            Classes = classes,
            Attributes = attributes
        });

        component.ApplyToTagHelper(output);
    }
}
