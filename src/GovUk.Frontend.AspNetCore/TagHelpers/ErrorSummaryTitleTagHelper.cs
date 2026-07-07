using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the title in the GDS error summary component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = ErrorSummaryTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the title for the error summary. If this element is not specified then the content is 'There is a problem'.")]
public class ErrorSummaryTitleTagHelper : TagHelper
{
    internal const string TagName = "govuk-error-summary-title";

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var errorSummaryContext = context.GetContextItem<ErrorSummaryContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        errorSummaryContext.SetTitle(
            new AttributeCollection(output.Attributes),
            content.ToTemplateString());

        output.SuppressOutput();
    }
}
