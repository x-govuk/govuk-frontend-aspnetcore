using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS component's fieldset.
/// </summary>
[HtmlTargetElement(ShortTagName, ParentTag = ShortTagNames.Fieldset)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend.")]
public class LegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
}
