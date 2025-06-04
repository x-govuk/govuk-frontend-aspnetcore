using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the description in a GDS error summary component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ErrorSummaryTagHelper.TagName)]
public class ErrorSummaryDescriptionTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-summary-description";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var errorSummaryContext = (ErrorSummaryContext)context.Items[typeof(ErrorSummaryContext)];

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        errorSummaryContext.SetDescription(
            new AttributeCollection(output.Attributes),
            content.ToTemplateString());

        output.SuppressOutput();
    }
}
