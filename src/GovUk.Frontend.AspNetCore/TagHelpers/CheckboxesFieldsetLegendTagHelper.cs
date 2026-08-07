using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS checkboxes component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root checkboxes element a fieldset is generated automatically.")]
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
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var fieldsetContext = context.GetContextItem<FormGroupFieldsetContext2>();

        var content = output.TagMode == TagMode.StartTagAndEndTag ?
            await output.GetChildContentAsync() :
            null;

        if (output.Content.IsModified)
        {
            content = output.Content;
        }

        fieldsetContext.SetLegend(
            IsPageHeading,
            new AttributeCollection(output.Attributes),
            html: content.Snapshot(),
            TagName);

        output.SuppressOutput();
    }
}
