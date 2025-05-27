using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the value in a GDS summary list component row.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = SummaryListRowTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.SummaryListRowValue)]
public class SummaryListRowValueTagHelper : TagHelper
{
    internal const string TagName = "govuk-summary-list-row-value";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var summaryListRowContext = context.GetContextItem<SummaryListRowContext>();

        var childContent = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            childContent = output.Content;
        }

        var attributes = new AttributeCollection(output.Attributes);
        attributes.Remove("class", out var classes);

        summaryListRowContext.SetValue(new SummaryListOptionsRowValue()
        {
            Text = null,
            Html = childContent.ToTemplateString(),
            Classes = classes,
            Attributes = attributes
        });

        output.SuppressOutput();
    }
}
