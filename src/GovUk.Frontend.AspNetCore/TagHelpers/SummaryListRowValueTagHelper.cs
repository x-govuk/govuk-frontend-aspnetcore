using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the value in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.ShortTagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListRowTagHelper.TagName)]
[HtmlTargetElement(ShortTagName, ParentTag = SummaryListRowTagHelper.ShortTagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the value for the row.")]
public class SummaryListRowValueTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-value";
    internal const string ShortTagName = ShortTagNames.Value;

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var summaryListRowContext = context.GetContextItem<SummaryListRowContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        summaryListRowContext.SetValue(new SummaryListOptionsRowValue()
        {
            Text = null,
            Html = content.ToTemplateString(),
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
