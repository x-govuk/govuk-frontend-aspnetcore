using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS checkboxes component fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = CheckboxesFieldsetTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = CheckboxesTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root checkboxes element a fieldset is generated automatically.")]
public class CheckboxesFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-checkboxes-fieldset-legend";

    /// <summary>
    /// Creates a <see cref="CheckboxesFieldsetLegendTagHelper"/>.
    /// </summary>
    public CheckboxesFieldsetLegendTagHelper()
    {
    }
}
