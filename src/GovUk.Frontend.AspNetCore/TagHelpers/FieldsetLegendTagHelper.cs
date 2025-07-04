using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS fieldset component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = FieldsetTagHelper.TagName)]
public class FieldsetLegendTagHelper : TagHelper
{
    internal const string TagName = "govuk-fieldset-legend";

    private const string IsPageHeadingAttributeName = "is-page-heading";

    /// <summary>
    /// Creates a <see cref="FieldsetLegendTagHelper"/>.
    /// </summary>
    public FieldsetLegendTagHelper()
    {
    }

    /// <summary>
    /// Whether the legend also acts as the heading for the page.
    /// </summary>
    [HtmlAttributeName(IsPageHeadingAttributeName)]
    public bool? IsPageHeading { get; set; }

    /// <inheritdoc/>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var fieldsetContext = context.GetContextItem<FieldsetContext>();

        var content = await output.GetChildContentAsync();

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.SetLegend(
            IsPageHeading ?? ComponentGenerator.FieldsetLegendDefaultIsPageHeading,
            output.Attributes.ToAttributeDictionary(),
            content.Snapshot());

        output.SuppressOutput();
    }
}
