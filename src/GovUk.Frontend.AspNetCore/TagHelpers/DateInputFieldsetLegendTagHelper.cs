using GovUk.Frontend.AspNetCore.ComponentGeneration;
using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS date input component's fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
public class DateInputFieldsetLegendTagHelper : TagHelper
{
    internal const string TagName = "govuk-date-input-fieldset-legend";

    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Whether the legend also acts as the heading for the page.
    /// </summary>
    /// <remarks>
    /// The default is <c>false</c>.
    /// </remarks>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool? IsPageHeading { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var fieldsetContext = context.GetContextItem<DateInputFieldsetContext>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.SetLegend(
            IsPageHeading ?? ComponentGenerator.FieldsetLegendDefaultIsPageHeading,
            new AttributeCollection(output.Attributes),
            html: content?.ToTemplateString());

        output.SuppressOutput();
    }
}
