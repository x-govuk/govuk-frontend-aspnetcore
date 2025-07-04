using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS checkboxes component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
public class CheckboxesFieldsetLegendTagHelper : TagHelper
{
    internal const string TagName = "govuk-checkboxes-fieldset-legend";

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
        var fieldsetContext = context.GetContextItem<CheckboxesFieldsetContext>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            (await output.GetChildContentAsync()).Snapshot() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.SetLegend(
            IsPageHeading ?? ComponentGenerator.FieldsetLegendDefaultIsPageHeading,
            output.Attributes.ToAttributeDictionary(),
            content: content);

        output.SuppressOutput();
    }
}
