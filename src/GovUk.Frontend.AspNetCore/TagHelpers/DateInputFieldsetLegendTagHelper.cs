using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the legend in a GDS date input component's fieldset.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[TagHelperDocumentation(ContentDescription = "The content is the HTML to use within the legend. When this element is specified directly inside the root date input element a fieldset is generated automatically.")]
public class DateInputFieldsetLegendTagHelper : FormGroupFieldsetLegendTagHelperBase
{
    internal const string TagName = "govuk-date-input-fieldset-legend";

    /// <summary>
    /// Creates a <see cref="DateInputFieldsetLegendTagHelper"/>.
    /// </summary>
    public DateInputFieldsetLegendTagHelper()
    {
    }
}
