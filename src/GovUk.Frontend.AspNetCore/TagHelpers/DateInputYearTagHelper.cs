using GovUk.Frontend.AspNetCore.ComponentGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the year item in a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = DateInputFieldsetTagHelper.TagName)]
[RestrictChildren(DateInputYearLabelTagHelper.TagName)]
[OutputElementHint(DefaultComponentGenerator.ComponentElementTypes.FormGroup)]
public class DateInputYearTagHelper : DateInputItemTagHelperBase
{
    internal const string TagName = "govuk-date-input-year";

    /// <summary>
    /// Creates a <see cref="DateInputYearTagHelper"/>.
    /// </summary>
    public DateInputYearTagHelper() : base(DateInputItemTypes.Year, labelTagName: DateInputYearLabelTagHelper.TagName)
    {
    }
}
