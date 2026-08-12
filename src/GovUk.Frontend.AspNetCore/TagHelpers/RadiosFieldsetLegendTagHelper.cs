using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS radios component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadiosFieldsetTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = RadiosTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root radios element a fieldset is generated automatically.")]
public class RadiosFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-radios-fieldset-legend";

    /// <summary>
    /// Creates a <see cref="RadiosFieldsetLegendTagHelper"/>.
    /// </summary>
    public RadiosFieldsetLegendTagHelper()
    {
    }
}
