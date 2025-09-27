using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the value in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
public class SummaryListRowValueTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-value";

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
