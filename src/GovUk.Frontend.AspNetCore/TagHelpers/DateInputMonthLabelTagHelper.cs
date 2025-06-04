using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GovUk.Frontend.AspNetCore.TagHelpers;

/// <summary>
/// Represents the label in the month item of a GDS date input component.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = DateInputMonthTagHelper.TagName)]
public class DateInputMonthLabelTagHelper : DateInputItemLabelTagHelperBase
{
    internal const string TagName = "govuk-date-input-month-label";

    /// <summary>
    /// Creates a <see cref="DateInputItemLabelTagHelperBase"/>.
    /// </summary>
    public DateInputMonthLabelTagHelper()
    {
    }
}
