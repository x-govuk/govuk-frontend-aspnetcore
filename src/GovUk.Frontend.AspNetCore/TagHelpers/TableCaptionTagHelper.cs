using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the caption in a GDS table component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = TableTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the table caption.")]
public class TableCaptionTagHelper : TagHelper
{
    internal const string TagName = "govuk-table-caption";
    internal const string ShortTagName = ShortTagNames.TableCaption;

    internal static IReadOnlyCollection<string> AllTagNames { get; } = [TagName, ShortTagName];

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var tableContext = context.GetContextItem<TableContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        tableContext.SetCaption(new TemplateString(content.Snapshot()), classes, attributes, context.TagName);

        output.SuppressOutput();
    }
}
