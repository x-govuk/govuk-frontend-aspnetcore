using GovUk.Frontend.AspNetCore.HtmlGeneration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in the year item of a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputYearTagHelper.TagName)]
[OutputElementHint(ComponentGenerator.LabelElement)]
public class DateInputYearLabelTagHelper : DateInputItemLabelTagHelperBase
{
    internal const string TagName = "govuk-date-input-year-label";

    /// <summary>
    /// Creates a <see cref="DateInputItemLabelTagHelperBase"/>.
    /// </summary>
    public DateInputYearLabelTagHelper()
    {
    }
}
